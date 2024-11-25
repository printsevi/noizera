# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the entire solution and all projects
COPY ./shared ./shared
COPY ./api/src ./src

# Restore only the relevant project
RUN dotnet restore ./src/Noizera.Api/Noizera.Api.csproj

# Build the project
RUN dotnet build ./src/Noizera.Api/Noizera.Api.csproj -c Release -o /app/build

# Publish the project
RUN dotnet publish ./src/Noizera.Api/Noizera.Api.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose the default port
EXPOSE 5000
ENTRYPOINT ["dotnet", "Noizera.Api.dll"]


# version label
LABEL version="1.0.0"