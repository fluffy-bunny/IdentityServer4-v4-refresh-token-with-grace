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
                if (_refreshTokenGraceEnabled == null) return false;
                return (bool)_refreshTokenGraceEnabled;
            }
            set { _refreshTokenGraceEnabled = value; }
        }
        private int? _refreshTokenGraceTTL;
        public int RefreshTokenGraceTTL
        {
            get
            {
                if (_refreshTokenGraceTTL == null) return 0;
                return (int)_refreshTokenGraceTTL;
            }
            set { _refreshTokenGraceTTL = value; }
        }
        private int? _refreshTokenGraceMaxAttempts;
        public int RefreshTokenGraceMaxAttempts
        {
            get
            {
                if (_refreshTokenGraceMaxAttempts == null) return 0;
                return (int)_refreshTokenGraceMaxAttempts;
            }
            set { _refreshTokenGraceMaxAttempts = value; }
        }

    }
}
