using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Helpers;
using Client.Models;
using Client.Models.Requests.BankService;
using Client.Models.Responses.BankService;
using Client.OptionModels;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace Client.Clients
{
    public class HistoryClient : IHistoryClient
    {
        private readonly HistoryService _historyService;

        public HistoryClient(IOptionsMonitor<Services> serviceOption)
        {
            _historyService = serviceOption.CurrentValue.HistoryService ??
                           throw new ArgumentNullException(nameof(serviceOption.CurrentValue.HistoryService));
        }

        public async Task<List<HistoryResponse>> GetHistory(Guid id, string jwtToken)
        {
            return await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _historyService.BaseAddress.AppendPathSegment(_historyService.HistoryPath.History).SetQueryParam("user", id)
                    .WithOAuthBearerToken(jwtToken).GetJsonAsync<List<HistoryResponse>>());
        }
    }
}
