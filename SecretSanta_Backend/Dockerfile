#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://*:5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SecretSanta_Backend/SecretSanta_Backend.csproj", "SecretSanta_Backend/"]
RUN dotnet restore "SecretSanta_Backend/SecretSanta_Backend.csproj"
COPY . .
WORKDIR "/src/SecretSanta_Backend"
RUN dotnet build "SecretSanta_Backend.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "SecretSanta_Backend.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SecretSanta_Backend.dll"]