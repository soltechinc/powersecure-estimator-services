using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
