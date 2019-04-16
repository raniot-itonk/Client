using System;
using System.Threading.Tasks;
using Client.Helpers;
using Client.Models;
using Client.Models.Requests.StockShareRequester;
using Client.OptionModels;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace Client.Clients
{
    public class StockShareRequesterClient : IStockShareRequesterClient
    {
        private readonly StockShareRequester _stockShareRequester;

        public StockShareRequesterClient(IOptionsMonitor<Services> serviceOption)
        {
            _stockShareRequester = serviceOption.CurrentValue.StockShareRequester ??
                                       throw new ArgumentNullException(nameof(serviceOption.CurrentValue.StockShareRequester));
        }

        public async Task<ValidationResult> PlaceBid(PlaceBidRequest placeBidRequest, string jwtToken)
        {
            return await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _stockShareRequester.BaseAddress.AppendPathSegment(_stockShareRequester.StockShareRequesterPath.StockBid)
                    .WithOAuthBearerToken(jwtToken).PostJsonAsync(placeBidRequest).ReceiveJson<ValidationResult>());
        }

        public async Task<ValidationResult> RemoveBid(long id, string jwtToken)
        {
            return await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _stockShareRequester.BaseAddress.AppendPathSegments(_stockShareRequester.StockShareRequesterPath.StockBid, id)
                    .DeleteAsync().ReceiveJson<ValidationResult>());
        }
    }
}
