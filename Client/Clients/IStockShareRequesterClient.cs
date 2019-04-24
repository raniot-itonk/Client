using System.Threading.Tasks;
using Client.Models;
using Client.Models.Requests.StockShareRequester;

namespace Client.Clients
{
    public interface IStockShareRequesterClient
    {
        Task<ValidationResult> PlaceBid(PlaceBidRequest placeBidRequest, string jwtToken);
        Task<ValidationResult> RemoveBid(long id, string jwtToken);
    }
}