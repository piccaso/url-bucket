FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine AS base
WORKDIR /app
ENV ASPNETCORE_URLS="http://+:80"
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /src
COPY ["UrlBucket/*.csproj", "UrlBucket/"]
COPY ["UrlBucket.Lib/*.csproj", "UrlBucket.Lib/"]
RUN cd UrlBucket.Lib/ && dotnet restore 
COPY . .
WORKDIR /src/UrlBucket

FROM build AS publish
RUN dotnet publish "UrlBucket.csproj" -c Release -o /app
RUN chmod -R 0400 /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "UrlBucket.dll"]