using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientStore.Models
{
    public class ClientExtra : Client
    {
       
        public ClientExtra()
        {

        }
        public ClientExtra ShallowCopy()
        {
            return (ClientExtra) MemberwiseClone();
        }
        private bool? _refreshTokenGraceEnabled;
        public bool RefreshTokenGraceEnabled
        {
            get
            {
                return _refreshTokenGraceEnabled == null ? false : (bool)_refreshTokenGraceEnabled;
            }
            set { _refreshTokenGraceEnabled = value; }
        }
        private int? _refreshTokenGraceTTL;
        public int RefreshTokenGraceTTL
        {
            get
            {
                return _refreshTokenGraceTTL == null ? 0 : (int)_refreshTokenGraceTTL;
            }
            set { _refreshTokenGraceTTL = value; }
        }
        private int? _refreshTokenGraceMaxAttempts;
        public int RefreshTokenGraceMaxAttempts
        {
            get
            {
                return _refreshTokenGraceMaxAttempts == null ? 0 : (int)_refreshTokenGraceMaxAttempts;
            }
            set { _refreshTokenGraceMaxAttempts = value; }
        }

        private bool? _requireRefreshClientSecret;
        public bool RequireRefreshClientSecret
        {
            get
            {
                return _requireRefreshClientSecret == null ? true : (bool)_requireRefreshClientSecret;
            }
            set { _requireRefreshClientSecret = value; }
        }

        // https://tools.ietf.org/html/draft-ietf-oauth-access-token-jwt-07
        // Once this is finalized this will be hard coded to REQUIRED/true.
        /// <summary>
        /// Include the client_id in the final access_token
        /// </summary>
        private bool? _includeClientId;
        public bool IncludeClientId
        {
            get
            {
                return _includeClientId == null ? true : (bool)_includeClientId;
            }
            set { _includeClientId = value; }
        }
        private bool? _includeAmr;
        public bool IncludeAmr
        {
            get
            {
                return _includeAmr == null ? true : (bool)_includeAmr;
            }
            set { _includeAmr = value; }
        }
    }
}
