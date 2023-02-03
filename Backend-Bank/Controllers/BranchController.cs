using Backend_Bank.Requirements;
using Database.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [Authorize(Policy.OrgAccess)]
        [HttpPost("addBranch")]
        public IActionResult AddBranch([FromBody] BranchData branchData)
        {
            if (branchData == null || branchData.IsNotValid())
                return BadRequest(new { error = "Invalid input.", isSuccess = false });

            var login = User.FindFirstValue("Login");

            if (login == null)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Organisation? organisation = _orgRep.GetOrganisationByLogin(login);

            if (organisation == null)
                return BadRequest(new { error = "Organisation not found.", isSuccess = false });

            Branch branch = new(organisation.Id, branchData.BranchName, branchData.BranchAddress,
                branchData.PhoneNumber, branchData.Coordinates.Longtitude, branchData.Coordinates.Lattitude);

            try
            {
                _brRep.Create(branch);
                _brRep.Save();
                var id = _brRep.Find(branch);
                return Json(new
                {
                    branchId = id,
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

        [Authorize(Policy.OrgAccess)]
        [HttpDelete("removeBranch")]
        public IActionResult RemoveBranch([FromBody] int branchId)
        {
            Branch? branch = _brRep.GetItem(branchId);

            if (branch == null)
                return BadRequest(new { error = "Branch not exists.", isSuccess = false });

            try
            {
                _brRep.Delete(branchId);
                _brRep.Save();
                return Json(new { isSuccess = true });
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

        [Authorize(Policy.OrgAccess)]
        [HttpGet("getBranches")]
        public IActionResult GetBranches()
        {
            var login = User.FindFirstValue("Login");

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
                return BadRequest(new { error = "Error.", isSuccess = false });
            }
        }
    }
}