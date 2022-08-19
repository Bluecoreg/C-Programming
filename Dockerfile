# the software that will be in the virtual enivronment to start. Get from Dockerhub
FROM openjdk

# specify what to copy and where on the virtual environment to put it. Invent the destination on the venv.
COPY . /workspace

WORKDIR /workspace
# Any future commands executed in your script are from that directory

EXPOSE 8080

#Command to run when you build the container from the image
ENTRYPOINT ["java", "-jar", "all-in-one-jar-1.0-SNAPSHOT.jar"]