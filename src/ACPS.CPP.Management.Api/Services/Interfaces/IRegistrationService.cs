using VOYG.CPP.Management.Api.Models;
using VOYG.CPP.Management.Api.Models.Requests.Registration;
using VOYG.CPP.Management.Api.Models.Responses.Registration;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Services.Interfaces
{
    public interface IRegistrationService
    {
        Task<IServiceResult<RegistrationResponse>> Register(string id, RegistrationRequest registrationRequest, CancellationToken cancellationToken);
        IServiceResult<LookupScopeResponse> LookupScope(string deviceId);
        IServiceResult<RegistrationStatusResponse> GetRegistrationStatus(string deviceId);
    }
}
