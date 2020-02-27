using Newtonsoft.Json.Linq;
using PowerSecure.Estimator.Services.Components.RulesEngine.Primitives;
using PowerSecure.Estimator.Services.Components.RulesEngine.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerSecure.Estimator.Services.Components.RulesEngine
{
    public class UnresolvedParameter
    {
        public UnresolvedParameter() { }

        public JToken Token { get; set; }
        public IDictionary<string, object> Parameters { get; set; }
        public IDictionary<string, IFunction> Functions { get; set; }
        public IReferenceDataRepository ReferenceDataRepository { get; set; }
        public bool IsNullValue { get; private set; } = false;

        private UnresolvedParameter(JToken jToken, UnresolvedParameter parentParameter)
        {
            Token = jToken;
            Parameters = parentParameter.Parameters;
            Functions = parentParameter.Functions;
            ReferenceDataRepository = parentParameter.ReferenceDataRepository;
        }

        public object Resolve()
        {
            if(IsNullValue)
            {
                return null;
            }

            object value = new Func<object>(() =>
                {
                    //convert object to primitive or scalar
                    switch (Token.Type)
                    {
                        case JTokenType.Object:
                            { //this is a primitive
                                var jProp = Token.Children<JProperty>().First();
                                var unresolvedParameters = jProp.Value.Children().Select(jToken => new UnresolvedParameter(jToken, this)).ToArray();
                                object retValue = null;
                                try
                                {
                                    retValue = Functions[jProp.Name].Invoke(unresolvedParameters, ReferenceDataRepository);
                                }
                                catch (Exception ignored) { }
                                return unresolvedParameters.Any(p => p.IsNullValue) ? null : retValue;
                            }
                        case JTokenType.Array when Token.Parent.Type == JTokenType.Array:
                            {
                                //this is an array, resolve all parameters
                                return Token.Children().Select(jToken => new UnresolvedParameter(jToken, this).Resolve()).ToArray();
                            }
                        case JTokenType.Array:
                            {
                                break;
                            }
                        case JTokenType.String:
                            {
                                string str = Token.ToString();
                                if (string.IsNullOrEmpty(str) || str.StartsWith('$')) //these are string literals
                                {
                                    return str;
                                }
                                else //these should be resolved against the parameters
                                {
                                    string key = str.Trim().ToLower();
                                    if (!Parameters.ContainsKey(key))
                                    {
                                        return null;
                                    }
                                    if (Parameters[key] is IInstructionSet childInstructionSet)
                                    {
                                        Parameters[key] = childInstructionSet.Evaluate(Parameters, Functions, ReferenceDataRepository);
                                    }

                                    return Parameters[key];
                                }
                            }
                        case JTokenType.Null:
                            {
                                return null;
                            }
                        case JTokenType.Boolean:
                            {
                                return bool.Parse(Token.ToString());
                            }
                        default:
                            {
                                return decimal.Parse(Token.ToString());
                            }
                    }
                    return null;
                })();

            if(value == null)
            {
                IsNullValue = true;
            }
            return value;
        }
    }
}
