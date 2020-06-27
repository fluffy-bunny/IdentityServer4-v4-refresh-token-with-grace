using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientStore.Models
{
    public class ClientExtra : Client
    {
        private bool? _requireRefreshClientSecret;
        public ClientExtra()
        {

        }
        public ClientExtra ShallowCopy()
        {
            return (ClientExtra) MemberwiseClone();
        }

        //
        // Summary:
        //     If set to false, no client secret is needed to refresh tokens at the token endpoint
        //     (defaults to RequireClientSecret)
        public bool RequireRefreshClientSecret
        {
            get
            {
                if (_requireRefreshClientSecret == null || RequireClientSecret == false)
                    return RequireClientSecret;
                return (bool) _requireRefreshClientSecret;
            }
            set => _requireRefreshClientSecret = value;
        }

        public string TenantId { get; set; }
        public bool? IncludeClientId { get; set; }
        public bool? RefreshTokenGraceEnabled { get; set; }
        public int? RefreshTokenGraceTTL { get; set; }
        public int? RefreshTokenGraceMaxAttempts { get; set; }

    }
}
