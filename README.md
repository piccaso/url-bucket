# url-bucket
Small wrapper around [minio](https://www.minio.io/) to store web assets.  

It provides a REST API and includes Swagger UI - packaged as [docker container](https://hub.docker.com/r/0xff/url-bucket/).  
By default it will use the demo minio instance running at [play.minio.io](http://play.minio.io/).  

The latest API can be viewed online: [editor.swagger.io](https://editor.swagger.io?url=https://raw.githubusercontent.com/piccaso/url-bucket/master/UrlBucket.Client/swagger.json)  

For a quick demo run:
```sh
docker run -d -p 8080:8080 0xff/url-bucket
```
And point your Browser to [localhost:8080](http://localhost:8080/).

To use your own instance of minio (or any S3 compatible storage) you have to set some environment variables.  
To get started have a look at the provided [docker-compose.yml](samples/docker-compose.yml) example.

[![Badge](https://quay.io/repository/0xff/url-bucket/status "Badge")](https://quay.io/repository/0xff/url-bucket)
[![Build Status](https://dev.azure.com/volatile-void/pipes/_apis/build/status/piccaso.url-bucket)](https://dev.azure.com/volatile-void/pipes/_build/latest?definitionId=1)
