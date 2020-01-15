using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public static class JTokenExtensions
    {
        public static void WalkNodes(this JToken node, Action<JToken> jObjectPreAction, Action<JToken> jObjectPostAction, Action<JToken> jTokenAction)
        {
            switch (node.Type)
            {
                case JTokenType.Object:
                    {
                        jObjectPreAction?.Invoke(node);

                        foreach (var child in node.Children<JProperty>())
                        {
                            child.Value.WalkNodes(jObjectPreAction, jObjectPostAction, jTokenAction);
                        }

                        jObjectPostAction?.Invoke(node);

                        break;
                    }
                case JTokenType.Array:
                    {
                        jObjectPreAction?.Invoke(node);

                        foreach (var child in node.Children())
                        {
                            child.WalkNodes(jObjectPreAction, jObjectPostAction, jTokenAction);
                        }

                        jObjectPostAction?.Invoke(node);
                    }
                    break;
                default:
                    {
                        jTokenAction?.Invoke(node);
                        break;
                    }
            }
        }
    }
}
