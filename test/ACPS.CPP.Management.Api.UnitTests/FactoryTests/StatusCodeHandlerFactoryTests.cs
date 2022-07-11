using VOYG.CPP.Management.Api.Factories;
using VOYG.CPP.Management.Api.Helpers;
using VOYG.CPP.Management.Api.StatusCodeHandlers;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Xunit;

namespace VOYG.CPP.Management.Api.UnitTests.FactoryTests
{
    public class StatusCodeHandlerFactoryTests
    {
        [Fact]
        public void GetResponseHandler_WhenInErrorAndStatusCodeFoundAndIsBadRequest_ReturnsBadRequestError()
        {
            var serviceResult = ResponseHelper.UnsuccessfulResult<BadRequestResponseHandler>(new Dictionary<string, string>(), StatusCodes.Status400BadRequest);
            var result = StatusCodeHandlerFactory.GetResponseHandler(serviceResult);

            Assert.IsType<BadRequestResponseHandler>(result);
        }

        [Fact]
        public void GetResponseHandler_WhenInErrorAndStatusCodeNotFound_ReturnsInternalServerError()
        {
            var serviceResult = ResponseHelper.UnsuccessfulResult<InternalServerErrorResponseHandler>(new Dictionary<string, string>());
            var result = StatusCodeHandlerFactory.GetResponseHandler(serviceResult);

            Assert.IsType<InternalServerErrorResponseHandler>(result);
        }

        [Fact]
        public void GetResponseHandler_WhenOK_ReturnsOkResponse()
        {
            var serviceResult = ResponseHelper.SuccessfulResult("success");
            var result = StatusCodeHandlerFactory.GetResponseHandler(serviceResult);

            Assert.IsType<OkResponseHandler>(result);
        }
    }
}