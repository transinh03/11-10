# ================= Build stage =================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj và restore
COPY API/API.csproj API/
RUN dotnet restore API/API.csproj

# Copy toàn bộ source và publish
COPY . .
RUN dotnet publish API/API.csproj -c Release -o /app/publish

# ================= Runtime stage =================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Render sẽ set PORT env
ENV ASPNETCORE_URLS=http://+:$PORT

ENTRYPOINT ["dotnet", "API.dll"]
