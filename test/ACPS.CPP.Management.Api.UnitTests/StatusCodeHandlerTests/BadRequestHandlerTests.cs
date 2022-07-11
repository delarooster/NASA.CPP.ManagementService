using VOYG.CPP.Management.Api.StatusCodeHandlers;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace VOYG.CPP.Management.Api.UnitTests.StatusCodeHandlerTests
{
    public class BadRequestHandlerTests
    {
        [Fact]
        public void BadRequestResponseHandler_Returns_ExpectedCode()
        {
            var expected = StatusCodes.Status400BadRequest;

            var sut = new BadRequestResponseHandler();

            Assert.Equal(expected, sut.StatusCode);
        }
    }
}