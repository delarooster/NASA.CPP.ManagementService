using System.Collections.Generic;
using System.Linq;

namespace VOYG.CPP.Management.Api.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return !collection?.Any() ?? true;
        }
    }
}
