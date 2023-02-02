using Backend_Bank.Converters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend_Bank.Requirements.Handlers
{
    public class TokenReqHandler : AuthorizationHandler<TokenRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == "token"))
            {
                if (context.User.FindFirst(c => c.Type == "token")!.Value == requirement.Token)
                    context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
