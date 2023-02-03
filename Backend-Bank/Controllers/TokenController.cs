using Backend_Bank.Requirements;
using Backend_Bank.Tokens;
using Database.Converters;
using Database.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend_Bank.Controllers
{
    /// <response code="400">If error occured or provided data is invalid</response>
    /// <response code="401">If token not provided</response>
    /// <response code="403">If token is invalid</response>
    [Route("api/v1/token")]
    public class TokenController : Controller
    {
        private readonly ITokenRepository _tokRep;

        public TokenController(ITokenRepository tokRep)
        {
            _tokRep = tokRep;
        }

        /// <summary>
        /// Checks if Access toket is valid
        /// </summary>
        /// <remarks>
        /// Requires any Access token
        /// </remarks>
        [Authorize(Policy.Access)]
        [HttpPost("validate")]
        public IActionResult ValidateAccess()
        {
            return Ok(new { isSuccess = true });
        }

        /// <summary>
        /// Checks if Refresh toket is valid
        /// </summary>
        /// <remarks>
        /// Requires any Refresh token
        /// </remarks>
        [Authorize(Policy.Refresh)]
        [HttpPost("validate_refresh")]
        public IActionResult ValidateRefresh()
        {
            return Ok(new { isSuccess = true });
        }

        /// <summary>
        /// Returns new Refresh and Access tokens of same type
        /// </summary>
        /// <remarks>
        /// Requires any Refresh token
        /// 
        /// Invalidates current Refresh token
        /// </remarks>
        [Authorize(Policy.Refresh)]
        [HttpGet("refresh_token")]
        public IActionResult GetTokens()
        {
            var id_s = User.FindFirstValue("Id");
            var type = User.FindFirstValue("Type");

            if (!int.TryParse(id_s, out int id) || type == null)
                return BadRequest(new { error = "Invalid token", isSuccess = 0 });

            var token = _tokRep.GetTokenById(id, type.ToObjectType());
            if (token == default)
                return BadRequest(new { error = "Invalid token", isSuccess = 0 });

            if (token.Token != User.FindFirstValue("nbf"))
                return BadRequest(new { error = "Invalid token", isSuccess = 0 });

            var tokens = TokenManager.Tokens(User.Claims);

            token.Token = tokens.Refresh.Claims.GetClaim("nbf")!;

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
