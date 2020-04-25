using System.Collections.Generic;

namespace Voidwell.Auth.Constants
{
    public static class ClientConstants
    {
        public static List<string> GetGrantTypes()
        {
            return new List<string>
                {
                    "implicit",
                    "client_credentials",
                    "authorization_code",
                    "hybrid",
                    "password",
                };
        }
    }
}
