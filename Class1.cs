using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Net;
using Newtonsoft.Json.Linq;

namespace YouTubeChat
{
    public static class Class1
    {
        const int EVENT_OTHER_SOCIAL = 70;

        delegate void GmlEventPerformAsyncDelegate(int map, int event_type);
        delegate int GmlDsMapCreateDelegate(int n);
        delegate bool GmlDsMapAddDoubleDelegate(int map, string key, double value);
        delegate bool GmlDsMapAddStringDelegate(int map, string key, string value);

        static GmlEventPerformAsyncDelegate GmlEventPerformAsync;
        static GmlDsMapCreateDelegate GmlDsMapCreate;
        static GmlDsMapAddDoubleDelegate GmlDsMapAddDouble;
        static GmlDsMapAddStringDelegate GmlDsMapAddString;

        [DllExport("RegisterCallbacks", CallingConvention.Cdecl)]
        public static unsafe double RegisterCallbacks(char* arg1, char* arg2, char* arg3, char* arg4)
        {
            GmlEventPerformAsync = Marshal.GetDelegateForFunctionPointer<GmlEventPerformAsyncDelegate>(new IntPtr(arg1));
            GmlDsMapCreate = Marshal.GetDelegateForFunctionPointer<GmlDsMapCreateDelegate>(new IntPtr(arg2));
            GmlDsMapAddDouble = Marshal.GetDelegateForFunctionPointer<GmlDsMapAddDoubleDelegate>(new IntPtr(arg3));
            GmlDsMapAddString = Marshal.GetDelegateForFunctionPointer<GmlDsMapAddStringDelegate>(new IntPtr(arg4));

            return 0;
        }

        [DllExport("Get_Chat", CallingConvention.Cdecl)]
        public static string Get_Chat(string str)
        {
            Task.Run(() =>
            {
                Task.Delay(1000).Wait();


                string ReturnString = "NASA Space Station";

                string videoId = str;
                string videoLink;
                videoLink = "https://www.googleapis.com/youtube/v3/videos?id=" + videoId + "&key=AIzaSyAXBJ8kKZPSyohUfhxMs63IjfpTlkU26-s&part=liveStreamingDetails,snippet";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(videoLink);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (System.IO.Stream stream = response.GetResponseStream())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    JObject joResponse = JObject.Parse(reader.ReadToEnd());
                    String liveChatId = joResponse["items"][0]["liveStreamingDetails"]["activeLiveChatId"].ToString();
                    String chatListLink = "https://www.googleapis.com/youtube/v3/liveChat/messages?liveChatId=" + liveChatId + "&part=id%2C+snippet%2C+authorDetails&key=AIzaSyAXBJ8kKZPSyohUfhxMs63IjfpTlkU26-s";
                    HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(chatListLink);
                    using (HttpWebResponse response2 = (HttpWebResponse)request2.GetResponse())
                    using (System.IO.Stream stream2 = response2.GetResponseStream())
                    using (System.IO.StreamReader reader2 = new System.IO.StreamReader(stream2))
                    {
                        JObject joResponse2 = JObject.Parse(reader2.ReadToEnd());
                        foreach (var item in joResponse2["items"])
                        {
                            ReturnString = Convert.ToString(item["snippet"]["displayMessage"]);
                        }
                    }
                }



                
                var map = GmlDsMapCreate(0);
                GmlDsMapAddString(map, "Chat", ReturnString);
                GmlEventPerformAsync(map, EVENT_OTHER_SOCIAL);
            });

            

            return "Running...";
        }
    }
}
