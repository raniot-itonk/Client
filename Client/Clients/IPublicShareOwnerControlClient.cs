using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Models.Requests.PublicShareOwnerControl;
using Client.Models.Responses.PublicShareOwnerControl;

namespace Client.Clients
{
    public interface IPublicShareOwnerControlClient
    {
        Task<List<StockResponse>> GetAllOwnedStocks(Guid id, string jwtToken);
        Task<List<StockResponse>> GetAllStocks();
        Task<List<StockWithOwnerInfoResponse>> GetStockWithOwnerInfo(Guid id, string jwtToken);
        Task IssueShares(IssueSharesRequest issueSharesRequest, long id, string jwtToken);
        Task PostStock(StockRequest stockRequest, string jwtToken);
    }
}