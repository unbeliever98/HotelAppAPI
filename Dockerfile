# Use .NET 8.0 SDK as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution file and all project files
COPY ["HotelManagementApp.sln", "./"]
COPY ["SQLDataAccessLibrary/", "SQLDataAccessLibrary/"]
COPY ["hotelappi/", "hotelappi/"]

# Restore dependencies for the whole solution
RUN dotnet restore "HotelManagementApp.sln"

# Build and publish the API project
RUN dotnet publish "hotelappi/hotelappi.csproj" -c Release -o /out

# Use ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

EXPOSE 7230
ENTRYPOINT ["dotnet", "hotelappi.dll"]
