using Microsoft.AspNetCore.Http;

namespace VOYG.CPP.Management.Api.StatusCodeHandlers
{
    public class BadRequestResponseHandler : StatusCodeHandlerBase
    {
        public BadRequestResponseHandler()
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}