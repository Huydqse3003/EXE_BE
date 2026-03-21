# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy sln + csproj trước (tối ưu cache)
COPY EXE_BE/*.sln ./

COPY EXE_BE/EXE_BE.API/*.csproj EXE_BE/EXE_BE.API/
COPY EXE_BE/EXE_BE.Application/*.csproj EXE_BE/EXE_BE.Application/
COPY EXE_BE/EXE_BE.Domain/*.csproj EXE_BE/EXE_BE.Domain/
COPY EXE_BE/EXE_BE.Infrastructure/*.csproj EXE_BE/EXE_BE.Infrastructure/

# Restore
RUN dotnet restore EXE_BE/EXE_BE.API/EXE_BE.API.csproj

# Copy full source
COPY . .

# Publish
RUN dotnet publish EXE_BE.API/EXE_BE.API.csproj -c Release -o /app/publish

# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

COPY --from=build /app/publish .

# 🔥 Quan trọng cho Render
ENV ASPNETCORE_URLS=http://+:${PORT}
ENV ASPNETCORE_ENVIRONMENT=Production

# Render sẽ inject PORT (thường là 10000)
EXPOSE 10000

ENTRYPOINT ["dotnet", "EXE_BE.API.dll"]