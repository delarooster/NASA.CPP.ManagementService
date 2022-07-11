using VOYG.CPP.Management.Api.Models.Requests.ManifestDeployment;
using VOYG.CPP.Management.Api.Models.Responses.ManifestDeployment;
using VOYG.CPP.Management.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Controllers
{
    public class ManifestDeploymentController : BaseController
    {
        private readonly IManifestDeploymentService _manifestDeploymentService;

        public ManifestDeploymentController(IManifestDeploymentService manifestDeploymentService)
        {
            _manifestDeploymentService = manifestDeploymentService;
        }

        [HttpPost("/manifestDeployments/")]
        [ProducesResponseType(typeof(ManifestDeploymentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegisterManifestDeployment(PostRegisterManifestDeploymentRequest postRegisterManifestDeploymentRequest, CancellationToken cancellationToken)
        {
            return NegotiateResponse(await _manifestDeploymentService.RegisterManifestDeployment(postRegisterManifestDeploymentRequest, cancellationToken));
        }

        [HttpGet("/manifestDeployments/")]
        [ProducesResponseType(typeof(ManifestDeploymentsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDeployments(int? limit, int? offset, [FromQuery(Name = "device")] string? deviceId, CancellationToken cancellationToken)
        {
            return NegotiateResponse(await _manifestDeploymentService.GetManifestDeployments(limit ?? 10, offset ?? 0, deviceId, cancellationToken));
        }

        [HttpGet("/manifestDeployments/{id}/")]
        [ProducesResponseType(typeof(ManifestDeploymentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDeployment(int id, CancellationToken cancellationToken)
        {
            return NegotiateResponse(await _manifestDeploymentService.GetManifestDeployment(id, cancellationToken));
        }
    }
}
