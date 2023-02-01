using Backend_Bank.Token;
using Database.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPost("addService")]
        public IActionResult AddBranch(string serviceName, string description, string percent, string minLoanPeriod, string maxLoanPeriod, bool isOnline)
        {
            if (!User.Claims.CheckClaim())
                return BadRequest(new { error = "Invalid token. Required access", isSuccess = 0 });

            var login = User.Claims.GetClaim("Login");

            if (login == null)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Organisation? organisation = _orgRep.GetOrganisationByLogin(login);

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

        [Authorize]
        [HttpDelete("removeService")]
        public IActionResult RemoveBranch(int serviceId)
        {
            if (!User.Claims.CheckClaim())
                return BadRequest(new { error = "Invalid token. Required access", isSuccess = 0 });

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

        [Authorize]
        [HttpGet("getServices")]
        public IActionResult GetServices()
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
                return BadRequest(new { error = "Error." });
            }
        }
    }
}