using VOYG.CPP.Management.Api.StatusCodeHandlers;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace VOYG.CPP.Management.Api.UnitTests.StatusCodeHandlerTests
{
    public class InternalServerErrorResponseHandlerTests
    {
        [Fact]
        public void InternalServerErrorResponseHandler_Returns_ExpectedCode()
        {
            var expected = StatusCodes.Status500InternalServerError;

            var sut = new InternalServerErrorResponseHandler();

            Assert.Equal(expected, sut.StatusCode);
        }
    }
}