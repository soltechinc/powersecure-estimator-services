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

            if (document.ContainsKey(key))
            {
                document[key] = JToken.FromObject(value);
            }
            else
            {
                document.Add(key, JToken.FromObject(value));
            }

            return document;
        }
    }
}
