using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Backend_Bank.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        [HttpGet("get")]
        public IActionResult Get()
        {
            return Ok(new { isSuccess = true });
        }
        [HttpGet("getBack")]
        public IActionResult GetBack(string str)
        {
            return Ok(new { str, isSuccess = true });
        }
        [HttpPost("post")]
        public IActionResult Post()
        {
            return Ok(new { isSuccess = true });
        }
        [HttpPost("postBack")]
        public IActionResult PostBack([FromBody] string str)
        {
            return Ok(new { str, isSuccess = true });
        }
    }
}
