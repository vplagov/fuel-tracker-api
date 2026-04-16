FROM mcr.microsoft.com/dotnet/sdk:10.0.103-alpine3.23 AS build
WORKDIR /src
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
COPY ["FuelTracker.sln", "global.json", "./"]
COPY ["FuelTracker.API/FuelTracker.API.csproj", "FuelTracker.API/"]
RUN dotnet restore "FuelTracker.API/FuelTracker.API.csproj"
COPY . .
WORKDIR "/src/FuelTracker.API"
RUN dotnet publish "FuelTracker.API.csproj" -c Release -o /app/publish
RUN dotnet ef migrations bundle --self-contained -r linux-musl-x64 -o /app/publish/migrate

FROM mcr.microsoft.com/dotnet/aspnet:10.0.3-alpine3.23 AS final
USER $APP_UID
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
CMD ["dotnet", "FuelTracker.API.dll"]
