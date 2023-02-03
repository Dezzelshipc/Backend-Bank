using Backend_Bank.Requirements;
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

        /// <summary>
        /// Adds branch for current organisation
        /// </summary>
        /// <param name="branchData"></param>
        /// <remarks>
        /// Requires Organisation Access token
        /// 
        /// Returns:
        /// 
        ///     { 
        ///         "branchId": int 
        ///     }
        /// 
        /// </remarks>
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
                branchData.PhoneNumber, branchData.Coordinates.Longtitude, branchData.Coordinates.Latitude);

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

        /// <summary>
        /// Removes branch of current organisation
        /// </summary>
        /// <param name="branchId"></param>
        /// <remarks>
        /// Requires Organisation Access token
        /// </remarks>
        [Authorize(Policy.OrgAccess)]
        [HttpDelete("removeBranch")]
        public IActionResult RemoveBranch([FromBody] INT branchId)
        {
            Branch? branch = _brRep.GetItem(branchId.Id);

            if (branch == null)
                return BadRequest(new { error = "Branch not exists.", isSuccess = false });

            var login = User.FindFirstValue("Login");

            if (login == null)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Organisation? organisation = _orgRep.GetOrganisationByLogin(login);

            if (organisation == null)
                return BadRequest(new { error = "Organisation not found.", isSuccess = false });

            if (branch.OrganisationId != organisation.Id)
                return BadRequest(new { error = "Branch of different organisation.", isSuccess = false });

            try
            {
                _brRep.Delete(branchId.Id);
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

        /// <summary>
        /// Returns branches of current organisation
        /// </summary>
        /// <remarks>
        /// Requires Organisation Access token
        /// 
        /// Returns: 
        /// 
        ///     [
        ///         {
        ///             "id": int,
        ///             "organisationId": int,
        ///             "name": str,
        ///             "address": str,
        ///             "phoneNumber": str,
        ///             "longtitude": double,
        ///             "latitude": double
        ///         }
        ///     ]
        ///     
        /// </remarks>
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