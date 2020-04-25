FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

# Copy and restore as distinct layers
COPY *.sln ./
COPY ./src/Voidwell.Auth/*.csproj ./src/Voidwell.Auth/
COPY ./src/Voidwell.Auth.Data/*.csproj ./src/Voidwell.Auth.Data/
COPY ./src/Voidwell.Auth.Admin/*.csproj ./src/Voidwell.Auth.Admin/
COPY ./src/Voidwell.Common/*.csproj ./src/Voidwell.Common/

RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime

# Copy the app
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000

# Start the app
ENTRYPOINT dotnet Voidwell.Auth.dll