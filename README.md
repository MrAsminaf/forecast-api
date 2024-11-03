## Building and running a Docker image
After cloning the repo, you can run these commands to build and run a docker image based on provided dockerfile

    docker build . -t forecast-api
    docker run -p 8080:8080 forecast-api

You can then access Swagger spec by entering

    http://localhost:8080/swagger/index.html
