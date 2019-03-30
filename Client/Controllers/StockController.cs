using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Client.Clients;
using Client.Helpers;
using Client.Models;
using Client.Models.Requests.BankService;
using Client.Models.Requests.StockShareProvider;
using Client.Models.Requests.StockShareRequester;
using Client.Models.Responses.PublicShareOwnerControl;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Client.Controllers
{
    public class StockController : Controller
    {
        private readonly IBankClient _bankClient;
        private readonly IPublicShareOwnerControlClient _publicShareOwnerControlClient;
        private readonly IStockShareRequesterClient _stockShareRequesterClient;
        private readonly IStockShareProviderClient _stockShareProviderClient;
        private readonly ILogger<StockController> _logger;

        public StockController(IBankClient bankClient, IPublicShareOwnerControlClient publicShareOwnerControlClient,
            IStockShareRequesterClient stockShareRequesterClient, IStockShareProviderClient stockShareProviderClient,
            ILogger<StockController> logger)
        {
            _bankClient = bankClient;
            _publicShareOwnerControlClient = publicShareOwnerControlClient;
            _stockShareRequesterClient = stockShareRequesterClient;
            _stockShareProviderClient = stockShareProviderClient;
            _logger = logger;
        }

        public async Task<ViewResult> Index()
        {
            try
            {

                var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);

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
            var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);
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
                var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);
                var allOwnedStocks = await _publicShareOwnerControlClient.GetAllOwnedStocks(id, jwtToken);
                var ownedStockViewModels = OwnedStockViewModel.FromStockResponseList(allOwnedStocks);
                return View(ownedStockViewModels);
            }
            catch (FlurlHttpException e)
            {
                if (e.Call.HttpStatus == HttpStatusCode.NotFound)
                    return View(null);
                _logger.LogError(e, "Failed to get Owned Stocks");
                throw;
            }
        }

        public IActionResult Buy(long id)
        {
            var buyStockViewModel = new BuyStockViewModel
            {
                Id = id,
                AmountOfShares = 0,
                TimeOut = DateTime.Today.AddDays(1),
                Price = 0
            };
            return View(buyStockViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Buy([Bind("Id,TimeOut,AmountOfShares,Price")] BuyStockViewModel createStockViewModel)
        {
            var placeBidRequest = new PlaceBidRequest
            {
                AmountOfShares = createStockViewModel.AmountOfShares,
                TimeOut = createStockViewModel.TimeOut,
                Price = createStockViewModel.Price,
                StockId = createStockViewModel.Id
            };
            var (jwtToken, _) = JwtHelper.GetJwtAndIdFromJwt(Request);
            await _stockShareRequesterClient.PlaceBid(placeBidRequest, jwtToken);
            return await Index();
        }

        public IActionResult Sell(long id)
        {
            var sellStockViewModel = new SellStockViewModel
            {
                Id = id,
                AmountOfShares = 0,
                TimeOut = DateTime.Today.AddDays(1),
                Price = 0
            };
            return View(sellStockViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Sell([Bind("Id,TimeOut,AmountOfShares,Price")] SellStockViewModel sellStockViewModel)
        {
            var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);
            var sellRequestRequest = new SellRequestRequest
            {
                AccountId = id,
                AmountOfShares = sellStockViewModel.AmountOfShares,
                TimeOut = sellStockViewModel.TimeOut,
                Price = sellStockViewModel.Price,
                StockId = sellStockViewModel.Id
            };
            await _stockShareProviderClient.SetSharesForSale(sellRequestRequest, jwtToken);
            return await Index();
        }
    }

    public class OwnedStockViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double LastTradedValue { get; set; }
        public int Amount { get; set; }



        public static List<OwnedStockViewModel> FromStockResponseList(List<StockResponse> stockResponses)
        {
            var ownedStockViewModels = new List<OwnedStockViewModel>();
            foreach (var stockResponse in stockResponses)
            {
                ownedStockViewModels.Add(new OwnedStockViewModel
                {
                    Amount = stockResponse.ShareHolders.First().Amount,
                    Id = stockResponse.Id,
                    LastTradedValue = stockResponse.LastTradedValue,
                    Name = stockResponse.Name
                });
            }
        }
    }
}