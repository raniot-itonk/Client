namespace Client.Models.Responses.PublicShareOwnerControl
{
    public class StockWithOwnerInfoResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double LastTradedValue { get; set; }
        public int TotalShares { get; set; }
    }
}

