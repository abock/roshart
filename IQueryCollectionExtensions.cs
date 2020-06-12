using System.Globalization;

using Microsoft.AspNetCore.Http;

namespace Roshart
{
    static class IQueryCollectionExtensions
    {
        public static bool TryGetLastInt32(
            this IQueryCollection query,
            string key,
            out int value)
        {
            if (query.TryGetLastString(key, out var stringValue))
                return int.TryParse(
                    stringValue,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out value);
            
            value = 0;
            return false;
        }
        
        public static bool TryGetLastString(
            this IQueryCollection query,
            string key,
            out string value)
        {
            if (query.TryGetValue(key, out var values) && values.Count > 0) {
                value = values[values.Count - 1];
                return true;
            }

            value = string.Empty;
            return false;
        }
    }
}
