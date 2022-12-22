using Database.Models; // класс Organisation
using Database.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend_Bank.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        // тестовые данные вместо использования базы данных
        List<Organisation> people = new();

        private readonly IOrganisationsRepository _orgRep;

        public AccountController(IOrganisationsRepository orgRep)
        {
            _orgRep = orgRep;
        }

        [HttpPost("/token")]
        public IActionResult Token(string username, string password)
        {
            var identity = GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name,
                ient = identity
            };

            return Json(response);
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            Organisation person = people.FirstOrDefault(x => x.Login == username && x.Password == password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.OrgName)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }

        [HttpPost("register")]
        public IActionResult Rgister(Organisation org)
        {
            if (string.IsNullOrEmpty(org.Login) || string.IsNullOrEmpty(org.Password))
                return BadRequest(new { errorText = "Invalid username or password." });

            _orgRep.Create(org);
            _orgRep.Save();

            var response = new
            {
                Login = org.Login,
                PasswordHasg = new PasswordHasher<Organisation>().HashPassword(org, org.Password)
            };
            return Json(response);
        }

        [HttpGet("/all")]
        public IActionResult GetAll()
        {
            return Json(_orgRep.GetAll());
        }
    }
}