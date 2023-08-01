FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["YoutubeApi/YoutubeApi.csproj", "YoutubeApi/"]
RUN dotnet restore "YoutubeApi/YoutubeApi.csproj"
COPY . .
WORKDIR "/src/YoutubeApi"
RUN dotnet build "YoutubeApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YoutubeApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YoutubeApi.dll"]
