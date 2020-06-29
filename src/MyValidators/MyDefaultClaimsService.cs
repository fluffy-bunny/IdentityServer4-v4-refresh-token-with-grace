using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer4.Validation
{
    public class MyDefaultClaimsService : DefaultClaimsService
    {
        private IOptionalClaims _optionalClaims;

        public MyDefaultClaimsService(
            IProfileService profile, 
            IOptionalClaims optionalClaims,
            ILogger<MyDefaultClaimsService> logger) : base(profile, logger)
        {
            _optionalClaims = optionalClaims;
        }
        protected override IEnumerable<Claim> GetOptionalClaims(ClaimsPrincipal subject)
        {
            var claims = new List<Claim>();
            claims.AddRange(base.GetOptionalClaims(subject));
            if (_optionalClaims.Claims != null)
            {
                claims.AddRange(_optionalClaims.Claims);
            }
            return claims;
        }
    }
}

