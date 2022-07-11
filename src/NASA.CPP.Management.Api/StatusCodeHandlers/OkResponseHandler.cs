using Microsoft.AspNetCore.Http;

namespace VOYG.CPP.Management.Api.StatusCodeHandlers
{
    public class OkResponseHandler : StatusCodeHandlerBase
    {
        public OkResponseHandler()
        {
            StatusCode = StatusCodes.Status200OK;
        }
    }
}