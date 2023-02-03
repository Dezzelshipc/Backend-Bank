using Backend_Bank.Requirements;
using Backend_Bank.Tokens;
using Database.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend_Bank.Controllers
{
    [Route("api/v1/organisation")]
    public class OrganisationController : Controller
    {
        private readonly IOrganisationsRepository _orgRep;
        private readonly ITokenRepository _tokRep;

        public OrganisationController(IOrganisationsRepository orgRep, ITokenRepository tokRep)
        {
            _orgRep = orgRep;
            _tokRep = tokRep;
        }

        [HttpPost("authorization")]
        public IActionResult Authorize([FromBody] LoginModel log)
        {
            if (log == null || log.IsNotValid())
                return BadRequest(new { error = "Invalid input.", isSuccess = false });

            var login = log.Login;
            var password = log.Password;

            Organisation? organisation = _orgRep.GetOrganisationByLogin(login);
            if (organisation == default)
                return BadRequest(new { error = "Invalid login or password.", isSuccess = false });

            var identity = TokenManager.GetIdentity(organisation);
            if (identity == null)
                return BadRequest(new { error = "Invalid login or password.", isSuccess = false });

            if ((new PasswordHasher<UserModel>().VerifyHashedPassword(new UserModel(login, password), organisation.Password, password)) == 0)
                return BadRequest(new { error = "Invalid login or password.", isSuccess = false });

            var tokens = TokenManager.Tokens(identity.Claims);

            var old_token = _tokRep.GetTokenById(organisation.Id, ObjectType.Organisation);
            if (old_token == null)
                return BadRequest(new { error = "Invalid user. Probably was created before refresh token update", isSuccess = false });

            old_token.Token = tokens.Refresh.Claims.GetClaim("nbf")!;

            try
            {
                _tokRep.Update(old_token);
                _tokRep.Save();
                return Json(TokenManager.WriteTokens(tokens));
            }
            catch
            {
                return BadRequest(new { error = "Error with tokens", isSuccess = false });
            }
        }

        [HttpPost("registration")]
        public IActionResult Rgister([FromBody] OrgFullData ofd)
        {
            if (ofd == null || ofd.IsNotValid())
                return BadRequest(new { error = "Invalid input.", isSuccess = false });

            Organisation org = new(ofd.Login, ofd.Password, ofd.OrgName, ofd.LegalAddress, ofd.GenDirector, ofd.FoundingDate)
            {
                Password = new PasswordHasher<UserModel>().HashPassword(new UserModel(ofd.Login, ofd.Password), ofd.Password)
            };

            if (!org.IsValid())
                return BadRequest(new { error = "Invalid data.", isSuccess = false });

            if (_orgRep.GetOrganisationByLogin(ofd.Login) != default)
                return BadRequest(new { error = "Organisation already exists.", isSuccess = false });


            try
            {
                _orgRep.Create(org);
                _orgRep.Save();

                var saved_org = _orgRep.GetOrganisationByLogin(org.Login)!;

                var tokens = TokenManager.Tokens(TokenManager.GetIdentity(saved_org)!.Claims);

                var new_token = new TokenModel(saved_org.Id, ObjectType.Organisation, tokens.Refresh.Claims.GetClaim("nbf")!);

                _tokRep.Create(new_token);
                _tokRep.Save();

                return Json(TokenManager.WriteTokens(tokens));
            }
            catch
            {
                return BadRequest(new { error = "Error while creating.", isSuccess = false });
            }
        }

        [Authorize(Policy.OrgAccess)]
        [HttpDelete("removeOrganisation")]
        public IActionResult Remove([FromBody] LoginModel log)
        {
            if (log == null || log.IsNotValid())
                return BadRequest(new { error = "Invalid input.", isSuccess = false });

            var login = log.Login;
            var password = log.Password;

            Organisation? organisation = _orgRep.GetOrganisationByLogin(login);
            if (organisation == default)
                return BadRequest(new { error = "Invalid login or password.", isSuccess = false });

            if ((new PasswordHasher<UserModel>().VerifyHashedPassword(new UserModel(login, password), organisation.Password, password)) == 0)
                return BadRequest(new { error = "Invalid login or password.", isSuccess = false });

            try
            {
                _orgRep.Delete(organisation.Id);
                _orgRep.Save();
                return Ok();
            }
            catch
            {
                return BadRequest(new { error = "Error while deleting.", isSuccess = false });
            }

        }

        [HttpOptions("/api/[controller]/all")]
        public IActionResult GetAll()
        {
            try
            {
                return Json(_orgRep.GetAll());
            }
            catch
            {
                return BadRequest(new { error = "Error.", isSuccess = false });
            }
        }

        [Authorize(Policy.OrgAccess)]
        [HttpGet("getPersonalData")]
        public IActionResult GetPersonalData()
        {
            var login = User.FindFirstValue("Login");

            if (login == null)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Organisation? organisation = _orgRep.GetOrganisationByLogin(login);

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

        [Authorize(Policy.OrgAccess)]
        [HttpPost("changePersonalData")]
        public IActionResult ChangePersonalData([FromBody] OrgData orgData)
        {
            if (orgData == null)
                return BadRequest(new { error = "Invalid input.", isSuccess = false });

            var login = User.FindFirstValue("Login");

            if (login == null)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Organisation? organisation = _orgRep.GetOrganisationByLogin(login);

            if (organisation == null)
                return BadRequest(new { error = "Organisation not found.", isSuccess = false });

            if (orgData.OrgName != null)
                organisation.OrgName = orgData.OrgName;

            if (orgData.LegalAddress != null)
                organisation.LegalAddress = orgData.LegalAddress;

            if (orgData.GenDirector != null)
                organisation.GenDirector = orgData.GenDirector;

            if (orgData.FoundingDate != null)
                organisation.FoundingDate = (DateTime)orgData.FoundingDate;

            try
            {
                _orgRep.Update(organisation);
                _orgRep.Save();
                return Json(new { isSuccess = true });
            }
            catch
            {
                return BadRequest(new { error = "Error while updating.", isSuccess = false });
            }
        }
    }
}