#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ProductManager.WebApi/ProductManager.WebApi.csproj", "src/ProductManager.WebApi/"]
COPY ["src/ProductManager.Application/ProductManager.Application.csproj", "src/ProductManager.Application/"]
COPY ["src/ProductManager.Domain/ProductManager.Domain.csproj", "src/ProductManager.Domain/"]
COPY ["src/ProductManager.Infrastructure/ProductManager.Infrastructure.csproj", "src/ProductManager.Infrastructure/"]
RUN dotnet restore "./src/ProductManager.WebApi/ProductManager.WebApi.csproj"
COPY . .
WORKDIR "/src/src/ProductManager.WebApi"
RUN dotnet build "./ProductManager.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProductManager.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductManager.WebApi.dll"]