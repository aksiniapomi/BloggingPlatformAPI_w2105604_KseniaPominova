# Stage 1: Build (includes SDK)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /out

# Stage 2: Runtime (includes only ASP.NET)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy the application from build stage
COPY --from=build /out .

# Install SQLite inside the runtime container
RUN apt update && apt install -y sqlite3

# Copy the database file into the container, ensures your database is inside the containe 
COPY bloggingplatform.db /app/bloggingplatform.db

# Expose ports
EXPOSE 8080

# Set entrypoint
CMD ["dotnet", "BloggingPlatformAPI.dll"]