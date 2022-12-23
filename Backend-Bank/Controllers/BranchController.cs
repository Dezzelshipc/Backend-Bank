using Database.Interfaces;
using Database.Logic;
using Database.Models;
using Database.Repository;
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

        [HttpPost("addBranch")]
        public IActionResult AddBranch(string access_token, string branchName, string branchAddress, string phoneNumber)
        {
            var token = TokenManager.ValidateToken(access_token);

            if (token == null || token.Type == true)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Organisation? organisation = _orgRep.GetOrganisationByLogin(token.Value);

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

        [HttpPost("removeBranch")]
        public IActionResult RemoveBranch(string access_token, int branchId)
        {
            var token = TokenManager.ValidateToken(access_token);

            if (token == null || token.Type == true)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

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

        [HttpPost("getBranches")]
        public IActionResult GetBranches(string access_token)
        {
            var token = TokenManager.ValidateToken(access_token);

            if (token == null || token.Type == true)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Organisation? organisation = _orgRep.GetOrganisationByLogin(token.Value);

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

        [HttpGet("/[controller]/all")]
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
