using System;
using System.Threading.Tasks;
using Client.Helpers;
using Client.Models.Requests.BankService;
using Client.Models.Responses.BankService;
using Client.OptionModels;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace Client.Clients
{
    public class BankClient : IBankClient
    {
        private readonly BankService _bankService;

        public BankClient(IOptionsMonitor<Services> serviceOption)
        {
            _bankService = serviceOption.CurrentValue.BankService ??
                           throw new ArgumentNullException(nameof(serviceOption.CurrentValue.BankService));
        }

        public async Task CreateAccount(CreateAccountRequest request, string jwtToken)
        {
            await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _bankService.BaseAddress.AppendPathSegment(_bankService.BankPath.Account)
                    .WithOAuthBearerToken(jwtToken).PostJsonAsync(request));
        }

        public async Task<GetAccountResponse> GetAccount(GetAccountRequest request, string jwtToken)
        {
            return await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _bankService.BaseAddress.AppendPathSegments(_bankService.BankPath.Account, request.Id)
                    .WithOAuthBearerToken(jwtToken).GetJsonAsync<GetAccountResponse>());
        }

        public async Task Deposit(DepositRequest request, Guid id, string jwtToken)
        {
            await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _bankService.BaseAddress.AppendPathSegments(_bankService.BankPath.Account, $"{id}/balance")
                    .WithOAuthBearerToken(jwtToken).PutJsonAsync(request));
        }
    }
}
