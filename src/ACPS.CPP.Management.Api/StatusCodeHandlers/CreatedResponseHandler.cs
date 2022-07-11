using Microsoft.AspNetCore.Http;

namespace VOYG.CPP.Management.Api.StatusCodeHandlers
{
    public class CreatedResponseHandler : StatusCodeHandlerBase
    {
        public CreatedResponseHandler()
        {
            StatusCode = StatusCodes.Status201Created;
        }
    }
}