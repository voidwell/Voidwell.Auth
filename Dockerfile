# --- Build (restore + publish) ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY *.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

COPY ./src/Voidwell.Auth/*.csproj ./src/Voidwell.Auth/
COPY ./src/Voidwell.Auth.Data/*.csproj ./src/Voidwell.Auth.Data/
COPY ./src/Voidwell.Auth.Admin/*.csproj ./src/Voidwell.Auth.Admin/
COPY ./src/Voidwell.Auth.IdentityServer/*.csproj ./src/Voidwell.Auth.IdentityServer/
COPY ./src/Voidwell.Auth.UserManagement/*.csproj ./src/Voidwell.Auth.UserManagement/

RUN --mount=type=cache,target=/root/.nuget/packages dotnet restore --nologo

COPY . .

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -c Release -o /app/publish --no-restore ./src/Voidwell.Auth/Voidwell.Auth.csproj

# --- Runtime ---
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app

COPY --from=build /app/publish ./

EXPOSE 5000

ENV ASPNETCORE_URLS http://*:5000
ENV ASPNETCORE_ENVIRONMENT Production

ENTRYPOINT ["dotnet", "Voidwell.Auth.dll"]
