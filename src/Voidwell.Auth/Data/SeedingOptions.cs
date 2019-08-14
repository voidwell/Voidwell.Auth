using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voidwell.Auth.Data
{
    public class SeedingOptions
    {
        public string ApiResourceSecret { get; set; }
        public string AuthApiResourceSecret { get; set; }
        public string MessagewellResourceSecret { get; set; }

        public string AuthClientSecret { get; set; }
        public string ApiClientSecret { get; set; }
        public string UserManagementClientSecret { get; set; }
        public string InternalClientSecret { get; set; }
        public string BungieNetClientSecret { get; set; }
        public string DaybreakGamesClientSecret { get; set; }
        public string MutterblackClientSecret { get; set; }
        public string FilewellClientSecret { get; set; }

    }
}
