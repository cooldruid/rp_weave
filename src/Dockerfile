# Build Angular
FROM node:22 AS frontend
WORKDIR /app
COPY RpWeave.Client/ .
RUN npm ci && npm run build --prod

# Build .NET
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend
WORKDIR /src
COPY RpWeave.Server/ .
RUN dotnet publish -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=backend /app/publish .
COPY --from=frontend /app/dist/rp-weave.client/browser /app/wwwroot
EXPOSE 80
ENTRYPOINT ["dotnet", "RpWeave.Server.Api.dll"]