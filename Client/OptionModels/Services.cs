﻿namespace Client.OptionModels
{
    public class Services
    {
        public BankService BankService { get; set; }
        public AuthorizationService AuthorizationService { get; set; }
        public PublicShareOwnerControl PublicShareOwnerControl { get; set; }
        public StockShareRequester StockShareRequester { get; set; }
        public StockShareProvider StockShareProvider { get; set; }
        public StockTraderBroker StockTraderBroker { get; set; }
        public HistoryService HistoryService { get; set; }
    }
}
