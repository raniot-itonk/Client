using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Threading.Tasks;
using Client.Clients;
using Client.Models.Requests.BankService;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Client.Controllers
{
    public class StockController : Controller
    {
        private readonly IBankClient _bankClient;
        private readonly IPublicShareOwnerControlClient _publicShareOwnerControlClient;
        private readonly ILogger<StockController> _logger;

        public StockController(IBankClient bankClient, IPublicShareOwnerControlClient publicShareOwnerControlClient , ILogger<StockController> logger)
        {
            _bankClient = bankClient;
            _publicShareOwnerControlClient = publicShareOwnerControlClient;
            _logger = logger;
        }

        public async Task<ViewResult> Index()
        {
            try
            {
                var (jwtToken, id) = GetJwtAndIdFromJwt();
                var getAccountRequest = new GetAccountRequest { Id = id };
                var account = await _bankClient.GetAccount(getAccountRequest, jwtToken);

                return View("Index", account);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Redirected person to login screen");
                return View("_LoginPartial");
            }
        }

        public async Task<ViewResult> AddBalance()
        {
            var (jwtToken, id) = GetJwtAndIdFromJwt();
            var depositRequest = new DepositRequest{Amount = 1000};
            await _bankClient.Deposit(depositRequest, id, jwtToken);
            return await Index();
        }

        public async Task<ViewResult> StockList()
        {
            try
            {
                var allStocks = await _publicShareOwnerControlClient.GetAllStocks();
                return View(allStocks);
            }
            catch (FlurlHttpException e)
            {
                if (e.Call.HttpStatus == HttpStatusCode.NotFound)
                    return View(null);
                _logger.LogError(e, "Failed to get all stocks");
                throw;
            }
        }

        public async Task<ViewResult> OwnedStockList()
        {
            try
            {
                var (jwtToken, id) = GetJwtAndIdFromJwt();
                var allOwnedStocks = await _publicShareOwnerControlClient.GetAllOwnedStocks(id, jwtToken);
                return View(allOwnedStocks);
            }
            catch (FlurlHttpException e)
            {
                if (e.Call.HttpStatus == HttpStatusCode.NotFound)
                    return View(null);
                _logger.LogError(e, "Failed to get Owned Stocks");
                throw;
            }
        }

        private (string, Guid) GetJwtAndIdFromJwt()
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
    }
}