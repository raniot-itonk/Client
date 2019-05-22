using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Clients;
using Client.Helpers;
using Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Client.Controllers
{
    public class StockRequestController : Controller
    {
        private readonly IStockTraderBrokerClient _stockTraderBrokerClient;
        private readonly IStockShareProviderClient _stockShareProviderClient;
        private readonly IStockShareRequesterClient _stockShareRequesterClient;
        private readonly IPublicShareOwnerControlClient _publicShareOwnerControlClient;
        private readonly ILogger<StockController> _logger;

        public StockRequestController(IStockTraderBrokerClient stockTraderBrokerClient,
            IStockShareProviderClient stockShareProviderClient, IStockShareRequesterClient stockShareRequesterClient,
            IPublicShareOwnerControlClient publicShareOwnerControlClient, 
            ILogger<StockController> logger)
        {
            _stockTraderBrokerClient = stockTraderBrokerClient;
            _stockShareProviderClient = stockShareProviderClient;
            _stockShareRequesterClient = stockShareRequesterClient;
            _publicShareOwnerControlClient = publicShareOwnerControlClient;

            _logger = logger;
        }

        [Authorize("client.UserActions")]
        public async Task<ViewResult> SellRequests()
        {
            var sellRequestViewModels = new List<GenericRequestViewModel>();
            var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);
            var sellRequestModels = await _stockTraderBrokerClient.GetSellRequests(id, jwtToken);
            foreach (var sellRequestModel in sellRequestModels)
            {
                var stock = await _publicShareOwnerControlClient.GetStock(sellRequestModel.StockId, jwtToken);
                sellRequestViewModels.Add(new GenericRequestViewModel
                {
                    AmountOfShares = sellRequestModel.AmountOfShares,
                    Id = sellRequestModel.Id,
                    TimeOut = sellRequestModel.TimeOut,
                    OfferedPrice = sellRequestModel.Price,
                    LastTradedValue = stock.LastTradedValue,
                    Name = stock.Name
                });
            }
            return View("SellRequests", sellRequestViewModels);
        }

        [Authorize("client.UserActions")]
        public async Task<ViewResult> RemoveSellRequest(long requestId)
        {
            var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);
            var validationResult = await _stockShareProviderClient.RemoveSharesForSale(requestId, jwtToken);
            if (validationResult.Valid) return await SellRequests();

            ViewBag.ShowErrorDialog = true;
            ViewBag.ErrorText = validationResult.ErrorMessage;
            return await SellRequests();
        }

        [Authorize("client.UserActions")]
        public async Task<ViewResult> BuyRequests()
        {
            var buyRequestViewModels = new List<GenericRequestViewModel>();
            var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);
            var buyRequestModels = await _stockTraderBrokerClient.GetBuyRequests(id, jwtToken);
            foreach (var buyRequestModel in buyRequestModels)
            {
                var stock = await _publicShareOwnerControlClient.GetStock(buyRequestModel.StockId, jwtToken);
                buyRequestViewModels.Add(new GenericRequestViewModel
                {
                    AmountOfShares = buyRequestModel.AmountOfShares,
                    Id = buyRequestModel.Id,
                    TimeOut = buyRequestModel.TimeOut,
                    OfferedPrice = buyRequestModel.Price,
                    LastTradedValue = stock.LastTradedValue,
                    Name = stock.Name
                });
            }
            return View("BuyRequests", buyRequestViewModels);
        }

        [Authorize("client.UserActions")]
        public async Task<ViewResult> RemoveBuyRequest(long requestId)
        {
            var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);
            var validationResult = await _stockShareRequesterClient.RemoveBid(requestId, jwtToken);
            if (validationResult.Valid) return await BuyRequests();

            ViewBag.ShowErrorDialog = true;
            ViewBag.ErrorText = validationResult.ErrorMessage;
            return await BuyRequests();
        }
    }
}