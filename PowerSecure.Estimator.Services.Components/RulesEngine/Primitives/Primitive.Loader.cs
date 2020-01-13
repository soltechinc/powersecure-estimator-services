using System.Collections.Generic;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
	public static partial class Primitive
	{
		public static IDictionary<string, IPrimitive> Load() 
		{
			IPrimitive primitive;
			var dict = new Dictionary<string, IPrimitive>();
			primitive = new AdditionPrimitive();
			dict.Add(primitive.Name, primitive);
			primitive = new DivisionPrimitive();
			dict.Add(primitive.Name, primitive);
			primitive = new MultiplicationPrimitive();
			dict.Add(primitive.Name, primitive);
			primitive = new SubtractionPrimitive();
			dict.Add(primitive.Name, primitive);
			return dict;
		}
	}
}
