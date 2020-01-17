using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public static class IInstructionSetMixin
    {
        private class EvaluationNode
        {
            public EvaluationNode Parent { get; set; }
            public object Value { get; set; }
        }

        private class PrimitiveValuePair
        {
            public IPrimitive Primitive { get; set; }
            public IList<EvaluationNode> Children { get; set; }
        }

        public static object Evaluate(this IInstructionSet instructionSet, IDictionary<string, object> parameters, IDictionary<string, IPrimitive> primitives, IReferenceDataRepository referenceDataRepository)
        {
            EvaluationNode rootNode = null;
            EvaluationNode currentNode = null;

            JObject.Parse(instructionSet.Instructions).WalkNodes(
            PreOrder: jToken =>
                {
                    if (!(jToken.Type == JTokenType.Object || (jToken.Type == JTokenType.Array && jToken.Parent.Type == JTokenType.Array)))
                    {
                        return;
                    }

                    var node = new EvaluationNode() { Parent = currentNode };
                    if (currentNode != null)
                    {
                        ((PrimitiveValuePair)currentNode.Value).Children.Add(node);
                    }
                    if (rootNode == null)
                    {
                        rootNode = node;
                    }
                    currentNode = node;

                    switch (jToken)
                    {
                        case JObject jObject:
                            {
                                var primitive = primitives[jObject.Properties().Select(p => p.Name.ToLower()).First()];
                                node.Value = new PrimitiveValuePair() { Primitive = primitive, Children = new List<EvaluationNode>() };
                                break;
                            }
                        case JToken j when j.Type == JTokenType.Array && j.Parent.Type == JTokenType.Array:
                            {
                                node.Value = new PrimitiveValuePair() { Primitive = null, Children = new List<EvaluationNode>() };
                                break;
                            }
                    }
                },
            PostOrder: jToken =>
                {
                    if (!(jToken.Type == JTokenType.Object || (jToken.Type == JTokenType.Array && jToken.Parent.Type == JTokenType.Array)))
                    {
                        return;
                    }

                    var node = currentNode;

                    switch (jToken)
                    {
                        case JObject jObject:
                            {
                                var pair = (PrimitiveValuePair)node.Value;
                                node.Value = pair.Primitive.Invoke(pair.Children.Select(p => p.Value).ToArray(), referenceDataRepository);
                                break;
                            }
                        case JToken j when j.Type == JTokenType.Array && j.Parent.Type == JTokenType.Array:
                            {
                                var pair = (PrimitiveValuePair)node.Value;
                                node.Value = pair.Children.Select(p => p.Value).ToArray();
                                break;
                            }
                    }

                    currentNode = node.Parent;
                },
            Visit: jToken =>
                {
                    var node = new EvaluationNode() { Parent = currentNode };
                    var pair = (PrimitiveValuePair)currentNode.Value;
                    pair.Children.Add(node);

                    if (jToken.Type == JTokenType.String)
                    {
                        string value = jToken.ToString();
                        if (value.StartsWith('$')) //these are string literals
                        {
                            node.Value = value;
                        }
                        else //these should be resolved against the parameters
                        {
                            string key = jToken.ToString().ToLower();
                            if(parameters[key] is IInstructionSet childInstructionSet)
                            {
                                parameters[key] = childInstructionSet.Evaluate(parameters, primitives, referenceDataRepository);
                            }
                            node.Value = parameters[jToken.ToString().ToLower()];
                        }
                    }
                    else if (jToken.Type == JTokenType.Null)
                    {
                        node.Value = null;
                    }
                    else
                    {
                        node.Value = decimal.Parse(jToken.ToString());
                    }
                }
            );

            return rootNode.Value;
        }
    }
}
