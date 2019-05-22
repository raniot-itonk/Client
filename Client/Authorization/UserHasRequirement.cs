using System;
using Microsoft.AspNetCore.Authorization;

namespace Client.Authorization
{
    public class UserHasRequirement : IAuthorizationRequirement
    {
        public string Issuer { get; }
        public string Scope { get; }
        public string Claim { get; set; }

        public UserHasRequirement(string scope, string issuer)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
        }

        public UserHasRequirement(string scope, string issuer, string claim)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
            Claim = claim?? throw new ArgumentNullException(nameof(claim));
        }
    }
}
