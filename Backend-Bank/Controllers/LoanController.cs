using Backend_Bank.Requirements;
using Backend_Bank.Tokens;
using Database.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Bank.Controllers
{
    [Route("api/v1/user")]
    public class LoanController : Controller
    {
        private readonly ILoansRepository _loanRep;

        public LoanController(ILoansRepository loanRep)
        {
            _loanRep = loanRep;
        }

        [Authorize(Policy.UserAccess)]
        [HttpPost("takeLoanOnline")]
        public IActionResult TakeLoan([FromBody] LoanData loanData)
        {
            if (loanData == null || loanData.IsNotValid())
                return BadRequest(new { error = "Invalid token", isSuccess = false });

            Loan loan = new(loanData.UserId, loanData.ServiceId, loanData.AmountMonth, loanData.Period);

            try
            {
                _loanRep.Create(loan);
                _loanRep.Save();
                return Ok(new { isSuccess = true });
            }
            catch
            {
                return BadRequest(new { error = "Error while creating", isSuccess = false });
            }
        }

        [HttpOptions("/api/[controller]/all")]
        public IActionResult GetAll()
        {
            return Json(_loanRep.GetAll());
        }
    }
}
