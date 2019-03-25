using System;

namespace Client.Models.Responses.PublicShareOwnerControl
{
    public class Shareholder
    {
        public Guid StockholderId { get; set; }
        public int Amount { get; set; }
    }
}