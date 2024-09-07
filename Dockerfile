# Use the official .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

EXPOSE 80

# Copy the project file and restore dependencies
COPY ["EventStaf.csproj", "./"]
RUN dotnet restore "EventStaf.csproj"

# Copy the rest of the source code
COPY . .

# Build the project
RUN dotnet build "EventStaf.csproj" -c Release -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "EventStaf.csproj" -c Release -o /app/publish

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventStaf.dll"]
