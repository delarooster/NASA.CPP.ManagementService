using VOYG.CPP.Management.Api.Models;
using VOYG.CPP.Management.Api.Models.Requests.Deployment;
using VOYG.CPP.Management.Api.Models.Responses.Deployment;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Services.Interfaces
{
    public interface IDeploymentService
    {
        Task<IServiceResult<DeploymentResponse>> RegisterDeployment(RegisterDeploymentRequest registerDeploymentRequest, CancellationToken cancellationToken);

        Task<IServiceResult<DeploymentResponse>> GetDeployment(int id, CancellationToken cancellationToken);

        Task<IServiceResult<DeploymentsResponse>> GetDeployments(int? limit, int? offset, IEnumerable<string> deviceIds, CancellationToken cancellationToken);

        Task<IServiceResult<UpdateStatusResponse>> UpdateStatus(int id, UpdateStatusRequest updateStatusRequest, CancellationToken cancellationToken);
    }
}
