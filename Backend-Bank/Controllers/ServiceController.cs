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
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _serRep;
        private readonly IOrganisationsRepository _orgRep;

        public ServiceController(IServiceRepository serRep, IOrganisationsRepository orgRep)
        {
            _serRep = serRep;
            _orgRep = orgRep;
        }

        /// <summary>
        /// Adds service for current organistion
        /// </summary>
        /// <remarks>
        /// Requires Organisation Access token
        /// 
        /// Service must be unique by pair of (organisationId, serviceName)
        /// 
        /// Returns:
        /// 
        ///     { 
        ///         "serviceId": int
        ///     }
        ///     
        /// </remarks>
        [Authorize(Policy.OrgAccess)]
        [HttpPost("addService")]
        public IActionResult AddService([FromBody] ServiceData serviceData)
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
                    serviceId = id,
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
        /// Removes service for current organistion by id
        /// </summary>
        /// <remarks>
        /// Requires Organisation Access token
        /// </remarks>
        [Authorize(Policy.OrgAccess)]
        [HttpDelete("removeService")]
        public IActionResult RemoveService([FromBody] INT serviceId)
        {
            Service? service = _serRep.GetItem(serviceId.Id);

            if (service == null)
                return BadRequest(new { error = "Service not exists.", isSuccess = false });

            var login = User.FindFirstValue("Login");

            if (login == null)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Organisation? organisation = _orgRep.GetOrganisationByLogin(login);

            if (organisation == null)
                return BadRequest(new { error = "Organisation not found.", isSuccess = false });

            if (service.OrganisationId != organisation.Id)
                return BadRequest(new { error = "Service of different organisation.", isSuccess = false });

            try
            {
                _serRep.Delete(serviceId.Id);
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

        /// <summary>
        /// Return all services for current organistion
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
        ///             "description": str,
        ///             "percent": srt,
        ///             "minLoanPeriod": str,
        ///             "maxLoadPeriod": str,
        ///             "isOnline": bool
        ///         }
        ///     ]
        ///     
        /// </remarks>
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

        /// <summary>
        /// Changes online state of service for current organistion
        /// </summary>
        /// <remarks>
        /// Requires Organisation Access token
        /// 
        /// Returns:
        /// 
        ///     {
        ///         "state": bool (changed)
        ///     }
        ///     
        /// 
        /// </remarks>
        [Authorize(Policy.OrgAccess)]
        [HttpPost("changeOnlineService")]
        public IActionResult ChangeOnlineServices([FromBody] INT serviceId)
        {
            var login = User.FindFirstValue("Login");
            if (login == null)
                return BadRequest(new { error = "Invalid token.", isSuccess = false });

            Organisation? organisation = _orgRep.GetOrganisationByLogin(login);
            if (organisation == null)
                return BadRequest(new { error = "Organisation not found.", isSuccess = false });

            Service? service = _serRep.GetItem(serviceId.Id);
            if (service == null)
                return BadRequest(new { error = "Service not found.", isSuccess = false });

            if (service.OrganisationId != organisation.Id)
                return BadRequest(new { error = "Service of different organisation", isSuccess = false });

            service.IsOnline = !service.IsOnline;

            try
            {
                _serRep.Update(service);
                _serRep.Save();
                return Ok(new
                {
                    state = service.IsOnline,
                    isSuccess = true
                });
            }
            catch
            {
                return BadRequest(new
                {
                    error = "Error while savinfg.",
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