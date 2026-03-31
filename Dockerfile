# --- Build (restore + publish) ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY . ./

RUN dotnet restore

RUN dotnet publish -c Release -o /app/publish --nologo --no-restore ./src/Voidwell.Auth/Voidwell.Auth.csproj

# --- Runtime ---
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app

COPY --from=build /app/publish ./

EXPOSE 5000

ENV ASPNETCORE_URLS http://*:5000
ENV ASPNETCORE_ENVIRONMENT Production

ENTRYPOINT ["dotnet", "Voidwell.Auth.dll"]
