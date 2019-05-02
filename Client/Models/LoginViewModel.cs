using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class LoginViewModel
    {
        public string FirstName { get; set; }
        public bool LoggedIn { get; set; }
        public bool IsStockProvider { get; set; }
    }
}
