using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend_Bank.Token
{
    public class TokenManager
    {
        public static string GetAccessToken(IEnumerable<Claim> claims)
        {
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: claims.Append(new Claim(ClaimsIdentity.DefaultRoleClaimType, "access")),
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFE_ACCESS)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        public static string GetRefreshToken(IEnumerable<Claim> claims)
        {
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: claims.Append(new Claim(ClaimsIdentity.DefaultRoleClaimType, "refresh")),
                    expires: now.Add(TimeSpan.FromDays(AuthOptions.LIFE_REFRESH)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public static Dictionary<string, string> Tokens(IEnumerable<Claim> claims)
        {
            return new Dictionary<string, string>()
            {
                { "access_token",  GetAccessToken(claims) },
                { "refresh_token", GetRefreshToken(claims) }
            };
        }

        public static ClaimsIdentity GetIdentity(Organisation? organisation)
        {
            if (organisation == default)
                return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, organisation.Login)
            };
            return new ClaimsIdentity(claims);
        }

        public static ClaimsIdentity GetIdentity(UserModel? user)
        {
            if (user == default)
                return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login)
            };
            return new ClaimsIdentity(claims);
        }
    }
}
