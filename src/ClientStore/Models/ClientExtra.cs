using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientStore.Models
{
    public class ClientExtra : Client
    {
       
        public ClientExtra()
        {

        }
        public ClientExtra ShallowCopy()
        {
            return (ClientExtra) MemberwiseClone();
        }
        
        public bool? RefreshTokenGraceEnabled { get; set; }
        public int? RefreshTokenGraceTTL { get; set; }
        public int? RefreshTokenGraceMaxAttempts { get; set; }

    }
}
