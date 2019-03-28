using System.Web.Http;
using System.Web.Http.ModelBinding;

public static class HttpErrorExtensions
{
	public static ModelStateDictionary GetModelState(this HttpError httpError)
	{
		// Ensure.Argument.NotNull(httpError, "httpError");

		object serialized;
		if (httpError.TryGetValue("ModelState", out serialized))
		{
			var modelState = new ModelStateDictionary();

			var errors = (HttpError)httpError["ModelState"];

			foreach (var error in errors)
			{
				foreach (var message in error.Value as string[])
				{
					modelState.AddModelError(error.Key, message);
				}
			}

			return modelState;
		}

		return null;
	}
}