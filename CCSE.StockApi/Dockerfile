#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CCSE.StockApi/CCSE.StockApi.csproj", "CCSE.StockApi/"]
RUN dotnet restore "CCSE.StockApi/CCSE.StockApi.csproj"
COPY . .
WORKDIR "/src/CCSE.StockApi"
RUN dotnet build "CCSE.StockApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CCSE.StockApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CCSE.StockApi.dll"]