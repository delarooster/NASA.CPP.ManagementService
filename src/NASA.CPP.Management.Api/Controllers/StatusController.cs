using VOYG.CPP.Management.Api.Models.Responses.Status;
using VOYG.CPP.Management.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Controllers
{
    public class StatusController : BaseController
    {
        private readonly IStatusService _statusService;

        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }

        [HttpGet("/statuses/")]
        [ProducesResponseType(typeof(StatusesResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStatuses(int? limit, int? offset, string? deviceId, string? deploymentId, CancellationToken cancellationToken)
        {
            return NegotiateResponse(await _statusService.GetStatuses(limit ?? 10, offset ?? 0, deviceId, deploymentId, cancellationToken));
        }
    }
}
