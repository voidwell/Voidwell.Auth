using System.Reflection;

namespace Voidwell.VoidwellAuth.Data
{
    internal static class Configuration
    {
        public static string ConnectionString = @"Data Source=HYPERION\SQLEXPRESS;Database=VoidwellAuth;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";
        public static string MigrationsAssembly = typeof(Configuration).GetTypeInfo().Assembly.GetName().Name;
    }
}
