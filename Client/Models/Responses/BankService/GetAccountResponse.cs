using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models.Responses.BankService
{
    public class GetAccountResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
    }
}
