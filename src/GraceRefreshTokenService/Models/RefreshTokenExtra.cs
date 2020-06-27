using IdentityServer4.Models;
using System;

namespace IdentityServer4.Services.Models
{
    public class RefreshTokenExtra : RefreshToken
    {
        public string RefeshTokenParent { get; set; }
        public string RefeshTokenChild { get; set; }
        public int ConsumedAttempts { get; set; } = 0;
    }
}
