using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace powersecure_instruction_set_engine
{
    public static class JTokenExtensions
    {
        public static void WalkNodes(this JToken node, Action<JObject> action)
        {
            switch (node.Type)
            {
                case JTokenType.Object:
                    {
                        action((JObject)node);

                        foreach (JProperty child in node.Children<JProperty>())
                        {
                            child.Value.WalkNodes(action);
                        }
                        break;
                    }
                case JTokenType.Array:
                    {
                        foreach (JToken child in node.Children())
                        {
                            child.WalkNodes(action);
                        }
                    }
                    break;
                default: { break; }
            }
        }
    }
}
