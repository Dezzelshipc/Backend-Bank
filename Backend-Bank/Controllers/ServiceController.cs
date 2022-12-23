using Database.Interfaces;
using Database.Logic;
using Database.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Bank.Controllers
{
    [Route("api/v1/organisation")]
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _serRep;
        private readonly IOrganisationsRepository _orgRep;

        public ServiceController(IServiceRepository serRep, IOrganisationsRepository orgRep)
        {
            _serRep = serRep;
            _orgRep = orgRep;
        }

        [HttpPost("addService")]
        public IActionResult AddBranch(string access_token, string serviceName, string description, string percent, string minLoanPeriod, string maxLoanPeriod, bool isOnline)
        {
            var token = TokenManager.ValidateToken(access_token);

            if (token == null || token.Type == true)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Organisation? organisation = _orgRep.GetOrganisationByLogin(token.Value);

            if (organisation == null)
                return BadRequest(new { error = "Organisation not found.", isSuccess = false });

            Service service = new(organisation.Id, serviceName, description, percent, minLoanPeriod, maxLoanPeriod, isOnline);

            try
            {
                _serRep.Create(service);
                _serRep.Save();
                var id = _serRep.Find(service);
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

        [HttpPost("removeService")]
        public IActionResult RemoveBranch(string access_token, int serviceId)
        {
            var token = TokenManager.ValidateToken(access_token);

            if (token == null || token.Type == true)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Service? service = _serRep.GetItem(serviceId);

            if (service == null)
                return BadRequest(new { error = "Service not exists.", isSuccess = false });

            try
            {
                _serRep.Delete(serviceId);
                _serRep.Save();
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

        [HttpGet("/[controller]/all")]
        public IActionResult GetAll()
        {
            try
            {
                return Json(_serRep.GetAll());
            }
            catch
            {
                return BadRequest(new { error = "Error." });
            }
        }
    }
}
