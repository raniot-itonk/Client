using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Models;

namespace Client.Clients
{
    public interface IHistoryClient
    {
        Task<List<HistoryResponse>> GetHistory(Guid id, string jwtToken);
    }
}