using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Client.Authorization
{
    public class HasScopeHandler : AuthorizationHandler<UserHasRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserHasRequirement requirement)
        {
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == requirement.Issuer))
                return Task.CompletedTask;

            // Split the scopes string into an array
            var scopes = context.User.FindAll(c => c.Type == "scope" && c.Issuer == requirement.Issuer).ToList();

            // Succeed if the scope array contains the required scope
            if (!string.IsNullOrEmpty(scopes.Find(s => s.Value == requirement.Scope)?.Value))
                context.Succeed(requirement);

            var claim = context.User.Claims.FirstOrDefault(c => c.Type == "StockProvider");

            if (claim != null && (!string.IsNullOrEmpty(requirement.Claim) && claim.Value.Equals("True")))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
