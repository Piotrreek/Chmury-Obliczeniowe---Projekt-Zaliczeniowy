FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Chmury.csproj", "Chmury/"]
RUN dotnet restore "Chmury/Chmury.csproj"

WORKDIR "/src/Chmury"
COPY . .

RUN dotnet build "Chmury.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "Chmury.csproj"  -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chmury.dll"]
