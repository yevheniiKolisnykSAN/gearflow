FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY GearFlow/GearFlow.sln .
COPY GearFlow/GearFlow.API/GearFlow.API.csproj GearFlow.API/
COPY GearFlow/GearFlow.Application/GearFlow.Application.csproj GearFlow.Application/
COPY GearFlow/GearFlow.Domain/GearFlow.Domain.csproj GearFlow.Domain/
COPY GearFlow/GearFlow.Infrastructure/GearFlow.Infrastructure.csproj GearFlow.Infrastructure/
RUN dotnet restore GearFlow.sln

COPY GearFlow/ .
RUN dotnet publish GearFlow.API/GearFlow.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "GearFlow.API.dll"]
