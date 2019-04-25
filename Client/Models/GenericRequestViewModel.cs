using System;

namespace Client.Models
{
    public class GenericRequestViewModel
    {
        public long Id { get; set; }
        public double OfferedPrice { get; set; }
        public DateTime TimeOut { get; set; }
        public int AmountOfShares { get; set; }
        public string Name { get; set; }
        public double LastTradedValue { get; set; }
    }
}