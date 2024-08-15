FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DwgSvgConversion/DwgSvgConversion.csproj", "DwgSvgConversion/"]
RUN dotnet restore "DwgSvgConversion/DwgSvgConversion.csproj"
COPY . .

WORKDIR "/src/DwgSvgConversion"
RUN dotnet build "DwgSvgConversion.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DwgSvgConversion.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DwgSvgConversion.dll"]
