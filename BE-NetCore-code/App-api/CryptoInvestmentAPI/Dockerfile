# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 4010
EXPOSE 4011


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CryptoInvestmentAPI/CryptoInvestment.API.csproj", "CryptoInvestmentAPI/"]
COPY ["CryptoCore/CryptoCore.csproj", "CryptoCore/"]
COPY ["CryptoInfrastructure/CryptoInfrastructure.csproj", "CryptoInfrastructure/"]
COPY ["ServiceBusDelivery/ServiceBusDelivery.csproj", "ServiceBusDelivery/"]
RUN dotnet restore "./CryptoInvestmentAPI/CryptoInvestment.API.csproj"
COPY . .
WORKDIR "/src/CryptoInvestmentAPI"
RUN dotnet build "./CryptoInvestment.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CryptoInvestment.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoInvestment.API.dll"]