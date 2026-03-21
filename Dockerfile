# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY . .

# 🔥 CHỈ RÕ FILE SLN
RUN dotnet restore EXE_BE.sln

# 🔥 BUILD API
RUN dotnet publish EXE_BE.API/EXE_BE.API.csproj -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

COPY --from=build /out .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "EXE_BE.API.dll"]