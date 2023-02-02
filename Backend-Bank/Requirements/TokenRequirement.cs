using Database.Models;
using Microsoft.AspNetCore.Authorization;

namespace Backend_Bank.Requirements
{
    public class TokenRequirement : IAuthorizationRequirement
    {
        public TokenRequirement(string token)
        {
            Token = token;
        }

        protected internal string Token { get; set; }
    }
}
