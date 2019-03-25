using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models.Requests.PublicShareOwnerControl
{
    public class IssueSharesRequest
    {
        public int Amount { get; set; }
        public Guid Owner { get; set; }
    }
}
