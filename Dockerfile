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
COPY --from=build /out .

# Expose ports
EXPOSE 8080

# Set entrypoint
CMD ["dotnet", "BloggingPlatformAPI.dll"]