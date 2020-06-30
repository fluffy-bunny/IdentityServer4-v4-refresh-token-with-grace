using ClientStore.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using IdentityModel;

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
        public override async Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, 
            ResourceValidationResult resourceResult, ValidatedRequest request)
        {
            var claims =  await base.GetAccessTokenClaimsAsync(subject, resourceResult, request);
            var clientExtra = request.Client as ClientExtra;
            if (clientExtra.IncludeClientId)
            {
                return claims;
            }
            var query = from claim in claims
                        where claim.Type != JwtClaimTypes.ClientId
                        select claim;
            if (clientExtra.IncludeAmr)
            {
                return query;
            }
            else
            {
                var queryAmr = from claim in claims
                            where claim.Type == JwtClaimTypes.AuthenticationMethod
                               select claim;
                if(queryAmr.Count() == 1)
                {
                    // only meant to remove the single one that IDS4 added.
                    query = from claim in query
                            where claim.Type != JwtClaimTypes.AuthenticationMethod
                            select claim;
                }
            }
            return query;
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

