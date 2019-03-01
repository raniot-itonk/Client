using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Models.Responses.PublicShareOwnerControl;

namespace Client.Clients
{
    public interface IPublicShareOwnerControlClient
    {
        Task<List<StockResponse>> GetAllOwnedStocks(Guid id, string jwtToken);
        Task<List<StockResponse>> GetAllStocks();
    }
}