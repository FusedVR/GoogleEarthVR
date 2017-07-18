using System.Text.RegularExpressions;

namespace Wrld.Scripts.Utilities
{
    public static class APIKeyPrevalidator
    {
        public static bool AppearsValid(string apiKey)
        {
            return Regex.IsMatch(apiKey, "^[a-f0-9]{32}$");
        }
    }
}

