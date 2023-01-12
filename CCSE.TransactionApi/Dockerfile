#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CCSE.TransactionApi/CCSE.TransactionApi.csproj", "CCSE.TransactionApi/"]
RUN dotnet restore "CCSE.TransactionApi/CCSE.TransactionApi.csproj"
COPY . .
WORKDIR "/src/CCSE.TransactionApi"
RUN dotnet build "CCSE.TransactionApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CCSE.TransactionApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CCSE.TransactionApi.dll"]