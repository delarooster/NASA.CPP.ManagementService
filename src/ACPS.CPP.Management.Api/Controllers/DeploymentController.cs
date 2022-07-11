using VOYG.CPP.Management.Api.Models.Requests.Deployment;
using VOYG.CPP.Management.Api.Models.Responses.Deployment;
using VOYG.CPP.Management.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Controllers
{
    public class DeploymentController : BaseController
    {
        private readonly IDeploymentService _deploymentService;

        public DeploymentController(IDeploymentService deploymentService)
        {
            _deploymentService = deploymentService;
        }

        [HttpPost("/deployments/")]
        [ProducesResponseType(typeof(DeploymentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterDeployment(RegisterDeploymentRequest registerDeploymentRequest, CancellationToken cancellationToken)
        {
            return NegotiateResponse(await _deploymentService.RegisterDeployment(registerDeploymentRequest, cancellationToken));
        }

        [HttpGet("/deployments/")]
        [ProducesResponseType(typeof(DeploymentsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDeployments(int? limit, int? offset, [FromQuery(Name = "device")] IEnumerable<string> deviceIds, CancellationToken cancellationToken)
        {
            return NegotiateResponse(await _deploymentService.GetDeployments(limit ?? 10, offset ?? 0, deviceIds, cancellationToken));
        }

        [HttpGet("/deployments/{id}/")]
        [ProducesResponseType(typeof(DeploymentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDeployment(int id, CancellationToken cancellationToken)
        {
            return NegotiateResponse(await _deploymentService.GetDeployment(id, cancellationToken));
        }

        [HttpPatch("/deployments/{id}/")]
        [ProducesResponseType(typeof(UpdateStatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusRequest updateStatusRequest, CancellationToken cancellationToken)
        {
            return NegotiateResponse(await _deploymentService.UpdateStatus(id, updateStatusRequest, cancellationToken));
        }
    }
}
