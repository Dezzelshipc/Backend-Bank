using Backend_Bank.Converters;
using Backend_Bank.Token;
using Database.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Bank.Controllers
{
    [Route("api/v1/token")]
    public class TokenController : Controller
    {
        private readonly ITokenRepository _tokRep;

        public TokenController(ITokenRepository tokRep)
        {
            _tokRep = tokRep;
        }

        [Authorize]
        [HttpPost("validate")]
        public IActionResult GetIdFromToken()
        {
            if (!User.Claims.CheckClaim())
                return BadRequest(new { error = "Invalid token. Required access", isSuccess = 0 });

            return Ok(isSuccess: true);
        }

        [Authorize]
        [HttpGet("refresh_token")]
        public IActionResult GetAccessToken()
        {
            if (!User.Claims.CheckClaim(value: "refresh"))
                return BadRequest(new { error = "Invalid token. Required refresh", isSuccess = 0 });


            var token = _tokRep.GetTokenById(int.Parse(User.Claims.GetClaim("Id")), User.Claims.GetClaim("Type").ToObjectType());
            if (token == default)
                return BadRequest(new { error = "Invalid token", isSuccess = 0 });

            if (token.Token != User.Claims.GetClaim("nbf"))
                return BadRequest(new { error = "Invalid token", isSuccess = 0 });

            var tokens = TokenManager.Tokens(User.Claims);

            token.Token = tokens.Refresh.Claims.GetClaim("nbf");

            try
            {
                _tokRep.Update(token);
                _tokRep.Save();
                return Json(TokenManager.WriteTokens(tokens));
            }
            catch
            {
                return BadRequest(new { error = "Error while creating", isSuccess = 0 });
            }

        }

        [HttpOptions("api/[controller]/all")]
        public IActionResult GetAll()
        {
            return Json(_tokRep.GetAll());
        }

        [HttpOptions("api/[controller]/token_claims")]
        public IActionResult Test()
        {
            return Json(User.Claims.ToDictionary(a => a.Type, a => a.Value));
        }
    }
}
