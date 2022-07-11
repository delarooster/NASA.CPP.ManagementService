using VOYG.CPP.Management.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace VOYG.CPP.Management.Api.StatusCodeHandlers
{
    public abstract class StatusCodeHandlerBase : IStatusCodeHandler
    {
        public int StatusCode { get; set; }

        public IActionResult HandleReponse(object message)
        {
            var response = new ContentResult
            {
                Content = JsonConvert.SerializeObject(message),
                ContentType = "application/json",
                StatusCode = StatusCode
            };

            return response;
        }
    }
}