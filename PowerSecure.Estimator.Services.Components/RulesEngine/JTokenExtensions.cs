using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public static class JTokenExtensions
    {
        public static void WalkNodes(this JToken node, Action<JToken> PreOrder = null, Action<JToken> Visit = null, Action<JToken> PostOrder = null)
        {
            switch (node.Type)
            {
                case JTokenType.Object:
                    {
                        PreOrder?.Invoke(node);

                        foreach (var child in node.Children<JProperty>())
                        {
                            child.Value.WalkNodes(PreOrder, Visit, PostOrder);
                        }

                        PostOrder?.Invoke(node);

                        break;
                    }
                case JTokenType.Array:
                    {
                        PreOrder?.Invoke(node);

                        foreach (var child in node.Children())
                        {
                            child.WalkNodes(PreOrder, Visit, PostOrder);
                        }

                        PostOrder?.Invoke(node);
                    }
                    break;
                default:
                    {
                        Visit?.Invoke(node);
                        break;
                    }
            }
        }

        public static IDictionary<string, object> ToDictionary(this JObject jObject)
        {
            return (IDictionary<string, object>)ToCollection(jObject);
        }

        private static object ToCollection(object obj)
        {
            switch (obj)
            {
                case JObject jo:
                    return jo.ToObject<IDictionary<string, object>>().ToDictionary(k => k.Key, v => ToCollection(v.Value));
                case JArray ja:
                    return ja.ToObject<List<object>>().Select(ToCollection).ToList();
            }
            return obj;
        }
    }
}
