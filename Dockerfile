# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy solution
COPY . .

# Restore
RUN dotnet restore

# Publish API
RUN dotnet publish EXE_BE.API/EXE_BE.API.csproj -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

COPY --from=build /out .

# Render yêu cầu PORT
ENV ASPNETCORE_URLS=http://+:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "EXE_BE.API.dll"]