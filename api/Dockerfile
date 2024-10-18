# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#FROM mcr.microsoft.com/dotnet/nightly/sdk:9.0-preview AS build
WORKDIR /App

# Copy everything
COPY . ./

# dotnet restore
RUN dotnet restore

# dotnet publish
RUN dotnet publish -c Release -o out


# Stage 2: run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build /App/out .
EXPOSE 5000
EXPOSE 5001
ENTRYPOINT ["dotnet", "Noizera.Api.dll"]


# version label
LABEL version="1.0.0"