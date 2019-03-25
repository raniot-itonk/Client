﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client.Clients;
using Client.Helpers;
using Client.Models;
using Client.Models.Requests.PublicShareOwnerControl;
using Client.Models.Responses.PublicShareOwnerControl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Client.Controllers
{
    public class BusinessController : Controller
    {
        private readonly IBankClient _bankClient;
        private readonly IPublicShareOwnerControlClient _publicShareOwnerControlClient;
        private readonly ILogger<StockController> _logger;

        public BusinessController(IBankClient bankClient, IPublicShareOwnerControlClient publicShareOwnerControlClient, ILogger<StockController> logger)
        {
            _bankClient = bankClient;
            _publicShareOwnerControlClient = publicShareOwnerControlClient;
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
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,AmountOfShares")] StockViewModel stockViewModel)
        {
            var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);
            var stockRequest = new StockRequest
            {
                Name = stockViewModel.Name,
                Shares = new List<Shareholder>
                {
                    new Shareholder
                    {
                        Amount = stockViewModel.AmountOfShares,
                        StockholderId = id
                    }
                },
                StockOwner = id
            };

            await _publicShareOwnerControlClient.PostStock(stockRequest, "jwtToken");
            return await List();
        }

        public IActionResult IssueMore(long id)
        {
            return View(new IssueMoreViewModel { Id = id, Amount = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> IssueMore([Bind("Id,Amount")] IssueMoreViewModel issueMoreViewModel)
        {
            var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);
            await _publicShareOwnerControlClient.IssueShares(new IssueSharesRequest
                {
                    Amount = issueMoreViewModel.Amount,
                    Owner = id
                },
                issueMoreViewModel.Id, jwtToken);

            return await List();
        }
    }
}