using VOYG.CPP.Management.Api.Models;
using VOYG.CPP.Management.Api.Models.Requests.SetFileUploads;
using VOYG.CPP.Management.Api.Models.Responses.SetFileUploads;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Services.Interfaces
{
    public interface ISetFileUploadsService
    {
        Task<IServiceResult<PostSetFileUploadsResponse>> InvokeSetFileUpload(PostSetFileUploadsRequest postSetFileUploadsRequest, CancellationToken cancellationToken);
    }
}
