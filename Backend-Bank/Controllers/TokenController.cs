using Microsoft.AspNetCore.Mvc;
using Database.Logic;
using System.Linq;

namespace Backend_Bank.Controllers
{
    [Route("api/v1/token")]
    public class TokenController : Controller
    {
        [HttpPost("validate")]
        public IActionResult GetIdFromToken(string token)
        {
            var val = TokenManager.ValidateToken(token);

            if (val == null)
                return BadRequest(new { error = "Invalid token." });

            return Ok(val.Value);
        }

        [HttpGet("refresh_token")]
        public IActionResult GetAccessToken(string refresh_token)
        {
            var claims = TokenManager.GetTokenClaimsValidate(refresh_token);
            var token = TokenManager.ClaimsToTokenModel(claims);

            if (token == null || !token.Type)
                return BadRequest(new { error = "Inalid token." });

            claims = claims!.Where(a => a.Type != "type");

            return Json(new { access_token = TokenManager.GetAccessToken(claims) });
        }
    }
}
