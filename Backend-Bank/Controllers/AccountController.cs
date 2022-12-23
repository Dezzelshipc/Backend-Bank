using Database.Interfaces;
using Database.Logic;
using Database.Models;
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
                access_token = TokenManager.GetAccessToken(claims),
                refresh_token = TokenManager.GetRefreshToken(claims)
            };

            return Json(response);
        }

        private static ClaimsIdentity GetIdentity(Organisation? organisation)
        {
            if (organisation == default)
                return null;

            var claims = new List<Claim>
            {
                new Claim("login", organisation.Login)
            };
            return new ClaimsIdentity(claims);
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

            try
            {
                _orgRep.Create(org);
                _orgRep.Save();
                return Token(GetIdentity(org).Claims);
            }
            catch
            {
                return BadRequest(new { error = "Error while creating." });
            }
        }

        [HttpPost("removeOrganisation")]
        public IActionResult Remove(string login, string password)
        {
            Organisation? organisation = _orgRep.GetOrganisationByLogin(login);
            if (organisation == default)
                return BadRequest(new { error = "Invalid login or password." });

            if ((new PasswordHasher<User>().VerifyHashedPassword(new User(login, password), organisation.Password, password)) == 0)
                return BadRequest(new { error = "Invalid login or password." });

            try
            {
                _orgRep.Delete(organisation.Id);
                _orgRep.Save();
                return Ok();
            }
            catch
            {
                return BadRequest(new { error = "Error while deleting." });
            }

        }

        [HttpGet("/[controller]/all")]
        public IActionResult GetAll()
        {
            try
            {
                return Json(_orgRep.GetAll());
            }
            catch
            {
                return BadRequest(new { error = "Error." });
            }
        }

        [HttpPost("getPersonalData")]
        public IActionResult GetPersonalData(string access_token)
        {
            var login = TokenManager.ValidateToken(access_token);

            if (login == null || login.Type == true)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Organisation? organisation = _orgRep.GetOrganisationByLogin(login.Value);

            if (organisation == null)
                return BadRequest(new { error = "Organisation not found.", isSuccess = false });

            return Json(new
            {
                orgName = organisation.OrgName,
                legalAddress = organisation.LegalAddress,
                genDirector = organisation.GenDirector,
                foundingDate = organisation.FoundingDate
            });
        }

        [HttpPost("changePersonalData")]
        public IActionResult ChangePersonalData(string access_token, string? orgName, string? legalAddress, string? genDirector, DateTime? foundingDate)
        {
            var login = TokenManager.ValidateToken(access_token);

            if (login == null || login.Type == true)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Organisation? organisation = _orgRep.GetOrganisationByLogin(login.Value);

            if (organisation == null)
                return BadRequest(new { error = "Organisation not found.", isSuccess = false });

            if (orgName != null)
                organisation.OrgName = orgName;

            if (legalAddress != null)
                organisation.LegalAddress = legalAddress;

            if (genDirector != null)
                organisation.GenDirector = genDirector;

            if (foundingDate != null)
                organisation.FoundingDate = (DateTime)foundingDate;

            try
            {
                _orgRep.Update(organisation);
                _orgRep.Save();
                return Json(new { error = "", isSuccess = true });
            }
            catch
            {
                return BadRequest(new { error = "Error while updating." });
            }
        }
    }
}