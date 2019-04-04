using System;
using System.Threading.Tasks;
using Client.Helpers;
using Client.Models;
using Client.Models.Requests.StockShareProvider;
using Client.OptionModels;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace Client.Clients
{
    public class StockShareProviderClient : IStockShareProviderClient
    {
        private readonly StockShareProvider _stockShareRequester;

        public StockShareProviderClient(IOptionsMonitor<Services> serviceOption)
        {
            _stockShareRequester = serviceOption.CurrentValue.StockShareProvider ??
                                       throw new ArgumentNullException(nameof(serviceOption.CurrentValue.StockShareProvider));
        }

        public async Task<ValidationResult> SetSharesForSale(SellRequestRequest sellRequestRequest, string jwtToken)
        {
            return await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _stockShareRequester.BaseAddress.AppendPathSegment(_stockShareRequester.StockShareProviderPath.StockSell)
                    .PostJsonAsync(sellRequestRequest).ReceiveJson<ValidationResult>());
        }
    }
}
