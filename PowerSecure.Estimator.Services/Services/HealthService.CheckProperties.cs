using System;
using System.Collections.Generic;

namespace PowerSecure.Estimator.Services.Services
{
	public partial class HealthService
	{
		public static IEnumerable<string> MissingPropertiesIterator() 
		{
			if (Environment.GetEnvironmentVariable("dbConnection") == null) yield return "dbConnection";
			if (Environment.GetEnvironmentVariable("databaseId") == null) yield return "databaseId";
			if (Environment.GetEnvironmentVariable("modulesCollectionId") == null) yield return "modulesCollectionId";
			if (Environment.GetEnvironmentVariable("factorsCollectionId") == null) yield return "factorsCollectionId";
			if (Environment.GetEnvironmentVariable("functionsCollectionId") == null) yield return "functionsCollectionId";
		}
	}
}