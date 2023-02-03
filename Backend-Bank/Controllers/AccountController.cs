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
    /// <response code="400">If error occured or provided data is invalid</response>
    [Route("api/v1/user")]
    public class AccountController : Controller
    {
        private readonly IUsersRepository _userRep;
        private readonly IServiceRepository _servRep;
        private readonly ITokenRepository _tokRep;
        private readonly IBranchesRepository _brRep;
        private readonly IOrganisationsRepository _orgRep;

        public AccountController(IUsersRepository userRep, IServiceRepository servRep, ITokenRepository tokRep, IBranchesRepository brRep, IOrganisationsRepository orgRep)
        {
            _userRep = userRep;
            _servRep = servRep;
            _tokRep = tokRep;
            _brRep = brRep;
            _orgRep = orgRep;
        }

        /// <summary>
        /// Authorizes user
        /// </summary>
        /// <param name="log"></param>
        /// <remarks>Returns Access and Refresh User tokens</remarks>
        [HttpPost("authorization")]
        public IActionResult Authorize([FromBody] LoginModel log)
        {
            if (log == null || log.IsNotValid())
                return BadRequest(new { error = "Invalid input.", isSuccess = false });

            var login = log.Login;
            var password = log.Password;

            UserModel? user = _userRep.GetUserByLogin(login);
            if (user == default)
                return BadRequest(new { error = "Invalid login or password.", error_mark = "Not Found", isSuccess = false });

            var identity = TokenManager.GetIdentity(user);
            if (identity == null)
                return BadRequest(new { error = "Invalid login or password.", error_mark = "Invalid identity", isSuccess = false });

            if ((new PasswordHasher<UserModel>().VerifyHashedPassword(new UserModel(login, password), user.Password, password)) == 0)
                return BadRequest(new { error = "Invalid login or password.", error_mark = "Invalid password", isSuccess = false });

            var tokens = TokenManager.Tokens(identity.Claims);

            var old_token = _tokRep.GetTokenById(user.Id, ObjectType.User);
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

        /// <summary>
        /// Registers new user
        /// </summary>
        /// <param name="log"></param>
        /// <remarks>Returns Access and Refresh User tokens</remarks>
        [HttpPost("registration")]
        public IActionResult Rgister([FromBody] LoginModel log)
        {
            if (log == null || log.IsNotValid())
                return BadRequest(new { error = "Invalid input.", isSuccess = false });

            var login = log.Login;
            var password = log.Password;

            UserModel user = new(login, password)
            {
                Password = new PasswordHasher<UserModel>().HashPassword(new UserModel(login, password), password)
            };

            if (!user.IsSemiValid())
                return BadRequest(new { error = "Invalid data.", isSuccess = false });

            if (_userRep.GetUserByLogin(login) != default)
                return BadRequest(new { error = "User already exists.", isSuccess = false });

            try
            {
                _userRep.Create(user);
                _userRep.Save();

                var saved_user = _userRep.GetUserByLogin(user.Login)!;

                var tokens = TokenManager.Tokens(TokenManager.GetIdentity(saved_user)!.Claims);

                var new_token = new TokenModel(saved_user.Id, ObjectType.User, tokens.Refresh.Claims.GetClaim("nbf")!);

                _tokRep.Create(new_token);
                _tokRep.Save();

                return Json(TokenManager.WriteTokens(tokens));
            }
            catch
            {
                return BadRequest(new { error = "Error while creating.", isSuccess = false });
            }
        }

        /// <summary>
        /// Verifies user
        /// </summary>
        /// <param name="userData"></param>
        /// <remarks>
        /// Requires User Access token
        /// </remarks>
        /// <returns>isSuccess = true</returns>
        /// <response code="401">If token not provided</response>
        /// <response code="403">If token is invalid</response>
        [Authorize(Policy.UserAccess)]
        [HttpPost("verification")]
        public IActionResult Verify([FromBody] UserData userData)
        {
            if (userData == null || userData.IsNotValid())
                return BadRequest(new { error = "Invalid input.", isSuccess = false });

            var login = User.FindFirstValue("Login");
            if (login == null)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            UserModel? user = _userRep.GetUserByLogin(login);

            if (user == default)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });


            user.Phone = userData.Phone;
            user.Email = userData.Email;
            user.FullName = userData.FullName;

            try
            {
                _userRep.Update(user);
                _userRep.Save();
                return Ok(new { isSuccess = true });
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
                return Json(_userRep.GetAll());
            }
            catch
            {
                return BadRequest(new { error = "Error." });
            }
        }

        /// <summary>
        /// Returns user personal data
        /// </summary>
        /// <remarks>
        /// Requires User Access token
        /// 
        /// Returns { login, phone, email, fullname }
        /// </remarks>
        /// <response code="401">If token not provided</response>
        /// <response code="403">If token is invalid</response>
        [Authorize(Policy.UserAccess)]
        [HttpGet("getPersonalData")]
        public IActionResult GetPersonalData()
        {
            var login = User.FindFirstValue("Login");

            if (login == null)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            UserModel? user = _userRep.GetUserByLogin(login);

            if (user == null)
                return BadRequest(new { error = "User not found.", isSuccess = false });

            return Json(new
            {
                login = login!,
                phone = user.Phone,
                email = user.Email,
                fullname = user.FullName
            });
        }

        /// <summary>
        /// Changes user personal data user
        /// </summary>
        /// <remarks>
        /// Requires User Access token
        /// </remarks>
        /// <returns>
        /// isSuccess = true
        /// </returns>
        /// <response code="401">If token not provided</response>
        /// <response code="403">If token is invalid</response>
        [Authorize(Policy.UserAccess)]
        [HttpPost("changePersonalData")]
        public IActionResult ChangePersonalData([FromBody] UserFullData userData)
        {
            if (userData == null)
                return BadRequest(new { error = "Invalid input.", isSuccess = false });

            var old_login = User.FindFirstValue("Login");

            if (old_login == null)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            UserModel? user = _userRep.GetUserByLogin(old_login);

            if (user == null)
                return BadRequest(new { error = "User not found.", isSuccess = false });

            if (userData.Login != null)
                user.Login = userData.Login;

            if (userData.Phone != null)
                user.Phone = userData.Phone;

            if (userData.Email != null)
                user.Email = userData.Email;

            if (userData.FullName != null)
                user.FullName = userData.FullName;

            try
            {
                _userRep.Update(user);
                _userRep.Save();
                return Json(new { isSuccess = true });
            }
            catch
            {
                return BadRequest(new { error = "Error while updating.", isSuccess = false });
            }
        }

        /// <summary>
        /// Returns every service with organisation Id
        /// </summary>
        /// <param name="orgId"></param>
        /// <remarks>
        /// Requires User Access token
        /// 
        /// Returns  { int id, str serviceName, str description, str percent, str minLoanPeriod, str maxLoadPeriod }
        /// </remarks>
        /// <response code="401">If token not provided</response>
        /// <response code="403">If token is invalid</response>
        [Authorize(Policy.UserAccess)]
        [HttpGet("getOnlineServicesByOrgId")]
        public IActionResult GetOnlineServicesByOrgId(int orgId)
        {
            var services = _servRep.GetServices(orgId);
            IEnumerable<dynamic> response = services.Select(a => new
            {
                id = a.Id,
                serviceName = a.Name,
                description = a.Description,
                percent = a.Percent,
                minLoanPeriod = a.MinLoanPeriod,
                maxLoadPeriod = a.MaxLoadPeriod
            });
            return Json(response);
        }

        /// <summary>
        /// Returns every branch on distance in km from coordinates
        /// </summary>
        /// <param name="distanceKm"></param>
        /// <param name="position"></param>
        /// <remarks>
        /// abs(Longitiude) not greater 180
        /// 
        /// abs(Latitude) not greater 90
        /// 
        /// Returns [{ int id, int organisationId, str name, str address, str phoneNumber, double longtitude, double latitude }]
        /// </remarks>
        /// <response code="401">If token not provided</response>
        /// <response code="403">If token is invalid</response>
        [HttpGet("getBranchesOnDistance")]
        public IActionResult GetNearestBranches(double distanceKm, Position position)
        {
            var response = _brRep.OnDistande(distanceKm, position);

            return Json(response);
        }

        /// <summary>
        /// Returns every organisation
        /// </summary>
        /// <remarks>
        /// Requires User Access token
        /// 
        /// Returns [{ int id, str orgName, str name, str legalAddress, str genDirector, str-datetime foundingDate }]
        /// </remarks>
        /// <response code="401">If token not provided</response>
        /// <response code="403">If token is invalid</response>
        [Authorize(Policy.UserAccess)]
        [HttpGet("getAllOrganisations")]
        public IActionResult GetAllOrgs()
        {
            try
            {
                return Json(_orgRep.GetAll().Select(a => new
                {
                    a.Id,
                    a.OrgName,
                    a.LegalAddress,
                    a.GenDirector,
                    a.FoundingDate
                }));
            }
            catch
            {
                return BadRequest(new { error = "Error.", isSuccess = false });
            }
        }
    }
}
