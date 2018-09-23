# url-bucket
Small wrapper around [minio](https://www.minio.io/) to store web assets.  

`url-bucket` provides a REST API and includes Swagger UI.  
All packaged as [docker container](https://hub.docker.com/r/0xff/url-bucket/).
By default it will use the demo minio instance running at [play.minio.io](http://play.minio.io/).

For a quick demo run:
```sh
docker run -d -p 8080:80 0xff/url-bucket
```
And point your Browser to [localhost:8080](http://localhost:8080/).

To use your own instance of minio (or any S3 compatible storage) you have to set some environment variables.  
To get started have a look at the provided [docker-compose.yml](samples/docker-compose.yml) example.
