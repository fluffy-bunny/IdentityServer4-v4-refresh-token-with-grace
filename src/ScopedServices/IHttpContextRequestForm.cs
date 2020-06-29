using System.Collections.Specialized;
using System.Threading.Tasks;

namespace IdentityServer4.Services
{
    public interface IHttpContextRequestForm
    {
        Task<NameValueCollection> GetFormCollectionAsync();
    }
}
