using ClientStore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientStore.Extensions
{
    public static class ClientExtraExtensions
    {
        public static bool IsRefreshGraceEnabled(this ClientExtra client)
        {
            if (client.RefreshTokenGraceEnabled == null) return false;
            return (bool) client.RefreshTokenGraceEnabled;
        }
    }
}
