﻿using System;

namespace Client.Models
{
    public class BuyStockViewModel
    {
        public long Id { get; set; }
        public DateTime TimeOut { get; set; }
        public int AmountOfShares { get; set; }
        public double Price { get; set; }
    }
}
