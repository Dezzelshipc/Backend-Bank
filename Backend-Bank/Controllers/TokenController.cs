using Microsoft.AspNetCore.Mvc;
using Database.Logic;

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
    }
}
