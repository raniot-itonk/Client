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
        private readonly IStockTraderBrokerClient _stockTraderBrokerClient;
        private readonly ILogger<StockController> _logger;

        public StockController(IBankClient bankClient, IPublicShareOwnerControlClient publicShareOwnerControlClient,
            IStockShareRequesterClient stockShareRequesterClient, IStockShareProviderClient stockShareProviderClient,
            IStockTraderBrokerClient stockTraderBrokerClient,
            ILogger<StockController> logger)
        {
            _bankClient = bankClient;
            _publicShareOwnerControlClient = publicShareOwnerControlClient;
            _stockShareRequesterClient = stockShareRequesterClient;
            _stockShareProviderClient = stockShareProviderClient;
            _stockTraderBrokerClient = stockTraderBrokerClient;
            _logger = logger;
        }

        public async Task<ViewResult> StockList()
        {
            try
            {
                var allStocks = await _publicShareOwnerControlClient.GetAllStocks();
                return View("StockList" ,allStocks);
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
                var ownedStockViewModels = OwnedStockViewModel.FromStockResponseList(allOwnedStocks, id);
                return View("OwnedStockList" ,ownedStockViewModels);
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

        public IActionResult Buy(long id, int amountOfShares, double price, DateTime timeOut)
        {
            var buyStockViewModel = new BuyStockViewModel
            {
                Id = id,
                AmountOfShares = amountOfShares,
                TimeOut = timeOut,
                Price = price
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
            var validationResult = await _stockShareRequesterClient.PlaceBid(placeBidRequest, jwtToken);
            if (validationResult.Valid) return await StockList();
            ViewBag.ShowErrorDialog = true;
            ViewBag.ErrorText = validationResult.ErrorMessage;
            return View(createStockViewModel);
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
            var validationResult = await _stockShareProviderClient.SetSharesForSale(sellRequestRequest, jwtToken);

            if(validationResult.Valid) return await OwnedStockList();
            ViewBag.ShowErrorDialog = true;
            ViewBag.ErrorText = validationResult.ErrorMessage;
            return View(sellStockViewModel);
        }

        public async Task<IActionResult> Details(long id, string name)
        {
            var (jwtToken, _) = JwtHelper.GetJwtAndIdFromJwt(Request);
            var sellRequestsForSpecificStock = await _stockTraderBrokerClient.GetSellRequestsForSpecificStock(id, jwtToken);
            var detailsBuySharesViewModel = new DetailsBuySharesViewModel { Name = name, SellRequestModels = sellRequestsForSpecificStock };
            return View(detailsBuySharesViewModel);
        }
    }

    public class DetailsBuySharesViewModel
    {
        public IEnumerable<SellRequestModel> SellRequestModels { get; set; }
        public string Name { get; set; }
    }

    public class OwnedStockViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double LastTradedValue { get; set; }
        public int Amount { get; set; }



        public static List<OwnedStockViewModel> FromStockResponseList(List<StockResponse> stockResponses, Guid id)
        {
            var ownedStockViewModels = new List<OwnedStockViewModel>();
            foreach (var stockResponse in stockResponses)
            {
                ownedStockViewModels.Add(new OwnedStockViewModel
                {
                    Amount = stockResponse.ShareHolders.Where(shareholder => shareholder.ShareholderId == id).Sum(shareholder => shareholder.Amount),
                    Id = stockResponse.Id,
                    LastTradedValue = stockResponse.LastTradedValue,
                    Name = stockResponse.Name
                });
            }

            return ownedStockViewModels;
        }
    }
}