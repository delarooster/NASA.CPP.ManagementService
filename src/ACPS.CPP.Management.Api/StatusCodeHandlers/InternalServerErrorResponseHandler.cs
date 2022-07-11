using Microsoft.AspNetCore.Http;

namespace VOYG.CPP.Management.Api.StatusCodeHandlers
{
    public class InternalServerErrorResponseHandler : StatusCodeHandlerBase
    {
        public InternalServerErrorResponseHandler()
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}