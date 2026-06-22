# Stage 1: Build React frontend
FROM node:20-alpine AS client-build
WORKDIR /app/client
COPY MusicStoreFaker.Web/clientapp/package.json MusicStoreFaker.Web/clientapp/package-lock.json* ./
RUN npm ci --silent
COPY MusicStoreFaker.Web/clientapp/ ./
RUN npm run build

# Stage 2: Build .NET backend – self‑contained for Linux
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS server-build
WORKDIR /src
COPY . .
RUN dotnet restore MusicStoreFaker.Web/MusicStoreFaker.Web.csproj -r linux-x64
RUN dotnet publish MusicStoreFaker.Web/MusicStoreFaker.Web.csproj \
    -r linux-x64 --self-contained true -c Release -o /app/publish

# Stage 3: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Only font libraries for SkiaSharp (LAME no longer needed)
RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    libfreetype6 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=server-build /app/publish .
COPY --from=client-build /app/client/build ./wwwroot
COPY MusicStoreFaker.Web/wwwroot/soundfonts ./wwwroot/soundfonts

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80
ENTRYPOINT ["dotnet", "MusicStoreFaker.Web.dll"]