using VOYG.CPP.Management.Api.Models.Requests.Registration;
using VOYG.CPP.Management.Api.Models.Responses.Registration;
using VOYG.CPP.Management.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Controllers
{
    public class RegistrationController : BaseController
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost("/registration/{regId}")]
        [ProducesResponseType(typeof(RegistrationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(string regId, RegistrationRequest registrationRequest, CancellationToken cancellationToken)
        {
            return NegotiateResponse(await _registrationService.Register(regId, registrationRequest, cancellationToken));
        }

        [HttpGet("/lookupScope/{deviceId}")]
        [ProducesResponseType(typeof(LookupScopeResponse), StatusCodes.Status200OK)]
        public IActionResult LookupScope(string deviceId)
        {
            return NegotiateResponse(_registrationService.LookupScope(deviceId));
        }
        
        [HttpGet("/getDeviceLifecycleStatus/{deviceId}")]
        [ProducesResponseType(typeof(RegistrationStatusResponse), StatusCodes.Status200OK)]
        public IActionResult GetRegistrationStatus(string deviceId)
        {
            return NegotiateResponse(_registrationService.GetRegistrationStatus(deviceId));
        }
    }
}
