FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
ENV ASPNETCORE_URLS="http://+:80"
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["UrlBucket/UrlBucket.csproj", "UrlBucket/"]
RUN dotnet restore "UrlBucket/UrlBucket.csproj"
COPY . .
WORKDIR "/src/UrlBucket"
RUN dotnet build "UrlBucket.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "UrlBucket.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "UrlBucket.dll"]