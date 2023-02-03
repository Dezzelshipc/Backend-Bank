using Database.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Backend_Bank.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly IUsersRepository _userRep;
        private readonly IServiceRepository _servRep;
        private readonly ITokenRepository _tokRep;
        private readonly IBranchesRepository _brRep;
        private readonly IOrganisationsRepository _orgRep;
        private readonly ILoansRepository _loanRep;

        public TestController(IUsersRepository userRep, IServiceRepository servRep, ITokenRepository tokRep,
            IBranchesRepository brRep, IOrganisationsRepository orgRep, ILoansRepository loanRep)
        {
            _userRep = userRep;
            _servRep = servRep;
            _tokRep = tokRep;
            _brRep = brRep;
            _orgRep = orgRep;
            _loanRep = loanRep;
        }

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
        [HttpDelete("clearDataBases")]
        public IActionResult ClearAll(string check)
        {
            if (check != "asd123")
                return BadRequest(new { error = "error check" });

            DeleteAll(_brRep);
            DeleteAll(_loanRep);
            DeleteAll(_orgRep);
            DeleteAll(_servRep);
            DeleteAll(_tokRep);
            DeleteAll(_userRep);

            return Ok();
        }

        private void DeleteAll<T>(IRepository<T> rep) where T : IModel
        {
            var all = rep.GetAll();
            foreach(var x in all)
            {
                rep.Delete(x);
            }
            rep.Save();
        }
    }
}
