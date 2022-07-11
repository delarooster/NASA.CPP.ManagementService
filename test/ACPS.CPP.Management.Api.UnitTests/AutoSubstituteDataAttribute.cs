using AutoFixture;
using AutoFixture.Xunit2;

namespace VOYG.CPP.Management.Api.UnitTests
{
    public class AutoSubstituteDataAttribute : AutoDataAttribute
    {
        public AutoSubstituteDataAttribute()
            : base(() => new Fixture().Customize(new AutoFixture.AutoNSubstitute.AutoNSubstituteCustomization { ConfigureMembers = true }))
        {
        }
    }
}