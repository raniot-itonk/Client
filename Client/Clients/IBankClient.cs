using System;
using System.Threading.Tasks;
using Client.Models.Requests.BankService;
using Client.Models.Responses.BankService;

namespace Client.Clients
{
    public interface IBankClient
    {
        Task CreateAccount(CreateAccountRequest request, string jwtToken);
        Task Deposit(DepositRequest request, Guid id, string jwtToken);
        Task<GetAccountResponse> GetAccount(GetAccountRequest request, string jwtToken);
    }
}