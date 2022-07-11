using VOYG.CPP.Management.Api.Models.Requests.SetFileUploads;
using VOYG.CPP.Management.Api.Models.Responses.SetFileUploads;
using VOYG.CPP.Management.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Controllers
{
    public class SetFileUploadsController : BaseController
    {
        private readonly ISetFileUploadsService _setFileUploadsService;

        public SetFileUploadsController(ISetFileUploadsService setFileUploadsService)
        {
            _setFileUploadsService = setFileUploadsService;
        }

        [HttpPost("/setFileUploads/")]
        [ProducesResponseType(typeof(PostSetFileUploadsResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> InvokeSetFileUpload(PostSetFileUploadsRequest postSetFileUploadsRequest, CancellationToken cancellationToken)
        {
            return NegotiateResponse(await _setFileUploadsService.InvokeSetFileUpload(postSetFileUploadsRequest, cancellationToken));
        }
    }
}
