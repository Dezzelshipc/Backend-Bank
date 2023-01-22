using Database.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Bank.Controllers
{
    [Route("api/v1/token")]
    public class TokenController : Controller
    {
        [Authorize(Roles = "access")]
        [HttpPost("validate")]
        public IActionResult GetIdFromToken()
        {
            return Ok();
        }

        [Authorize(Roles = "refresh")]
        [HttpGet("refresh_token")]
        public IActionResult GetAccessToken()
        {
            return Json(new { access_token = TokenManager.GetAccessToken(User.Claims) });
        }
    }
}
