using VOYG.CPP.Management.Api.Models;
using VOYG.CPP.Management.Api.Models.Requests.Commands;
using VOYG.CPP.Management.Api.Models.Responses.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Services.Interfaces
{
    public interface ICommandsService
    {
        Task<IServiceResult<InvokeCommandResponse>> InvokeSetFileUpload(InvokeCommandRequest invokeCommandRequest, CancellationToken cancellationToken);
    }
}
