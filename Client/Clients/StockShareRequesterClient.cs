﻿using System;
using System.Threading.Tasks;
using Client.Helpers;
using Client.Models.Requests.StockShareRequester;
using Client.OptionModels;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace Client.Clients
{
    public class StockShareRequesterClient : IStockShareRequesterClient
    {
        private readonly StockShareRequester _stockShareRequester;

        public StockShareRequesterClient(IOptionsMonitor<Services> serviceOption)
        {
            _stockShareRequester = serviceOption.CurrentValue.StockShareRequester ??
                                       throw new ArgumentNullException(nameof(serviceOption.CurrentValue.StockShareRequester));
        }

        public async Task PlaceBid(PlaceBidRequest placeBidRequest, string jwtToken)
        {
            await PolicyHelper.ThreeRetriesAsync().ExecuteAsync(() =>
                _stockShareRequester.BaseAddress.AppendPathSegment(_stockShareRequester.StockShareRequesterPath.StockBid)
                    .PostJsonAsync(placeBidRequest));
        }
    }
}