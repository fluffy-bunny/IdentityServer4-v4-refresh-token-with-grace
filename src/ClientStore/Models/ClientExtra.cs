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

    }
}
