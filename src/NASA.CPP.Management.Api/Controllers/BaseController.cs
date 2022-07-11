using VOYG.CPP.Management.Api.Factories;
using VOYG.CPP.Management.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Mime;

namespace VOYG.CPP.Management.Api.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult NegotiateResponse<T>(IServiceResult<T> serviceResult)
        {
            if (serviceResult == null)
            {
                throw new NullReferenceException();
            }

            if (serviceResult.IsInError)
            {
                return StatusCodeHandlerFactory.GetResponseHandler(serviceResult).HandleReponse(serviceResult.ErrorResponse.Errors);
            }

            if (serviceResult.Result == null)
            {
                throw new NullReferenceException();
            }

            return StatusCodeHandlerFactory.GetResponseHandler(serviceResult).HandleReponse(serviceResult.Result);
        }
    }
}