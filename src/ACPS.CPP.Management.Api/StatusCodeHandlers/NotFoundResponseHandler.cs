using Microsoft.AspNetCore.Http;

namespace VOYG.CPP.Management.Api.StatusCodeHandlers
{
    public class NotFoundResponseHandler : StatusCodeHandlerBase
    {
        public NotFoundResponseHandler()
        {
            StatusCode = StatusCodes.Status404NotFound;
        }
    }
}
