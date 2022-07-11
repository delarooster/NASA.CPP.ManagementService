using AutoFixture.Xunit2;

namespace VOYG.CPP.Management.Api.UnitTests
{
    /// <summary>
    /// Used to create parameterized repeatable tests using autodata and inline data. Note that the inline data must be the first parameters of the test method.
    /// </summary>
    public class InlineAutoSubstituteDataAttribute : InlineAutoDataAttribute
    {
        public InlineAutoSubstituteDataAttribute(params object[] objects)
            : base(new AutoSubstituteDataAttribute(), objects)
        {
        }
    }
}