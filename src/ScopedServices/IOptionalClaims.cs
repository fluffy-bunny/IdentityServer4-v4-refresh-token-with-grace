using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer4.Services
{
    public interface IOptionalClaims
    {
         List<Claim> Claims { get; }
    }
}
