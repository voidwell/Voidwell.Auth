pushd %~dp0\src\Voidwell.Auth
set ASPNETCORE_ENVIRONMENT=Development
dotnet ef database update -v ^
    -c IdentityServer4.EntityFramework.DbContexts.ConfigurationDbContext ^
    --msbuildprojectextensionspath ./../../build/Voidwell.Auth/Debug/obj
dotnet ef database update -v ^
    -c IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext ^
    --msbuildprojectextensionspath ./../../build/Voidwell.Auth/Debug/obj
popd