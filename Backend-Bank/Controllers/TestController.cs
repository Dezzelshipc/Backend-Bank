using Microsoft.AspNetCore.Mvc;

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
        [HttpPost("post")]
        public IActionResult Post()
        {
            return Ok(new { isSuccess = true });
        }
    }
}
