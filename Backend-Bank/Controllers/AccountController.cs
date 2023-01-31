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

        public AccountController(IUsersRepository userRep, IServiceRepository servRep)
        {
            _userRep = userRep;
            _servRep = servRep;
        }

        [HttpPost("authorization")]
        public IActionResult Authorize(string login, string password)
        {
            UserModel? user = _userRep.GetUserByLogin(login);
            if (user == default)
                return BadRequest(new { error = "Invalid login or password." });

            var identity = TokenManager.GetIdentity(user);
            if (identity == null)
                return BadRequest(new { error = "Invalid login or password." });

            if ((new PasswordHasher<UserModel>().VerifyHashedPassword(new UserModel(login, password), user.Password, password)) == 0)
                return BadRequest(new { error = "Invalid login or password." });

            return Json(TokenManager.Tokens(identity.Claims));
        }

        [HttpPost("registration")]
        public IActionResult Rgister(string login, string password)
        {
            UserModel user = new(login, password)
            {
                Password = new PasswordHasher<UserModel>().HashPassword(new UserModel(login, password), password)
            };

            if (!user.IsValid())
                return BadRequest(new { error = "Invalid data." });

            if (_userRep.GetUserByLogin(login) != default)
                return BadRequest(new { error = "User already exists." });

            try
            {
                _userRep.Create(user);
                _userRep.Save();
                return Json(TokenManager.Tokens(TokenManager.GetIdentity(user).Claims));
            }
            catch
            {
                return BadRequest(new { error = "Error while creating." });
            }
        }

        [Authorize(Roles = "access")]
        [HttpPost("verification")]
        public IActionResult Verify(string phone, string email, string fullname)
        {
            var login = User.Identity.Name;
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

        //[Authorize(Roles = "access")]
        [HttpGet("/api/[controller]/all")]
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

        [Authorize(Roles = "access")]
        [HttpGet("getPersonalData")]
        public IActionResult GetPersonalData()
        {
            var login = User.Identity.Name;

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

        [Authorize(Roles = "access")]
        [HttpPost("changePersonalData")]
        public IActionResult ChangePersonalData(string? login, string? phone, string? email, string? fullname)
        {
            var old_login = User.Identity.Name;

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

        internal class SmallService
        {
            public string serviceName { get; set; }
            public string description { get; set; }
            public string percent { get; set; }
            public string minLoanPeriod { get; set; }

            SmallService(Service service)
            {
                serviceName = service.Name;
                description = service.Description;
                percent = service.Percent;
                minLoanPeriod = service.MinLoanPeriod;
            }
        }

        [HttpGet("getServices")]
        public IActionResult GetServices(int id)
        {
            if (id < 0)
                return BadRequest(new { error = "Invalid Id." });

            var services = _servRep.GetServices(id);

            return Json(services);
        }

        //[Authorize(Roles = "access")]
        [HttpGet("takeLoanOnline")]
        public IActionResult TakeLoan(int id)
        {
            return BadRequest(error: "Not yet working");
        }
    }
}
