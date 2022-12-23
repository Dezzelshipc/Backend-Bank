using Database.Interfaces;
using Database.Logic;
using Database.Models; // класс Organisation
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend_Bank.Controllers
{
    [Route("api/v1/organisation")]
    public class AccountController : Controller
    {
        private readonly IOrganisationsRepository _orgRep;

        public AccountController(IOrganisationsRepository orgRep)
        {
            _orgRep = orgRep;
        }

        [HttpPost("authorization")]
        public IActionResult Authorize(string login, string password)
        {
            Organisation? organisation = _orgRep.GetOrganisationByLogin(login);
            if (organisation == default)
                return BadRequest(new { error = "Invalid login or password." });

            var identity = GetIdentity(organisation);
            if (identity == null)
                return BadRequest(new { error = "Invalid login or password." });

            if ((new PasswordHasher<User>().VerifyHashedPassword(new User(login, password), organisation.Password, password)) == 0)
                return BadRequest(new { error = "Invalid login or password." });

            return Token(identity.Claims);
        }

        private IActionResult Token(IEnumerable<Claim> claims)
        {
            var response = new
            {
                access_token = TokenGetter.GetAccessToken(claims),
                refresh_token = TokenGetter.GetRefreshToken(claims)
            };
            
            return Json(response);
        }

        private static ClaimsIdentity GetIdentity(Organisation? organisation)
        {
            if (organisation == default)
                return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, organisation.Login),
                new Claim(ClaimsIdentity.DefaultNameClaimType, organisation.OrgName)
            };
            ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultNameClaimType);
            return claimsIdentity;
        }

        [HttpPost("registration")]
        public IActionResult Rgister(string login, string password, string orgName, string legalAddress, string genDirector, DateTime foundingDate)
        {
            Organisation org = new(login, password, orgName, legalAddress, genDirector, foundingDate)
            {
                Password = new PasswordHasher<User>().HashPassword(new User(login, password), password)
            };

            if (!org.IsValid())
                return BadRequest(new { error = "Invalid data." });

            if (_orgRep.GetOrganisationByLogin(login) != default)
                return BadRequest(new { error = "Organisation already exists." });

            _orgRep.Create(org);
            _orgRep.Save();

            return Token(GetIdentity(org).Claims);
        }

        [HttpGet("/all")]
        public IActionResult GetAll()
        {
            return Json(_orgRep.GetAll());
        }
    }
}