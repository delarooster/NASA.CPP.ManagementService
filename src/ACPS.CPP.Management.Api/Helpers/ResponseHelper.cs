using VOYG.CPP.Management.Api.Models;
using System.Collections.Generic;

namespace VOYG.CPP.Management.Api.Helpers
{
    public static class ResponseHelper
    {
        public static IServiceResult<T> SuccessfulResult<T>(T result)
        {
            return new ServiceResult<T>
            {
                IsInError = false,
                Result = result
            };
        }

        public static IServiceResult<T> SuccessfulResult<T>(T result, int statusCode)
        {
            return new ServiceResult<T>
            {
                IsInError = false,
                Result = result,
                StatusCode = statusCode
            };
        }

        public static IServiceResult<T> UnsuccessfulResult<T>(IDictionary<string, string> errors)
        {
            return new ServiceResult<T>
            {
                IsInError = true,
                ErrorResponse = new ErrorResponse
                {
                    Errors = errors
                }
            };
        }

        public static IServiceResult<T> UnsuccessfulResult<T>(IDictionary<string, string> errors, int statusCode)
        {
            return new ServiceResult<T>
            {
                IsInError = true,
                ErrorResponse = new ErrorResponse
                {
                    Errors = errors
                },
                StatusCode = statusCode
            };
        }
    }
}