using System;

namespace Client.Models.Requests.BankService
{
    public class CreateAccountRequest
    {
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; }
        public double Balance { get; set; }
    }
}
