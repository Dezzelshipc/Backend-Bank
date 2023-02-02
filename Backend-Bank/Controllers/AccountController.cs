using Backend_Bank.Token;
using Database.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Bank.Controllers
{
    [Route("api/v1/user")]
    public class AccountController : Controller
    {
        private readonly IUsersRepository _userRep;
        private readonly IServiceRepository _servRep;
        private readonly ITokenRepository _tokRep;

        public AccountController(IUsersRepository userRep, IServiceRepository servRep, ITokenRepository tokRep)
        {
            _userRep = userRep;
            _servRep = servRep;
            _tokRep = tokRep;
        }

        [HttpPost("authorization")]
        public IActionResult Authorize([FromBody] LoginModel log)
        {
            if (log == null)
                return BadRequest(new { error = "Invalid input." });

            var login = log.Login;
            var password = log.Password;

            UserModel? user = _userRep.GetUserByLogin(login);
            if (user == default)
                return BadRequest(new { error = "Invalid login or password.", error_mark = "Not Found" });

            var identity = TokenManager.GetIdentity(user);
            if (identity == null)
                return BadRequest(new { error = "Invalid login or password.", error_mark = "Invalid identity" });

            if ((new PasswordHasher<UserModel>().VerifyHashedPassword(new UserModel(login, password), user.Password, password)) == 0)
                return BadRequest(new { error = "Invalid login or password.", error_mark = "Invalid password" });

            var tokens = TokenManager.Tokens(identity.Claims);

            var old_token = _tokRep.GetTokenById(user.Id, ObjectType.User);
            if (old_token == null)
                return BadRequest(new { error = "Invalid user. Probably was created before refresh token update" });

            old_token.Token = tokens.Refresh.Claims.GetClaim("nbf")!;

            try
            {
                _tokRep.Update(old_token);
                _tokRep.Save();
                return Json(TokenManager.WriteTokens(tokens));
            }
            catch
            {
                return BadRequest(new { error = "Error with tokens", isSuccess = 0 });
            }
        }

        [HttpPost("registration")]
        public IActionResult Rgister([FromBody] LoginModel log)
        {
            if (log == null)
                return BadRequest(new { error = "Invalid input." });

            var login = log.Login;
            var password = log.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                return BadRequest(new { error = "Invalid input." });

            UserModel user = new(login, password)
            {
                Password = new PasswordHasher<UserModel>().HashPassword(new UserModel(login, password), password)
            };

            if (!user.IsSemiValid())
                return BadRequest(new { error = "Invalid data." });

            if (_userRep.GetUserByLogin(login) != default)
                return BadRequest(new { error = "User already exists." });

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
                return BadRequest(new { error = "Error while creating." });
            }
        }

        [Authorize]
        [HttpPost("verification")]
        public IActionResult Verify([FromBody] UserData userData)
        {
            if (userData == null)
                return BadRequest(new { error = "Invalid input." });

            var phone = userData.Phone;
            var email = userData.Email;
            var fullname = userData.FullName;

            if (!User.Claims.CheckClaim())
                return BadRequest(new { error = "Invalid token. Required access", isSuccess = 0 });

            var login = User.Claims.GetClaim("Login");
            if (login == null)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            UserModel? user = _userRep.GetUserByLogin(login);

            if (user == default)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullname))
                return BadRequest(new { error = "Invalid input data.", isSuccess = false });

            user.Phone = phone;
            user.Email = email;
            user.FullName = fullname;

            try
            {
                _userRep.Update(user);
                _userRep.Save();
                return Ok(new { isSuccess = 1 });
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

        [Authorize]
        [HttpGet("getPersonalData")]
        public IActionResult GetPersonalData()
        {
            if (!User.Claims.CheckClaim())
                return BadRequest(new { error = "Invalid token. Required access", isSuccess = 0 });

            var login = User.Claims.GetClaim("Login");

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

        [Authorize]
        [HttpPost("changePersonalData")]
        public IActionResult ChangePersonalData([FromBody] UserFullData userData)
        {
            if (userData == null)
                return BadRequest(new { error = "Invalid input." });

            var login = userData.Login;
            var phone = userData.Phone;
            var email = userData.Email;
            var fullname = userData.FullName;


            if (!User.Claims.CheckClaim())
                return BadRequest(new { error = "Invalid token. Required access", isSuccess = 0 });

            var old_login = User.Claims.GetClaim("Login");

            if (old_login == null)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            UserModel? user = _userRep.GetUserByLogin(old_login);

            if (user == null)
                return BadRequest(new { error = "User not found.", isSuccess = false });

            if (login != null)
                user.Login = login;

            if (phone != null)
                user.Phone = phone;

            if (email != null)
                user.Email = email;

            if (fullname != null)
                user.FullName = fullname;

            try
            {
                _userRep.Update(user);
                _userRep.Save();
                return Json(new { isSuccess = true });
            }
            catch
            {
                return BadRequest(new { error = "Error while updating." });
            }
        }

        [HttpGet("getBranches")]
        public IActionResult GetNearestBranches(int distance, string position)
        {
            return BadRequest(error: "Not yet working");
        }

        [HttpGet("getServices")]
        public IActionResult GetServices([FromBody] int id)
        {
            if (id < 0)
                return BadRequest(new { error = "Invalid Id." });

            var services = _servRep.GetServices(id);

            return Json(services);
        }
    }
}
