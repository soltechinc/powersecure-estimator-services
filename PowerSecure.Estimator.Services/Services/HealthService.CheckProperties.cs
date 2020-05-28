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
			if (Environment.GetEnvironmentVariable("moduleTemplatesCollectionId") == null) yield return "moduleTemplatesCollectionId";
			if (Environment.GetEnvironmentVariable("factorsCollectionId") == null) yield return "factorsCollectionId";
			if (Environment.GetEnvironmentVariable("functionsCollectionId") == null) yield return "functionsCollectionId";
			if (Environment.GetEnvironmentVariable("dev-url") == null) yield return "dev-url";
		}
	}
}