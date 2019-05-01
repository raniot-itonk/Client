using System;
using System.IdentityModel.Tokens.Jwt;
using Client.Models;
using Microsoft.AspNetCore.Http;

namespace Client.Helpers
{
    public class JwtHelper
    {
        public static (string, Guid) GetJwtAndIdFromJwt(HttpRequest request)
        {
            request.Cookies.TryGetValue("jwtCookie", out var jwtToken);
            Console.WriteLine("My JwtToken : " + jwtToken);
            var id = GetIdFromToken(jwtToken);
            return (jwtToken, id);
        }

        public static LoginViewModel LoggedIn(HttpRequest request)
        {
            var loginViewModel = new LoginViewModel();
            var tryGetValue = request.Cookies.TryGetValue("jwtCookie", out var jwtToken);
            loginViewModel.LoggedIn = tryGetValue;

            return loginViewModel;
        }

        public static Guid GetIdFromToken(string jwtToken)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(jwtToken);
                var idFromJwt = Guid.Parse(jwtSecurityToken.Subject);
                return idFromJwt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
