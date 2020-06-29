using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer4.Services
{
    internal class OptionalClaims : IOptionalClaims
    {
        List<Claim> _claims;
        public List<Claim> Claims
        {
            get
            {
                if (_claims == null)
                {
                    _claims = new List<Claim>();
                }
                return _claims;
            }

        }
    }
}
