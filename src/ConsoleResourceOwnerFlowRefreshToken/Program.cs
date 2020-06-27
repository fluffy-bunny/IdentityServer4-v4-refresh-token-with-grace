using Clients;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleResourceOwnerFlowRefreshToken
{
    public class Program
    {
        static HttpClient _tokenClient = new HttpClient();
        static DiscoveryCache _cache = new DiscoveryCache(Constants.Authority);

        static async Task Main()
        {
            Console.Title = "Console ResourceOwner Flow RefreshToken";

            var response = await RequestTokenAsync();
            response.Show();
            Console.ReadLine();

            await DoParentRemovedWhenChildComesInTestAsync(response.RefreshToken);

            // get a new token for the next run.
            response = await RequestTokenAsync();
            response.Show();
            Console.ReadLine();

            var childRefreshTokens = new List<string>();
            var refresh_token = response.RefreshToken;
            int nCount = 0;
            while (true)
            {
                // we are going to deplete our grace max count attempt by sending the
                // same refresh_token
                nCount += 1;
                try
                {
                    response = await RefreshTokenAsync(refresh_token);
                    ShowResponse(response);
                    Console.WriteLine($"nCount:{nCount}  - {refresh_token}");
                    Console.ReadLine();
                    await CallServiceAsync(response.AccessToken);
                    if (response.RefreshToken != refresh_token)
                    {
                        // store the children refresh_tokens 
                        childRefreshTokens.Add(response.RefreshToken);
                    }

                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
            nCount = 0;
            // all of these should be invalid, EXCEPT the last one.
            foreach(var childRefreshToken in childRefreshTokens)
            {
                try
                {
                    nCount += 1;
                    response = await RefreshTokenAsync(childRefreshToken);
                    ShowResponse(response);
                    Console.WriteLine($"nCount:{nCount}  - {refresh_token}");
                    Console.ReadLine();
                    await CallServiceAsync(response.AccessToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static async Task DoParentRemovedWhenChildComesInTestAsync(string refresh_token)
        {
            Console.WriteLine($"parent first attempt  - {refresh_token}");
            var response = await RefreshTokenAsync(refresh_token);
            ShowResponse(response);
            Console.ReadLine();

            // this proves that we got the child_refresh_token, by trying to redeem it.
            var child_refresh_token = response.RefreshToken;
            response = await RefreshTokenAsync(child_refresh_token);
            ShowResponse(response);

            Console.WriteLine($"parent second attempt - should fail  - {refresh_token}");
            try
            {
                response = await RefreshTokenAsync(refresh_token);
                ShowResponse(response);
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task<TokenResponse> RequestTokenAsync()
        {
            var disco = await _cache.GetAsync();

            var response = await _tokenClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "roclient",
                ClientSecret = "secret",

                UserName = "bob",
                Password = "bob",

                Scope = "api1 offline_access",
            });

            if (response.IsError) throw new Exception(response.Error);
            return response;
        }

        private static async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            Console.WriteLine("Using refresh token: {0}", refreshToken);

            var disco = await _cache.GetAsync();
            var response = await _tokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "roclient",
                ClientSecret = "secret",
                RefreshToken = refreshToken
            });

            if (response.IsError) throw new Exception(response.Error);
            return response;
        }

        static async Task CallServiceAsync(string token)
        {
            var baseAddress = Constants.SampleApi;

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            client.SetBearerToken(token);
            var response = await client.GetStringAsync("identity");

            "\n\nService claims:".ConsoleGreen();
            Console.WriteLine(JArray.Parse(response));
        }

        private static void ShowResponse(TokenResponse response)
        {
            if (!response.IsError)
            {
                "Token response:".ConsoleGreen();
                Console.WriteLine(response.Json);

                if (response.AccessToken.Contains("."))
                {
                    "\nAccess Token (decoded):".ConsoleGreen();

                    var parts = response.AccessToken.Split('.');
                    var header = parts[0];
                    var claims = parts[1];

                    Console.WriteLine(JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(header))));
                    Console.WriteLine(JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(claims))));
                }
            }
            else
            {
                if (response.ErrorType == ResponseErrorType.Http)
                {
                    "HTTP error: ".ConsoleGreen();
                    Console.WriteLine(response.Error);
                    "HTTP status code: ".ConsoleGreen();
                    Console.WriteLine(response.HttpStatusCode);
                }
                else
                {
                    "Protocol error response:".ConsoleGreen();
                    Console.WriteLine(response.Json);
                }
            }
        }
    }
}
