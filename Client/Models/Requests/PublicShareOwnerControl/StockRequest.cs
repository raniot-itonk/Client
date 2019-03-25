using System;
using System.Collections.Generic;
using Client.Models.Responses.PublicShareOwnerControl;

namespace Client.Models.Requests.PublicShareOwnerControl
{
    public class StockRequest
    {
        public string Name { get; set; }
        public Guid StockOwner { get; set; }
        public List<Shareholder> Shares { get; set; }
    }
}
