using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Client.Clients;
using Client.Models.Requests.BankService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Client.Controllers
{
    public class StockController : Controller
    {
        private readonly IBankClient _bankClient;
        private readonly ILogger<StockController> _logger;

        public StockController(IBankClient bankClient, ILogger<StockController> logger)
        {
            _bankClient = bankClient;
            _logger = logger;
        }

        public async Task<ViewResult> Index()
        {
            try
            {
                var (jwtToken, id) = GetJwtAndIdFromJwt();
                var getAccountRequest = new GetAccountRequest { Id = id };
                var account = await _bankClient.GetAccount(getAccountRequest, jwtToken);

                return View("Index",account);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Redirected person to login screen");
                return View("_LoginPartial");
            }
        }

        private (string,Guid) GetJwtAndIdFromJwt()
        {
            Request.Cookies.TryGetValue("jwtCookie", out var jwtToken);
            var id = GetIdFromToken(jwtToken);
            return (jwtToken, id);
        }

        private Guid GetIdFromToken(string jwtToken)
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

        public async Task<ViewResult> AddBalance()
        {
            var (jwtToken, id) = GetJwtAndIdFromJwt();
            var depositRequest = new DepositRequest{Amount = 1000};
            await _bankClient.Deposit(depositRequest, id, jwtToken);
            return await Index();
        }
    }
}