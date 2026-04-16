# Fenio1 API – Docker za Railway/Render/Fly.io deployment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Fenio1.sln ./
COPY src/Fenio1.Core/Fenio1.Core.csproj src/Fenio1.Core/
COPY src/Fenio1.Infrastructure/Fenio1.Infrastructure.csproj src/Fenio1.Infrastructure/
COPY src/Fenio1.API/Fenio1.API.csproj src/Fenio1.API/

RUN dotnet restore src/Fenio1.API/Fenio1.API.csproj

COPY . .
RUN dotnet publish src/Fenio1.API/Fenio1.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# SQLite baza će biti u /app/data (persistent volume na Railway)
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/fenio1.db"
ENV ASPNETCORE_URLS="http://+:8080"
ENV ASPNETCORE_ENVIRONMENT="Production"

EXPOSE 8080

# Kreira /data direktorij ako ne postoji
RUN mkdir -p /app/data

COPY src/Fenio1.API/fenio1.db /app/data/fenio1.db

ENTRYPOINT ["dotnet", "Fenio1.API.dll"]
