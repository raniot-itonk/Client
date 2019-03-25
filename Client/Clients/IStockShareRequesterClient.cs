using System.Threading.Tasks;
using Client.Models.Requests.StockShareRequester;

namespace Client.Clients
{
    public interface IStockShareRequesterClient
    {
        Task PlaceBid(PlaceBidRequest placeBidRequest, string jwtToken);
    }
}