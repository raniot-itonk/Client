using System;

namespace Client.Models.Responses.BankService
{
    public class GetAccountResponse
    {
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; }
        public double Balance { get; set; }
    }
}
