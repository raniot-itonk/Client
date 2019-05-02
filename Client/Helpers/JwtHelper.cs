using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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

            if (tryGetValue)
            {
                loginViewModel.IsStockProvider = GetIsStockProviderFromToken(jwtToken);
                loginViewModel.FirstName = GetFirstNameFromToken(jwtToken);
            }

            return loginViewModel;
        }

        private static string GetFirstNameFromToken(string jwtToken)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(jwtToken);
                var firstNameFromJwt = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type.Equals("First Name"))?.Value;
                return firstNameFromJwt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static bool GetIsStockProviderFromToken(string jwtToken)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(jwtToken);
                var stockProviderFromJwt = bool.Parse(jwtSecurityToken.Claims.FirstOrDefault(x => x.Type.Equals("StockProvider"))?.Value);
                return stockProviderFromJwt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
