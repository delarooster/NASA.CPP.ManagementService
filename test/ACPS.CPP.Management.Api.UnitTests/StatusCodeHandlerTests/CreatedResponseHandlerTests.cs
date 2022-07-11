using VOYG.CPP.Management.Api.StatusCodeHandlers;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace VOYG.CPP.Management.Api.UnitTests.StatusCodeHandlerTests
{
    public class CreatedResponseHandlerTests
    {
        [Fact]
        public void CreateResponseHandler_Returns_ExpectedCode()
        {
            var expected = StatusCodes.Status201Created;

            var sut = new CreatedResponseHandler();

            Assert.Equal(expected, sut.StatusCode);
        }
    }
}