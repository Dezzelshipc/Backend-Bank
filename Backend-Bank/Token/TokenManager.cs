using Database.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend_Bank.Token
{
    public static class TokenManager
    {
        public static bool CheckClaim(this IEnumerable<Claim> claims, string claim = "token", string value = "access")
        {
            return claims.Any(c => c.Type == claim && c.Value == value);
        }

        public static JwtSecurityToken GetAccessToken(IEnumerable<Claim> claims)
        {
            var now = DateTime.UtcNow;

            claims = ClearClaims(claims);

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: claims.Append(new Claim("token", "access")),
                    expires: now.Add(TimeSpan.FromHours(AuthOptions.LIFE_ACCESS)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return jwt;
        }
        public static JwtSecurityToken GetRefreshToken(IEnumerable<Claim> claims)
        {
            var now = DateTime.UtcNow;

            claims = ClearClaims(claims);

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: claims.Append(new Claim("token", "refresh")),
                    expires: now.Add(TimeSpan.FromDays(AuthOptions.LIFE_REFRESH)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return jwt;
        }

        public static string WriteToken(JwtSecurityToken token)
        {
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static TokenPair<JwtSecurityToken> Tokens(IEnumerable<Claim> claims)
        {
            return new TokenPair<JwtSecurityToken>(GetAccessToken(claims), GetRefreshToken(claims));
        }

        public static Dictionary<string, string> WriteTokens(TokenPair<JwtSecurityToken> pair)
        {
            return new Dictionary<string, string>()
            {
                { "access_token",  WriteToken(pair.Access) },
                { "refresh_token", WriteToken(pair.Refresh) }
            };
        }

        private static IEnumerable<Claim> ClearClaims(IEnumerable<Claim> claims)
        {
            return claims.Where(c => c.Type == "Id" || c.Type == "Login" || c.Type == "Type");
        }
        public static string? GetClaim(this IEnumerable<Claim> claims, string type)
        {
            return claims.First(c => c.Type == type)?.Value;
        }

        public static ClaimsIdentity? GetIdentity(Organisation? organisation)
        {
            if (organisation == default)
                return null;

            var claims = new List<Claim>
            {
                new Claim("Login", organisation.Login),
                new Claim("Id", organisation.Id.ToString()),
                new Claim("Type", "1")
            };
            return new ClaimsIdentity(claims);
        }

        public static ClaimsIdentity? GetIdentity(UserModel? user)
        {
            if (user == default)
                return null;

            var claims = new List<Claim>
            {
                new Claim("Login", user.Login),
                new Claim("Id", user.Id.ToString()),
                new Claim("Type", "0")
            };
            return new ClaimsIdentity(claims);
        }
    }
}
