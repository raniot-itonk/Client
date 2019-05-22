using System;
using System.Threading.Tasks;
using Client.Clients;
using Client.Helpers;
using Client.Models.Requests.BankService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Client.Controllers
{
    public class BalanceController : Controller
    {
        private readonly IBankClient _bankClient;
        private readonly IPublicShareOwnerControlClient _publicShareOwnerControlClient;
        private readonly IStockShareRequesterClient _stockShareRequesterClient;
        private readonly IStockShareProviderClient _stockShareProviderClient;
        private readonly ILogger<BalanceController> _logger;

        public BalanceController(IBankClient bankClient, IPublicShareOwnerControlClient publicShareOwnerControlClient,
            IStockShareRequesterClient stockShareRequesterClient, IStockShareProviderClient stockShareProviderClient,
            ILogger<BalanceController> logger)
        {
            _bankClient = bankClient;
            _publicShareOwnerControlClient = publicShareOwnerControlClient;
            _stockShareRequesterClient = stockShareRequesterClient;
            _stockShareProviderClient = stockShareProviderClient;
            _logger = logger;
        }

        [Authorize("client.UserActions")]
        public async Task<ViewResult> Index()
        {
            var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);

            var getAccountRequest = new GetAccountRequest { Id = id };
            var account = await _bankClient.GetAccount(getAccountRequest, jwtToken);

            return View("Index", account);
        }

        [Authorize("client.UserActions")]
        public async Task<ViewResult> AddBalance()
        {
            var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);
            var depositRequest = new DepositRequest{Amount = 1000};
            await _bankClient.Deposit(depositRequest, id, jwtToken);
            return await Index();
        }
    }
}