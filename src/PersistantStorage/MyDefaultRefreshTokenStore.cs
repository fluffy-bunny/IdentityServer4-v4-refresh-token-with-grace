using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Services.Models;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PersistantStorage
{
    public class MyDefaultRefreshTokenStore : DefaultGrantStore<RefreshTokenExtra>, IRefreshTokenStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRefreshTokenStore"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="handleGenerationService">The handle generation service.</param>
        /// <param name="logger">The logger.</param>
        public MyDefaultRefreshTokenStore(
            IPersistedGrantStore store,
            IPersistentGrantSerializer serializer,
            IHandleGenerationService handleGenerationService,
            ILogger<DefaultRefreshTokenStore> logger)
            : base(IdentityServerConstants.PersistedGrantTypes.RefreshToken, store, serializer, handleGenerationService, logger)
        {
        }

        /// <summary>
        /// Stores the refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns></returns>
        public async Task<string> StoreRefreshTokenAsync(RefreshToken refreshToken)
        {
            return await CreateItemAsync(refreshToken as RefreshTokenExtra, refreshToken.ClientId, refreshToken.SubjectId, refreshToken.SessionId, refreshToken.Description, refreshToken.CreationTime, refreshToken.Lifetime);
        }

        /// <summary>
        /// Updates the refresh token.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns></returns>
        public Task UpdateRefreshTokenAsync(string handle, RefreshToken refreshToken)
        {
            return StoreItemAsync(handle, refreshToken as RefreshTokenExtra, refreshToken.ClientId, refreshToken.SubjectId, refreshToken.SessionId, refreshToken.Description, refreshToken.CreationTime, refreshToken.CreationTime.AddSeconds(refreshToken.Lifetime), refreshToken.ConsumedTime);
        }

        /// <summary>
        /// Gets the refresh token.
        /// </summary>
        /// <param name="refreshTokenHandle">The refresh token handle.</param>
        /// <returns></returns>
        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshTokenHandle)
        {
            return await GetItemAsync(refreshTokenHandle) as RefreshToken;
        }

        /// <summary>
        /// Removes the refresh token.
        /// </summary>
        /// <param name="refreshTokenHandle">The refresh token handle.</param>
        /// <returns></returns>
        public Task RemoveRefreshTokenAsync(string refreshTokenHandle)
        {
            return RemoveItemAsync(refreshTokenHandle);
        }

        /// <summary>
        /// Removes the refresh tokens.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public Task RemoveRefreshTokensAsync(string subjectId, string clientId)
        {
            return RemoveAllAsync(subjectId, clientId);
        }
        protected override string GetHashedKey(string value)
        {
            var ori = base.GetHashedKey(value);
            ori = ori.Replace('/', '_');
            ori = ori.Replace('-', '+');
            return ori;
        }
    }
}
