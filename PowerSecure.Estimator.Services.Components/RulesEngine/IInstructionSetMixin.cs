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
        public static decimal Evaluate(this IInstructionSet instructionSet, IDictionary<string, string> parameters, IDictionary<string, IPrimitive> primitives, IReferenceDataRepository referenceDataRepository)
        {
            EvaluationNode rootNode = null;
            EvaluationNode currentNode = null;

            JObject.Parse(instructionSet.Instructions).DoubleWalkNodes(jToken =>
            {
                if (!(jToken.Type == JTokenType.Object || (jToken.Type == JTokenType.Array && jToken.Parent.Type == JTokenType.Array)))
                {
                    return;
                }

                var node = new EvaluationNode();
                node.Parent = currentNode;
                if (currentNode != null)
                {
                    ((Tuple<IPrimitive, List<EvaluationNode>>)currentNode.Value).Item2.Add(node);
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
                            var primitive = primitives[jObject.Properties().Select(p => p.Name).First()];
                            node.Value = Tuple.Create(primitive, new List<EvaluationNode>());
                            break;
                        }
                    case JToken j when j.Type == JTokenType.Array && j.Parent.Type == JTokenType.Array:
                        {
                            node.Value = Tuple.Create((IPrimitive)null, new List<EvaluationNode>());
                            break;
                        }
                }
            },
            jToken =>
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
                            var tuple = (Tuple<IPrimitive, List<EvaluationNode>>)node.Value;
                            var value = tuple.Item1.Invoke(tuple.Item2.Select(p => p.Value).ToArray(), referenceDataRepository);
                            node.Value = value.ToString();
                            break;
                        }
                    case JToken j when j.Type == JTokenType.Array && j.Parent.Type == JTokenType.Array:
                        {
                            var tuple = (Tuple<IPrimitive, List<EvaluationNode>>)node.Value;
                            node.Value = tuple.Item2.Select(p => p.Value).ToArray();
                            break;
                        }
                }

                currentNode = node.Parent;
            },
            jToken =>
            {
                var node = new EvaluationNode();
                var tuple = (Tuple<IPrimitive, List<EvaluationNode>>)currentNode.Value;
                tuple.Item2.Add(node);

                if (jToken.Type == JTokenType.String && !(tuple.Item1 == null || !tuple.Item1.ResolveParameters))
                {
                    node.Value = parameters[jToken.ToString()];
                }
                else
                {
                    node.Value = jToken.ToString();
                }
            });

            return decimal.Parse(rootNode.Value.ToString());
        }
    }
}
