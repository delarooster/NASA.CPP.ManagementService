using VOYG.CPP.Management.Api.Models;
using VOYG.CPP.Management.Api.Models.Responses.Status;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Services.Interfaces
{
    public interface IStatusService
    {
        Task<IServiceResult<StatusesResponse>> GetStatuses(int? limit, int? offset, string? deviceId, string? deploymentId, CancellationToken cancellationToken);
    }
}
