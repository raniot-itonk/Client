using System.Threading.Tasks;
using Client.Models;
using Client.Models.Requests.StockShareProvider;

namespace Client.Clients
{
    public interface IStockShareProviderClient
    {
        Task<ValidationResult> SetSharesForSale(SellRequestRequest sellRequestRequest, string jwtToken);
        Task<ValidationResult> RemoveSharesForSale(long id, string jwtToken);
    }
}