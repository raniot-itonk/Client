using System;

namespace Client.Models
{
    public class CreateStockViewModel
    {
        public string Name { get; set; }
        public DateTime TimeOut { get; set; }
        public int AmountOfShares { get; set; }
        public double Price { get; set; }
    }
}
