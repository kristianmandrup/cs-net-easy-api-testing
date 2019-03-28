using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Switchr.Models;

namespace Switchr.API.Tests.Common
{

	public class ApiException : Exception
	{
		public HttpResponseMessage Response { get; set; }
		public ApiException(HttpResponseMessage response)
		{
			this.Response = response;
		}

		public HttpStatusCode StatusCode => this.Response.StatusCode;

		public int Code => (int) this.StatusCode;

		public IEnumerable<string> Errors => this.Data.Values.Cast<string>().ToList();

		public bool IsError => Code < 400;

		public bool HasMatchingError(string error = "any")
		{
			if (error == "any" && IsError) return true;
			if (!IsError) return false;
			return Errors.FirstOrDefault(err => MatchErr(err, error)) != null;
		}

		// switch strategy here
		protected bool MatchErr(string errStr, string compareStr)
		{
			return MatchRegExp(errStr , compareStr);
		}
		
		protected bool MatchExact(string errStr, string compareStr)
		{
			return errStr == compareStr;
		}

		protected bool MatchContains(string errStr, string compareStr)
		{
			return errStr.Contains(compareStr);
		}

		protected bool MatchRegExp(string errStr, string pattern)
		{
			Match m = Regex.Match(errStr, pattern, RegexOptions.IgnoreCase);
			return m.Success;
		}
	}
	
}
