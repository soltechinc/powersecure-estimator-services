﻿using System.Collections.Generic;

namespace PowerSecure.Estimator.Services.Components.RulesEngine.Primitives
{
	public static partial class Primitive
	{
		public static IDictionary<string, IFunction> Load() 
		{
			IFunction function;
			var dict = new Dictionary<string, IFunction>();
			function = new AdditionPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new AndPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new CeilingPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new CheckPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new ConcatenatePrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new DivisionPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new EqualPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new FindPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new GreaterThanPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new IsNullPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new LessThanPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new ListPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new MarginPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new MaxPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new MinPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new MinusOnePrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new MultiplicationPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new NotEqualPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new NotPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new OrPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new PricePrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new StepPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new SubtractionPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new SumProductPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new SwitchPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new ThresholdPrimitive();
			dict.Add(function.Name.ToLower(), function);
			function = new ZeroPrimitive();
			dict.Add(function.Name.ToLower(), function);
			return dict;
		}
	}
}
