using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public static class JTokenExtensions
    {
        public static void WalkNodes(this JToken node, Action<JObject> jObjectAction, Action<JToken> jTokenAction)
        {
            switch (node.Type)
            {
                case JTokenType.Object:
                    {
                        jObjectAction((JObject)node);

                        foreach (JProperty child in node.Children<JProperty>())
                        {
                            child.Value.WalkNodes(jObjectAction, jTokenAction);
                        }
                        break;
                    }
                case JTokenType.Array:
                    {
                        foreach (JToken child in node.Children())
                        {
                            child.WalkNodes(jObjectAction, jTokenAction);
                        }
                    }
                    break;
                default:
                    {
                        jTokenAction(node);
                        break;
                    }
            }
        }
    }
}
