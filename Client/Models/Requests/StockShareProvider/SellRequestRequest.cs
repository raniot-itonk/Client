using System;

namespace Client.Clients
{
    public class PlaceBidRequest
    {
        public long StockId { get; set; }
        public int AmountOfShares { get; set; }
        public double Price { get; set; }
        public DateTime TimeOut { get; set; }
    }
}