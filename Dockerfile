# ===== BUILD =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier la solution et le projet
COPY csharp_web.sln .
COPY csharp_web/csharp_web.csproj ./csharp_web/

# Restaurer les dépendances
RUN dotnet restore ./csharp_web/csharp_web.csproj

# Copier tout le reste
COPY . .

# Publier l'application
WORKDIR /src/csharp_web
RUN dotnet publish -c Release -o /app/publish --no-restore

# ===== RUNTIME =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENV PORT=8080
ENV ASPNETCORE_URLS=http://+:$PORT
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ConnectionStrings__DefaultConnection="Data Source=local.db"
EXPOSE 8080

ENTRYPOINT ["dotnet", "csharp_web.dll"]
