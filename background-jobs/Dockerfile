# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the entire solution and all projects
COPY ../../noizera.sln ./
COPY ../shared ./shared
COPY ./src ./background-jobs/src

# Restore only the relevant project
RUN dotnet restore ./background-jobs/src/Noizera.BackgroundJobs/Noizera.BackgroundJobs.csproj

# Build the project
RUN dotnet build ./background-jobs/src/Noizera.BackgroundJobs/Noizera.BackgroundJobs.csproj -c Release -o /app/build

# Publish the project
RUN dotnet publish ./background-jobs/src/Noizera.BackgroundJobs/Noizera.BackgroundJobs.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5125
ENTRYPOINT ["dotnet", "Noizera.BackgroundJobs.dll"]

# version label
LABEL version="1.0.0"