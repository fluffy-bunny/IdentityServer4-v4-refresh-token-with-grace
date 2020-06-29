using IdentityServer4.Models;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServer4.Services.Extensions
{
    public static class ApiResourceExtensions
    {
        public static List<string> ToScopeNames(this List<ApiResource> self)
        {
            var query = from item in self
                        select item.Name;
            return query.ToList();
        }
    }
}
