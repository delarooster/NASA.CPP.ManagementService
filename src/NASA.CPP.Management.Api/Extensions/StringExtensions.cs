using System.Text.RegularExpressions;

namespace VOYG.CPP.Management.Api.Extensions
{
    public static class StringExtensions
    {
        private const string DetectNumbersRegex = @"\d+";

        public static int GetNumberFromString(this string input)
        {
            var match = Regex.Match(input, DetectNumbersRegex);
            return int.Parse(match.Value);
        }

        public static string RemoveNumbersFromString(this string input)
        {
            var result = Regex.Replace(input, DetectNumbersRegex, "");
            return result;
        }
    }
}
