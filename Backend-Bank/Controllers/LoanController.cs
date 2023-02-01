using Backend_Bank.Token;
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

        [Authorize]
        [HttpGet("takeLoanOnline")]
        public IActionResult TakeLoan(int userId, int serviceId, int amountMonth, int period)
        {
            if (!User.Claims.CheckClaim())
                return BadRequest(new { error = "Invalid token", isSuccess = 0 });

            Loan loan = new(userId, serviceId, amountMonth, period);

            try
            {
                _loanRep.Create(loan);
                _loanRep.Save();
                return BadRequest(new { isSuccess = 1 });
            }
            catch
            {
                return BadRequest(new { error = "Error while creating", isSuccess = 0 });
            }
        }

        [HttpOptions("/api/[controller]/all")]
        public IActionResult GetAll()
        {
            return Json(_loanRep.GetAll());
        }
    }
}
