using Backend_Bank.Requirements;
using Backend_Bank.Tokens;
using Database.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [Authorize(Policy.OrgAccess)]
        [HttpPost("addService")]
        public IActionResult AddBranch([FromBody] ServiceData serviceData)
        {
            if (serviceData == null || serviceData.IsNotValid())
                return BadRequest(new { error = "Invalid input.", isSuccess = false });

            var login = User.FindFirstValue("Login");

            if (login == null)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Organisation? organisation = _orgRep.GetOrganisationByLogin(login);

            if (organisation == null)
                return BadRequest(new { error = "Organisation not found.", isSuccess = false });

            Service service = new(organisation.Id, serviceData.ServiceName, serviceData.Description,
                serviceData.Percent, serviceData.MinLoanPeriod, serviceData.MaxLoanPeriod, serviceData.IsOnline);

            try
            {
                _serRep.Create(service);
                _serRep.Save();
                var id = _serRep.Find(service);
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
        [HttpDelete("removeService")]
        public IActionResult RemoveBranch([FromBody] int serviceId)
        {
            Service? service = _serRep.GetItem(serviceId);

            if (service == null)
                return BadRequest(new { error = "Service not exists.", isSuccess = false });

            try
            {
                _serRep.Delete(serviceId);
                _serRep.Save();
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
        [HttpGet("getServices")]
        public IActionResult GetServices()
        {
            var login = User.FindFirstValue("Login");

            if (login == null)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Organisation? organisation = _orgRep.GetOrganisationByLogin(login);

            if (organisation == null)
                return BadRequest(new { error = "Organisation not found.", isSuccess = false });

            try
            {
                return Json(_serRep.GetServices(organisation.Id));
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
                return Json(_serRep.GetAll());
            }
            catch
            {
                return BadRequest(new { error = "Error.", isSuccess = false });
            }
        }
    }
}