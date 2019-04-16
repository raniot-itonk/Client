using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Helpers;
using Client.Models;
using Client.OptionModels;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace Client.Clients
{
    public interface IStockTraderBrokerClient
    {
        Task<List<BuyRequestModel>> GetBuyRequests(Guid id, string jwtToken);
        Task<List<BuyRequestModel>> GetSellRequests(Guid id, string jwtToken);
    }

    public class StockTraderBrokerClient : IStockTraderBrokerClient
    {
        private readonly StockShareRequester.OptionModels.StockTraderBroker _stockTraderBroker;

        public StockTraderBrokerClient(IOptionsMonitor<Services> serviceOption)
        {
            _stockTraderBroker = serviceOption.CurrentValue.StockTraderBroker ??
                           throw new ArgumentNullException(nameof(serviceOption.CurrentValue.StockTraderBroker));
        }
        public async Task<List<BuyRequestModel>> GetBuyRequests(Guid id, string jwtToken)
        {
            return await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _stockTraderBroker.BaseAddress
                    .AppendPathSegment(_stockTraderBroker.StockTraderBrokerPath.BuyRequest).SetQueryParam("ownerId", id)
                    .WithOAuthBearerToken(jwtToken).GetJsonAsync<List<BuyRequestModel>>());
        }
        public async Task<List<BuyRequestModel>> GetSellRequests(Guid id, string jwtToken)
        {
            return await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _stockTraderBroker.BaseAddress
                    .AppendPathSegment(_stockTraderBroker.StockTraderBrokerPath.SellRequest).SetQueryParam("ownerId", id)
                    .WithOAuthBearerToken(jwtToken).GetJsonAsync<List<BuyRequestModel>>());
        }
    }
}
