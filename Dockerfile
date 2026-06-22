# Stage 1: Build React frontend
FROM node:20-alpine AS client-build
WORKDIR /app/client
COPY MusicStoreFaker.Web/clientapp/package.json MusicStoreFaker.Web/clientapp/package-lock.json* ./
RUN npm ci --silent
COPY MusicStoreFaker.Web/clientapp/ ./
RUN npm run build

# Stage 2: Build .NET backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS server-build
WORKDIR /src
COPY . .
RUN dotnet restore MusicStoreFaker.Web/MusicStoreFaker.Web.csproj
RUN dotnet publish MusicStoreFaker.Web/MusicStoreFaker.Web.csproj -c Release -o /app/publish

# Stage 3: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=server-build /app/publish .
COPY --from=client-build /app/client/build ./wwwroot

# Ensure the SoundFont is available (if it wasn't published automatically)
COPY MusicStoreFaker.Web/wwwroot/soundfonts ./wwwroot/soundfonts

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80
ENTRYPOINT ["dotnet", "MusicStoreFaker.Web.dll"]