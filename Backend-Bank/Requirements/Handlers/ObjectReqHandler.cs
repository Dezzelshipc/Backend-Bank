using Backend_Bank.Converters;
using Microsoft.AspNetCore.Authorization;

namespace Backend_Bank.Requirements.Handlers
{
    public class ObjectReqHandler : AuthorizationHandler<ObjectRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ObjectRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == "Type"))
            {
                if (context.User.FindFirst(c => c.Type == "Type")!.Value.ToObjectType() == requirement.Type)
                    context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
