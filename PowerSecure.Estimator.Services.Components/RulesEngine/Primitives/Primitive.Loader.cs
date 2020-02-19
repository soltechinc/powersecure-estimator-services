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
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new AndPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new CeilingPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new CheckPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new ConcatenatePrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new DivisionPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new EqualPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new FindPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new GreaterThanPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new IsNullPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new LessThanPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new ListPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new MarginPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new MaxPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new MinPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new MinusOnePrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new MultiplicationPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new NotEqualPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new NotPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new OrPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new PricePrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new StepPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new SubtractionPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new SumProductPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new SwitchPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new ThresholdPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			primitive = new ZeroPrimitive();
			dict.Add(primitive.Name.ToLower(), primitive);
			return dict.ToReadonlyDictionary();
		}
	}
}
