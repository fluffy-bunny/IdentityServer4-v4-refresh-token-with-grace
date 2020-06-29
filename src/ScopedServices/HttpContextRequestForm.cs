using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace IdentityServer4.Services
{
    public class HttpContextRequestForm : IHttpContextRequestForm
    {
        private IHttpContextAccessor _httpContextAccessor;

        public HttpContextRequestForm(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        NameValueCollection _form;
        public async Task<NameValueCollection> GetFormCollectionAsync()
        {
            if(_form == null)
            {
                _form = (await _httpContextAccessor.HttpContext.Request.ReadFormAsync()).AsNameValueCollection();
            }
            return _form;
        }
    }
}
