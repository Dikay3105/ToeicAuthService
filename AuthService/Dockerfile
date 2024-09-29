# Use SDK image for building the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY AuthService/AuthService.csproj AuthService/
RUN dotnet restore AuthService/AuthService.csproj

# Copy the rest of the application code
COPY AuthService/ AuthService/

# Publish the project
RUN dotnet publish AuthService/AuthService.csproj -c Release -o /app/publish

# Use ASP.NET image for the final stage (production)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set the entry point for the application
ENTRYPOINT ["dotnet", "AuthService.dll"]
