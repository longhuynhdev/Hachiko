# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first for layer caching
COPY EC-Project.sln ./
COPY Hachiko/Hachiko.csproj Hachiko/
COPY Hachiko.Models/Hachiko.Models.csproj Hachiko.Models/
COPY Hachiko.DataAccess/Hachiko.DataAccess.csproj Hachiko.DataAccess/
COPY Hachiko.Utility/Hachiko.Utility.csproj Hachiko.Utility/

RUN dotnet restore EC-Project.sln

# Copy remaining source files and publish
COPY . .
RUN dotnet publish Hachiko/Hachiko.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create directory for uploaded product images
RUN mkdir -p wwwroot/images/product

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Hachiko.dll"]
