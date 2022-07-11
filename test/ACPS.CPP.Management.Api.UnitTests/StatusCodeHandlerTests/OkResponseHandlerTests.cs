using VOYG.CPP.Management.Api.StatusCodeHandlers;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace VOYG.CPP.Management.Api.UnitTests.StatusCodeHandlerTests
{
    public class OkResponseHandlerTests
    {
        [Fact]
        public void OkResponseHandler_Returns_ExpectedCode()
        {
            var expected = StatusCodes.Status200OK;

            var sut = new OkResponseHandler();

            Assert.Equal(expected, sut.StatusCode);
        }
    }
}