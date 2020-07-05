namespace IdentityServer4.Hosting
{
    public interface ITenantResolver
    {
        bool IsTenantValid(string tenantId);
    }
}
