﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Helpers;
using Client.Models.Requests.BankService;
using Client.Models.Responses.BankService;
using Client.Models.Responses.PublicShareOwnerControl;
using Client.OptionModels;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace Client.Clients
{
    public class PublicShareOwnerControlClient : IPublicShareOwnerControlClient
    {
        private readonly PublicShareOwnerControl _publicShareOwnerControl;

        public PublicShareOwnerControlClient(IOptionsMonitor<Services> serviceOption)
        {
            _publicShareOwnerControl = serviceOption.CurrentValue.PublicShareOwnerControl ??
                           throw new ArgumentNullException(nameof(serviceOption.CurrentValue.PublicShareOwnerControl));
        }

        public async Task<List<StockResponse>> GetAllStocks()
        {
            return await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _publicShareOwnerControl.BaseAddress.AppendPathSegment(_publicShareOwnerControl.PublicSharePath.Stock)
                    .GetJsonAsync< List<StockResponse>>());
        }

        public async Task<List<StockResponse>> GetAllOwnedStocks(Guid id, string jwtToken)
        {
            return await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _publicShareOwnerControl.BaseAddress.AppendPathSegment(_publicShareOwnerControl.PublicSharePath.Stock)
                    .WithOAuthBearerToken(jwtToken).SetQueryParam("userIdGuid", id).GetJsonAsync<List<StockResponse>>());
        }
    }
}