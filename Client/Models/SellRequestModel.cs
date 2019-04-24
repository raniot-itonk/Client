using System;

namespace Client.Models
{
    public class SellRequestModel
    {
        public long Id { get; set; }
        public Guid AccountId { get; set; }
        public long StockId { get; set; }
        public double Price { get; set; }
        public DateTime TimeOut { get; set; }
        public int AmountOfShares { get; set; }
    }
}