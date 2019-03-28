using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace Switchr.API.Tests.Common
{
	class APIExceptionDeserializer
	{
		public static ApiException CreateApiException(HttpResponseMessage response)
		{
			var httpErrorObject = response.Content.ReadAsStringAsync().Result;

			// Create an anonymous object to use as the template for deserialization:
			var anonymousErrorObject = new {message = "", ModelState = new Dictionary<string, string[]>()};

			// Deserialize:
			var deserializedErrorObject = JsonConvert.DeserializeAnonymousType(httpErrorObject, anonymousErrorObject);

			// Now wrap into an exception which best fullfills the needs of your application:
			var ex = new ApiException(response);

			// Sometimes, there may be Model Errors:
			if (deserializedErrorObject.ModelState != null)
			{
				var errors = deserializedErrorObject.ModelState.Select(kvp => string.Join(". ", kvp.Value));
				var enumerable = errors as string[] ?? errors.ToArray();
				for (var i = 0; i < enumerable.Count(); i++)
				{
					// Wrap the errors up into the base Exception.Data Dictionary:
					ex.Data.Add(i, enumerable.ElementAt(i));
				}
			}
			// Othertimes, there may not be Model Errors:
			else
			{
				var error = JsonConvert.DeserializeObject<Dictionary<string, string>>(httpErrorObject);
				foreach (var kvp in error)
				{
					// Wrap the errors up into the base Exception.Data Dictionary:
					ex.Data.Add(kvp.Key, kvp.Value);
				}
			}

			return ex;

		}
	}
}