using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Switchr.API.Tests.Common
{
	public class ResultExpectation
	{
		private ErrorType errorType;
		private Boolean ok;
		private string name;

		public string Name { get => name; set => name = value; }
		public bool Ok { get => ok; set => ok = value; }
		public ErrorType ErrorType { get => errorType; set => errorType = value; }

		public static ResultExpectation CreateError(ErrorType errorType, string name)
		{
			var re = new ResultExpectation();
			re.ErrorType = errorType;
			re.Name = name;
			return re;
		}

		public static ResultExpectation CreateOK()
		{
			var re = new ResultExpectation();
			re.Ok = true;
			return re;
		}

		override public string ToString()
		{
			return Ok ? "OK" : $"Exception: {errorType.ExceptionName}";
		}
	}
}
