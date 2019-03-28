using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Switchr.Models;

namespace Switchr.API.Tests.Common
{
	public class ErrorType
	{
		protected string Validation;
		protected Type ExceptionType;
		public string ExceptionName => ExceptionType.ToString();

		public ErrorCode ErrorCode { get; private set; }
		public string name;
		public string MatchMsg;

		static public ErrorType CreateException<T>(ErrorCode code, string msg) where T: Exception
		{
			var et = new ErrorType();
			et.ExceptionType = typeof(T);
			et.ErrorCode = code;
			et.MatchMsg = msg;
			return et;
		}

		static public ErrorType CreateValidation(string validation)
		{
			var et = new ErrorType();
			et.Validation = validation;
			return et;
		}
	}
}
