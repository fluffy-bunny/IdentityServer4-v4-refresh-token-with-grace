namespace IdentityServer4.Hosting
{
    internal class TenantResolver : ITenantResolver
    {
        public bool IsTenantValid(string tenantId)
        {
            return true;
        }
    }
}
