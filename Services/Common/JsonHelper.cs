using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ABC.Leaves.Api.Services
{
    public static class JsonHelper
    {
        public static string GetPropertyValue(string json, string propertyName)
        {
            try
            {
                var jsonObject = JObject.Parse(json);
                return (string)jsonObject[propertyName];
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}