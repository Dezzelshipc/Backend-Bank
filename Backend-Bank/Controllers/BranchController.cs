using Backend_Bank.Token;
using Database.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Bank.Controllers
{
    [Route("api/v1/organisation")]
    public class BranchController : Controller
    {
        private readonly IBranchesRepository _brRep;
        private readonly IOrganisationsRepository _orgRep;

        public BranchController(IBranchesRepository brRep, IOrganisationsRepository orgRep)
        {
            _brRep = brRep;
            _orgRep = orgRep;
        }

        [Authorize]
        [HttpPost("addBranch")]
        public IActionResult AddBranch(string branchName, string branchAddress, string phoneNumber)
        {
            if (!User.Claims.CheckClaim())
                return BadRequest(new { error = "Invalid token. Required access", isSuccess = 0 });

            var login = User.Claims.GetClaim("Login");

            if (login == null)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Organisation? organisation = _orgRep.GetOrganisationByLogin(login);

            if (organisation == null)
                return BadRequest(new { error = "Organisation not found.", isSuccess = false });

            Branch branch = new(organisation.Id, branchName, branchAddress, phoneNumber);

            try
            {
                _brRep.Create(branch);
                _brRep.Save();
                var id = _brRep.Find(branch);
                return Json(new
                {
                    branchId = id,
                    error = "",
                    isSuccess = true
                });
            }
            catch
            {
                return BadRequest(new
                {
                    error = "Error while creating.",
                    isSuccess = false
                });
            }
        }

        [Authorize]
        [HttpDelete("removeBranch")]
        public IActionResult RemoveBranch(int branchId)
        {
            if (!User.Claims.CheckClaim())
                return BadRequest(new { error = "Invalid token. Required access", isSuccess = 0 });

            Branch? branch = _brRep.GetItem(branchId);

            if (branch == null)
                return BadRequest(new { error = "Branch not exists.", isSuccess = false });

            try
            {
                _brRep.Delete(branchId);
                _brRep.Save();
                return Json(new
                {
                    error = "",
                    isSuccess = true
                });
            }
            catch
            {
                return BadRequest(new
                {
                    error = "Error while deleting.",
                    isSuccess = false
                });
            }
        }

        [Authorize]
        [HttpGet("getBranches")]
        public IActionResult GetBranches()
        {
            if (!User.Claims.CheckClaim())
                return BadRequest(new { error = "Invalid token. Required access", isSuccess = 0 });

            var login = User.Claims.GetClaim("Login");

            if (login == null)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Organisation? organisation = _orgRep.GetOrganisationByLogin(login);

            if (organisation == null)
                return BadRequest(new { error = "Organisation not found.", isSuccess = false });

            try
            {
                return Json(_brRep.GetBranches(organisation.Id));
            }
            catch
            {
                return BadRequest(new
                {
                    error = "Error while getting.",
                    isSuccess = false
                });
            }
        }

        [HttpOptions("/api/[controller]/all")]
        public IActionResult GetAll()
        {
            try
            {
                return Json(_brRep.GetAll());
            }
            catch
            {
                return BadRequest(new { error = "Error." });
            }
        }
    }
}