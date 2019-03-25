using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Helpers;
using Client.Models.Requests.PublicShareOwnerControl;
using Client.Models.Responses.PublicShareOwnerControl;
using Client.OptionModels;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace Client.Clients
{
    public class PublicShareOwnerControlClient : IPublicShareOwnerControlClient
    {
        private readonly PublicShareOwnerControl _publicShareOwnerControl;

        public PublicShareOwnerControlClient(IOptionsMonitor<Services> serviceOption)
        {
            _publicShareOwnerControl = serviceOption.CurrentValue.PublicShareOwnerControl ??
                           throw new ArgumentNullException(nameof(serviceOption.CurrentValue.PublicShareOwnerControl));
        }

        public async Task<List<StockResponse>> GetAllStocks()
        {
            return await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _publicShareOwnerControl.BaseAddress.AppendPathSegment(_publicShareOwnerControl.PublicSharePath.Stock)
                    .GetJsonAsync< List<StockResponse>>());
        }

        public async Task<List<StockResponse>> GetAllOwnedStocks(Guid id, string jwtToken)
        {
            return await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _publicShareOwnerControl.BaseAddress.AppendPathSegment(_publicShareOwnerControl.PublicSharePath.Stock)
                    .WithOAuthBearerToken(jwtToken).SetQueryParam("userIdGuid", id).GetJsonAsync<List<StockResponse>>());
        }

        public async Task<List<StockWithOwnerInfoResponse>> GetStockWithOwnerInfo(Guid id, string jwtToken)
        {
            return await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _publicShareOwnerControl.BaseAddress.AppendPathSegment(_publicShareOwnerControl.PublicSharePath.Stock)
                    .WithOAuthBearerToken(jwtToken).SetQueryParam("ownerId", id).GetJsonAsync<List<StockWithOwnerInfoResponse>>());
        }

        public async Task<StockResponse> PostStock(StockRequest stockRequest, string jwtToken)
        {
            return await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _publicShareOwnerControl.BaseAddress.AppendPathSegment(_publicShareOwnerControl.PublicSharePath.Stock)
                    .PostJsonAsync(stockRequest).ReceiveJson<StockResponse>());
        }

        public async Task IssueShares(IssueSharesRequest issueSharesRequest, long id, string jwtToken)
        {
            await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _publicShareOwnerControl.BaseAddress.AppendPathSegments(_publicShareOwnerControl.PublicSharePath.Stock, id, "Issue")
                    .PutJsonAsync(issueSharesRequest));
        }
    }
}
