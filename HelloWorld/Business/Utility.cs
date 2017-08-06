using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelloWorld.Business
{
	public class Utility
	{
		public static string GetConnectionString()
		{
			return System.Configuration.ConfigurationManager.ConnectionStrings["HelloWorldCon"].ConnectionString;
		}

		public static bool IsValidUrl(string uriName)
		{
			Uri uriResult;
			bool result = Uri.TryCreate(uriName, UriKind.Absolute, out uriResult)
				&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

			return result;
		}
	}
}