using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using NUnit.Framework;
using Switchr.Logic.Exceptions;
using Switchr.Models;
using ModelStateDictionary = System.Web.Http.ModelBinding.ModelStateDictionary;

namespace Switchr.API.Tests.Common
{
	public class ResponseCheckerException: Exception
	{

	}

	public class ResponseChecker
	{
		protected string ScenarioLabel;
		protected ISwitchrException Exception;
		protected HttpResponseMessage ResponseMsg;
		public string Label;
		public bool Success;
		public bool NullRef;

		public HttpResponseMessage Response => ResponseMsg;
		public bool IsBadRequest => Response.StatusCode == HttpStatusCode.BadRequest;
		public bool IsResponseValid => Success || (Response != null) && (Response.StatusCode == HttpStatusCode.OK);
		public bool IsResponseInvalid => !IsResponseValid;
		public bool IsNullRefError => NullRef;

		protected ResponseChecker(ISwitchrException exception = null, HttpResponseMessage result = null, bool success = false, bool nullRef = false)
		{
			Exception = exception;
			ResponseMsg = result;
			Success = success;
			NullRef = nullRef;
		}

		public void SetScenarioLabel(string scenarioLabel)
		{
			ScenarioLabel = scenarioLabel;
		}


		// factory methods

		public static ResponseChecker Create(object result, string label = null)
		{
			if (result == null)
			{
				throw new InvalidResultException("Error: Result is null");
			}

			var typeName = result.GetType().Name;

			Debug.WriteLine($"Create ResponseChecker for { typeName }");
			try
			{
				var checker = CreateFor((dynamic) result);
				checker.Label = label;
				return checker;
			}
			catch (Exception ex)
			{
				throw new InvalidResultException($"Error: Result type has no handler { typeName } - { ex.Message}");
			}			
		}

		public static ResponseChecker CreateFor(Task task)
		{
			throw new InvalidResultException("Error: Result is an async task that must be awaited to provide the result to be tested");
		}

		public static ResponseChecker CreateFor(ISwitchrException exception)
		{
			return new ResponseChecker(exception: exception);
		}

		public static ResponseChecker CreateFor(NullReferenceException exception)
		{
			return new ResponseChecker(nullRef: true);
		}

		public static ResponseChecker CreateFor(AggregateException aggrException)
		{
			var inner = aggrException.InnerExceptions;
			var exception = inner[0] as ISwitchrException;
			return new ResponseChecker(exception: exception);
		}


		public static ResponseChecker CreateFor(HttpResponseMessage result)
		{
			return new ResponseChecker(result: result);
		}

		public static ResponseChecker CreateFor<T>(OkNegotiatedContentResult<T> okResult)
		{
			return new ResponseChecker(success: true);
		}

		public bool CheckResponse(ResultExpectation resultExpectation)
		{
			Debug.WriteLine($"resultExpectation: {ScenarioLabel} - { resultExpectation }");
			return (resultExpectation.Ok) ? CheckValidResult() : CheckInvalidResult(resultExpectation.ErrorType);
		}

		public bool CheckInvalidResult(ErrorType expectedError)
		{
			if (expectedError == null)
				return true;

			if (Success)
			{
				Debug.WriteLine($"Result Check Error: Checking for invalid result when the result returned was a success");
				return false;
			}

			if (Exception == null)
			{
				return HandleNullRefException();
			}

			try
			{
				bool isInvalidAsExpected = IsInvalidResultWith(expectedError.name, expectedError.ErrorCode);
				Assert.IsTrue(isInvalidAsExpected);
				Debug.WriteLine($":: {Label} Was invalid as expected: {expectedError.name}");
				return isInvalidAsExpected;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Unexpected error when checking invalid result: { ex.Message} source: { ex.Source}");
				throw ex;
			}
		}

		public bool CheckValidResult()
		{
			bool isValidResponse = IsResponseValid;
			Assert.IsTrue(isValidResponse);
			// TODO: scenario label
			Debug.WriteLine($":: {Label} Was valid as expected");
			return isValidResponse;
		}


		// Ideally we should do sth like
		// https://www.infoworld.com/article/3207541/application-development/how-to-make-your-asp-net-web-api-responses-consistent-and-useful.html

		public bool IsInvalidResultWith(string errorMsg = null, ErrorCode code = ErrorCode.Unknown)
		{
			return code != ErrorCode.Unknown ? HasErrorCode(code) : IsInvalidResult(errorMsg);
		}

		protected bool HandleNullRefException()
		{
			Debug.WriteLine($"Result Check Error: { Label} The result was null");
			return false;
		}

		public bool IsInvalidResult(string errorMsg = null)
		{
			return !IsResponseValid && HasError(errorMsg);
		}

		public bool HasErrorCode(ErrorCode code)
		{
			return Exception.Code == code;
		}

		protected bool HasError(string errorMsg = null)
		{
			return !IsResponseValid && HasMatchingErrorMsg(errorMsg);
		}

		protected bool HasMatchingErrorMsg(string errorMsg)
		{
			return errorMsg != null ? GetApiException.HasMatchingError(errorMsg) : true;
		}
			
		public ApiException GetApiException => APIExceptionDeserializer.CreateApiException(Response);

		public static List<string> GetErrorListFromModelState
			(ModelStateDictionary modelState)
		{
			var query = from state in modelState.Values
				from error in state.Errors
				select error.ErrorMessage;

			var errorList = query.ToList();
			return errorList;
		}
	}
}
