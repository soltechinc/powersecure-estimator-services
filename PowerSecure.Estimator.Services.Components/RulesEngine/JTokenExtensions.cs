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

                        foreach (var child in node.Children<JProperty>())
                        {
                            child.Value.WalkNodes(jObjectAction, jTokenAction);
                        }

                        break;
                    }
                case JTokenType.Array:
                    {
                        foreach (var child in node.Children())
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
        public static void DoubleWalkNodes(this JToken node, Action<JObject> jObjectPreAction, Action<JObject> jObjectPostAction, Action<JToken> jTokenAction)
        {
            switch (node.Type)
            {
                case JTokenType.Object:
                    {
                        jObjectPreAction((JObject)node);

                        foreach (var child in node.Children<JProperty>())
                        {
                            child.Value.DoubleWalkNodes(jObjectPreAction, jObjectPostAction, jTokenAction);
                        }

                        jObjectPostAction((JObject)node);

                        break;
                    }
                case JTokenType.Array:
                    {
                        foreach (var child in node.Children())
                        {
                            child.DoubleWalkNodes(jObjectPreAction, jObjectPostAction, jTokenAction);
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
