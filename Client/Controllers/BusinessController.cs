using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Clients;
using Client.Helpers;
using Client.Models;
using Client.Models.Requests.PublicShareOwnerControl;
using Client.Models.Requests.StockShareProvider;
using Client.Models.Responses.PublicShareOwnerControl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Client.Controllers
{
    public class BusinessController : Controller
    {
        private readonly IBankClient _bankClient;
        private readonly IPublicShareOwnerControlClient _publicShareOwnerControlClient;
        private readonly IStockShareProviderClient _stockShareProviderClient;
        private readonly ILogger<StockController> _logger;

        public BusinessController(IBankClient bankClient, IPublicShareOwnerControlClient publicShareOwnerControlClient, IStockShareProviderClient stockShareProviderClient , ILogger<StockController> logger)
        {
            _bankClient = bankClient;
            _publicShareOwnerControlClient = publicShareOwnerControlClient;
            _stockShareProviderClient = stockShareProviderClient;
            _logger = logger;
        }

        public async Task<IActionResult> List()
        {
            var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);
            var stocksWithOwnerInfo = await _publicShareOwnerControlClient.GetStockWithOwnerInfo(id, jwtToken);
            return View("List", stocksWithOwnerInfo);
        }


        public IActionResult Create()
        {
            var createStockViewModel = new CreateStockViewModel
            {
                AmountOfShares = 1,
                TimeOut = DateTime.Today.AddDays(1),
                Price = 1
            };
            return View(createStockViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,Price,TimeOut,AmountOfShares")] CreateStockViewModel createStockViewModel)
        {
            var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);
            var stockRequest = new StockRequest
            {
                Name = createStockViewModel.Name,
                Shares = new List<Shareholder>
                {
                    new Shareholder
                    {
                        Amount = createStockViewModel.AmountOfShares,
                        ShareholderId = id
                    }
                },
                StockOwner = id
            };

            var stockResponse = await _publicShareOwnerControlClient.PostStock(stockRequest, jwtToken);
            var sellRequestRequest = new SellRequestRequest
            {
                AmountOfShares = createStockViewModel.AmountOfShares,
                AccountId = id,
                Price = createStockViewModel.Price,
                StockId = stockResponse.Id,
                TimeOut = createStockViewModel.TimeOut

            };
            await _stockShareProviderClient.SetSharesForSale(sellRequestRequest, jwtToken);
            return await List();
        }

        public IActionResult IssueMore(long id)
        {
            var issueMoreViewModel = new IssueMoreViewModel
            {
                Id = id,
                Amount = 1,
                TimeOut = DateTime.Today.AddDays(1),
                Price = 1

            };
            return View(issueMoreViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> IssueMore([Bind("Id,Price,Amount")] IssueMoreViewModel issueMoreViewModel)
        {
            var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);
            await _publicShareOwnerControlClient.IssueShares(new IssueSharesRequest
                {
                    Amount = issueMoreViewModel.Amount,
                    Owner = id
                },
                issueMoreViewModel.Id, jwtToken);

            var sellRequestRequest = new SellRequestRequest
            {
                AmountOfShares = issueMoreViewModel.Amount,
                AccountId = id,
                Price = issueMoreViewModel.Price,
                StockId = issueMoreViewModel.Id,
                TimeOut = issueMoreViewModel.TimeOut

            };
            await _stockShareProviderClient.SetSharesForSale(sellRequestRequest, jwtToken);

            return await List();
        }
    }
}