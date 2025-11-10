FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RpWeave.Server.Api/RpWeave.Server.Api.csproj", "RpWeave.Server.Api/"]
RUN dotnet restore "RpWeave.Server.Api/RpWeave.Server.Api.csproj"
COPY . .
WORKDIR "/src/RpWeave.Server.Api"
RUN dotnet build "RpWeave.Server.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RpWeave.Server.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install Tesseract
RUN apt-get update && apt-get install -y tesseract-ocr tesseract-ocr-eng libtesseract-dev && rm -rf /var/lib/apt/lists/*

ENTRYPOINT ["dotnet", "RpWeave.Server.Api.dll"]
