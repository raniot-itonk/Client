using System.Collections.Generic;

namespace Client.Models.Responses.PublicShareOwnerControl
{
    public class StockResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double LastTradedValue { get; set; }
        public List<Shareholder> ShareHolders { get; set; }
    }
}

