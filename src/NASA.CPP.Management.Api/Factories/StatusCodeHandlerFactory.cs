using VOYG.CPP.Management.Api.Interfaces;
using VOYG.CPP.Management.Api.Models;
using VOYG.CPP.Management.Api.StatusCodeHandlers;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace VOYG.CPP.Management.Api.Factories
{
    public static class StatusCodeHandlerFactory
    {
        private static readonly Dictionary<int, IStatusCodeHandler> _statusCodeHandlers = new Dictionary<int, IStatusCodeHandler>();

        static StatusCodeHandlerFactory()
        {
            _statusCodeHandlers.Add(StatusCodes.Status200OK, new OkResponseHandler());
            _statusCodeHandlers.Add(StatusCodes.Status201Created, new CreatedResponseHandler());
            _statusCodeHandlers.Add(StatusCodes.Status400BadRequest, new BadRequestResponseHandler());
            _statusCodeHandlers.Add(StatusCodes.Status404NotFound, new NotFoundResponseHandler());
            _statusCodeHandlers.Add(StatusCodes.Status500InternalServerError, new InternalServerErrorResponseHandler());
        }

        public static IStatusCodeHandler GetResponseHandler<T>(IServiceResult<T> result)
        {
            if (result.IsInError)
            {
                if (!_statusCodeHandlers.ContainsKey(result.StatusCode))
                {
                    return new InternalServerErrorResponseHandler();
                }

                return _statusCodeHandlers[result.StatusCode];
            }

            if (!_statusCodeHandlers.ContainsKey(result.StatusCode))
            {
                return new OkResponseHandler();
            }

            return _statusCodeHandlers[result.StatusCode];
        }
    }
}