using System.Threading.Tasks;
using Client.Clients;
using Client.Helpers;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Client.Controllers
{
    public class StockRequestController : Controller
    {
        private readonly IStockTraderBrokerClient _stockTraderBrokerClient;
        private readonly IStockShareProviderClient _stockShareProviderClient;
        private readonly IStockShareRequesterClient _stockShareRequesterClient;
        private readonly ILogger<StockController> _logger;

        public StockRequestController(IStockTraderBrokerClient stockTraderBrokerClient,IStockShareProviderClient stockShareProviderClient, IStockShareRequesterClient stockShareRequesterClient , ILogger<StockController> logger)
        {
            _stockTraderBrokerClient = stockTraderBrokerClient;
            _stockShareProviderClient = stockShareProviderClient;
            _stockShareRequesterClient = stockShareRequesterClient;

            _logger = logger;
        }
        public async Task<ViewResult> SellRequests()
        {
            var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);
            var sellRequestModels = await _stockTraderBrokerClient.GetSellRequests(id, jwtToken);
            return View("SellRequests", sellRequestModels);
        }

        public async Task<ViewResult> RemoveSellRequest(long requestId)
        {
            var (jwtToken, id) = JwtHelper.GetJwtAndIdFromJwt(Request);
            var validationResult = await _stockShareRequesterClient.RemoveBid(requestId, jwtToken);
            if (validationResult.Valid) return await SellRequests();

            ViewBag.ShowErrorDialog = true;
            ViewBag.ErrorText = validationResult.ErrorMessage;
            return await SellRequests();
        }
    }
}