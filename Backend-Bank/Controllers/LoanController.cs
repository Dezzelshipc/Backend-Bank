using Backend_Bank.Requirements;
using Database.Converters;
using Database.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend_Bank.Controllers
{
    /// <response code="400">If error occured or provided data is invalid</response>
    /// <response code="401">If token not provided</response>
    /// <response code="403">If token is invalid</response>
    public class LoanController : Controller
    {
        protected readonly IUsersRepository _userRep;
        protected readonly ILoansRepository _loanRep;

        public LoanController(IUsersRepository userRep, ILoansRepository loanRep)
        {
            _userRep = userRep;
            _loanRep = loanRep;
        }
    }

    public class LoanAllController : LoanController
    {
        public LoanAllController(IUsersRepository userRep, ILoansRepository loanRep) : base(userRep, loanRep) { }

        [HttpOptions("/api/[controller]/all")]
        public IActionResult GetAll()
        {
            return Json(_loanRep.GetAll());
        }
    }

    [Route("api/v1/user")]
    public class LoanUserController : LoanController
    {
        public LoanUserController(IUsersRepository userRep, ILoansRepository loanRep) : base(userRep, loanRep) { }

        /// <summary>
        /// Saves loan notification
        /// </summary>
        /// <remarks>
        /// Requires User Access token
        /// </remarks>
        [Authorize(Policy.UserAccess)]
        [HttpPost("takeLoanOnline")]
        public IActionResult TakeLoan([FromBody] LoanData loanData)
        {
            if (loanData == null || loanData.IsNotValid())
                return BadRequest(new { error = "Invalid data", isSuccess = false });

            var userId = User.FindFirstValue("Id");
            if (userId == null)
                return BadRequest(new { error = "Invalid token", isSuccess = false });

            Loan loan = new(int.Parse(userId), loanData.ServiceId, loanData.Amount, loanData.Period);

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

        /// <summary>
        /// Returns all notifications by user phone number
        /// </summary>
        /// <remarks>
        /// Returns [{ int notificationId, int userId, str description, str status }]
        /// </remarks>
        [HttpGet("getAllNotification")]
        public IActionResult GetAllNotification(string phone)
        {
            var user = _userRep.GetUserByPhone(phone);
            if (user == null)
                return BadRequest(new { error = "User not found.", isSuccess = false });

            var loans = _loanRep.GetLoansByUserId(user.Id);

            IEnumerable<dynamic> respose = loans.Select(a => new
            {
                notificationId = a.Id,
                userId = a.UserId,
                description = a.Desctiption,
                status = a.Status.GetString()
            });
            return Json(respose);
        }
    }

    [Route("api/v1/organisation")]
    public class LoanOrganisationController : LoanController
    {
        public LoanOrganisationController(IUsersRepository userRep, ILoansRepository loanRep) : base(userRep, loanRep) { }

        /// <summary>
        /// Changes notification
        /// </summary>
        /// <remarks>
        /// Requires Organisation Access token
        /// </remarks>
        [Authorize(Policy.OrgAccess)]
        [HttpPost("changeNotification")]
        public IActionResult ChangeNotification([FromBody] LoanNotification loanChange)
        {
            var loan = _loanRep.GetItem(loanChange.Id);
            if (loan == null || loanChange.IsNotValid)
                return BadRequest(new { error = "Invalid data", isSuccess = false });

            if (loanChange.Description != null)
                loan.Desctiption = loanChange.Description;

            if (loanChange.Status != null)
                loan.Status = loanChange.Status.GetStatus();

            try
            {
                _loanRep.Update(loan);
                _loanRep.Save();
                return Ok(new { isSuccess = true });
            }
            catch
            {
                return BadRequest(new { error = "Error while saving", isSuccess = false });
            }
        }

        /// <summary>
        /// Returns all notifications by service id
        /// </summary>
        /// <remarks>
        /// Requires Organisation Access token
        /// 
        /// Returns [{ int id, int userId, int serviceId, int amount, int period, int desctiption, int status }]
        /// </remarks>
        [Authorize(Policy.OrgAccess)]
        [HttpGet("getAllLoansByServiceId")]
        public IActionResult ChangeNotification(int serviceId)
        {
            return Json(_loanRep.GetLoansByServiceId(serviceId).Select(a => a.WithFormatStatus()));
        }

        /// <summary>
        /// Removes notification by id
        /// </summary>
        /// <remarks>
        /// Requires Organisation Access token
        /// </remarks>
        [Authorize(Policy.OrgAccess)]
        [HttpDelete("removeNotification")]
        public IActionResult RemoveNotification([FromBody] int notificationId)
        {
            var loan = _loanRep.GetItem(notificationId);
            if (loan == null)
                return BadRequest(new { error = "Notification not exists", isSuccess = false });

            try
            {
                _loanRep.Delete(notificationId);
                _loanRep.Save();
                return Ok(new { isSuccess = true });
            }
            catch
            {
                return BadRequest(new { error = "Error while deleting", isSuccess = false });
            }
        }
    }
}
