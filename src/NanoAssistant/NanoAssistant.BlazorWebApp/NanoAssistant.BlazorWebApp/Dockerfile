#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NanoAssistant.BlazorWebApp/NanoAssistant.BlazorWebApp/NanoAssistant.BlazorWebApp.csproj", "NanoAssistant.BlazorWebApp/NanoAssistant.BlazorWebApp/"]
COPY ["NanoAssistant.Core/NanoAssistant.Core.csproj", "NanoAssistant.Core/"]
COPY ["NanoAssistant.Shared/NanoAssistant.Shared.csproj", "NanoAssistant.Shared/"]
COPY ["NanoAssistant.BlazorWebApp/NanoAssistant.BlazorWebApp.Client/NanoAssistant.BlazorWebApp.Client.csproj", "NanoAssistant.BlazorWebApp/NanoAssistant.BlazorWebApp.Client/"]
RUN dotnet restore "./NanoAssistant.BlazorWebApp/NanoAssistant.BlazorWebApp/NanoAssistant.BlazorWebApp.csproj"
COPY . .
WORKDIR "/src/NanoAssistant.BlazorWebApp/NanoAssistant.BlazorWebApp"
RUN dotnet build "./NanoAssistant.BlazorWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NanoAssistant.BlazorWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NanoAssistant.BlazorWebApp.dll"]