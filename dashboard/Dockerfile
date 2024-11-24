# Adjust based on Dashboard's tech stack. If it’s .NET, use a similar pattern:
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ../../noizera.sln ./
COPY ../shared ./shared
COPY ./Dashboard ./dashboard/Dashboard

RUN dotnet restore ./dashboard/Dashboard/Dashboard.csproj
RUN dotnet build ./dashboard/Dashboard/Dashboard.csproj -c Release -o /app/build
RUN dotnet publish ./dashboard/Dashboard/Dashboard.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5143
ENTRYPOINT ["dotnet", "Dashboard.dll"]


# version label
LABEL version="1.0.0"