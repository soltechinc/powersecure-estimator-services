using Newtonsoft.Json.Linq;

namespace PowerSecure.Estimator.Services
{
    public static class JObjectExtensions
    {
        public static JObject UpdateKeyWithValue(this JObject document, string key, object value)
        {
            if (value == null)
            {
                value = 0;
            }
            
            var valueJToken = JToken.FromObject(value.ToString().UnwrapString());

            if (document.ContainsKey(key))
            {
                document[key] = valueJToken;
            }
            else
            {
                document.Add(key, valueJToken);
            }

            return document;
        }
    }
}
