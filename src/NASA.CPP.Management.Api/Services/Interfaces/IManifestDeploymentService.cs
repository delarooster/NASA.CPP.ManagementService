using VOYG.CPP.Management.Api.Models;
using VOYG.CPP.Management.Api.Models.Requests.ManifestDeployment;
using VOYG.CPP.Management.Api.Models.Responses.ManifestDeployment;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Services.Interfaces
{
    public interface IManifestDeploymentService
    {
        Task<IServiceResult<ManifestDeploymentResponse>> RegisterManifestDeployment(
            PostRegisterManifestDeploymentRequest postRegisterManifestDeploymentRequest,
            CancellationToken cancellationToken);
        Task<IServiceResult<ManifestDeploymentsResponse>> GetManifestDeployments(int limit, int offset, string? deviceId, CancellationToken cancellationToken);
        Task<IServiceResult<ManifestDeploymentResponse>> GetManifestDeployment(int id, CancellationToken cancellationToken);
    }
}
