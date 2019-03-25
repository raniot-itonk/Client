using System.Threading.Tasks;
using Client.Models.Requests.StockShareProvider;

namespace Client.Clients
{
    public interface IStockShareProviderClient
    {
        Task SetSharesForSale(SellRequestRequest sellRequestRequest, string jwtToken);
    }
}