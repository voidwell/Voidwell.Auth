pushd %~dp0\src\Voidwell.Auth
set ASPNETCORE_ENVIRONMENT=Development
dotnet ef migrations add configurationdb.release.1 -v ^
    -c IdentityServer4.EntityFramework.DbContexts.ConfigurationDbContext ^
    -o ./Data/Migrations/ConfigurationDb ^
    --msbuildprojectextensionspath ./../../build/Voidwell.Auth/Debug/obj
dotnet ef migrations add persistedgrantdb.release.1 -v ^
    -c IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext ^
    -o ./Data/Migrations/PersistedGrantDb ^
    --msbuildprojectextensionspath ./../../build/Voidwell.Auth/Debug/obj
popd