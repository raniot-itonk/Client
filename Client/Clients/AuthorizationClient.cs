using System;
using System.Net.Http;
using System.Threading.Tasks;
using Client.Models.Requests.AuthorizationService;
using Client.OptionModels;
using Flurl;
using Flurl.Http;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace Client.Clients
{
    public class AuthorizationClient
    {
        private readonly AuthorizationService _authorizationService;

        public AuthorizationClient(IOptionsMonitor<Services> serviceOptions)
        {
            _authorizationService = serviceOptions.CurrentValue.AuthorizationService ??
                                    throw new ArgumentNullException(nameof(serviceOptions.CurrentValue
                                        .AuthorizationService));
        }

        public async Task Register(RegisterRequest request)
        {
            await _authorizationService.BaseAddress.AppendPathSegment(_authorizationService.AuthPath.Register).PostJsonAsync(request);
        }

        public async Task<TokenResponse> Login(LoginRequest request)
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(_authorizationService.BaseAddress);

            if (disco.IsError) throw new Exception("Failed");

            var userLoginRequest = new PasswordTokenRequest()
            {
                Address = disco.TokenEndpoint,
                ClientId = "client.user",
                ClientSecret = "secret",
                GrantType = OidcConstants.GrantTypes.Password,
                Scope = "BankingService.UserActions openid profile",

                UserName = request.Email,
                Password = request.Password,
            };

            var response = await client.RequestPasswordTokenAsync(userLoginRequest);
            if(response.IsError) throw new Exception("Failed");

            return response;
        }
    }
}
