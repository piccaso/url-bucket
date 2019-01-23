FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /src
COPY ["*.sln", "./"]
COPY ["*/*.csproj", "./"]
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore
COPY . .
RUN dotnet publish "UrlBucket/UrlBucket.csproj" -c Release -o /app
# Fix permissions (if built from windows filesystem)
RUN chmod -R a-w,a-x,a+X /app
RUN chown -R nobody:nobody /app

FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine
WORKDIR /app
ENV ASPNETCORE_URLS="http://+:8080"
EXPOSE 8080
WORKDIR /app
COPY --from=build /app .
USER nobody
#HEALTHCHECK CMD wget --quiet --tries=1 --spider http://127.0.0.1:8080/ || exit 1
ENTRYPOINT ["dotnet", "UrlBucket.dll"]
