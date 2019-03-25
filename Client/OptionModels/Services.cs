namespace Client.OptionModels
{
    public class Services
    {
        public BankService BankService { get; set; }
        public AuthorizationService AuthorizationService { get; set; }
        public PublicShareOwnerControl PublicShareOwnerControl { get; set; }
        public StockShareRequester StockShareRequester { get; set; }
        public StockShareProvider StockShareProvider { get; set; }
    }

    public class StockShareProvider
    {
        public string BaseAddress { get; set; }
        public StockShareProviderPath StockShareProviderPath { get; set; }
    }

    public class StockShareProviderPath
    {
        public string StockSell { get; set; }
    }

    public class StockShareRequester
    {
        public string BaseAddress { get; set; }
        public StockShareRequesterPath PublicSharePath { get; set; }
    }

    public class StockShareRequesterPath
    {
        public string StockBid { get; set; }
    }
}
