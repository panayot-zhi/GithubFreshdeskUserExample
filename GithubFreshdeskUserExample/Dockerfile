#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

ENV FRESHDESK__TOKEN=CHANGE_ME
ENV GITHUB__TOKEN=CHANGE_ME

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GithubFreshdeskUserExample.csproj", "."]
RUN dotnet restore "./././GithubFreshdeskUserExample.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./GithubFreshdeskUserExample.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GithubFreshdeskUserExample.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GithubFreshdeskUserExample.dll"]