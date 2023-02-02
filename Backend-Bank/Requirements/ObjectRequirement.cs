using Database.Models;
using Microsoft.AspNetCore.Authorization;

namespace Backend_Bank.Requirements
{
    public class ObjectRequirement : IAuthorizationRequirement
    {
        public ObjectRequirement(ObjectType type)
        {
            Type = type;
        }

        protected internal ObjectType Type { get; set; }

    }
}
