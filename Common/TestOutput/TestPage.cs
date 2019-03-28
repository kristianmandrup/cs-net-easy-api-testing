using Switchr.API.Tests.Common.TestOutput;

namespace Switchr.API.Tests.Common
{
	public class TestPage
	{
		private string[] _scenarios;
		private TestResult[] _testResults;

		private string title => $"Test results - { _controllerName }";
		private string _controllerName;

		public TestPage(TestResult[] testResults, string[] scenarios, string controllerName = "Unknown")
		{
			_testResults = testResults;
			_scenarios = scenarios;
			_controllerName = controllerName;
		}

		public string Page(string title) {
			return $@"{Header(title)}";
		}

		protected string Header(string title)
		{
			return $@"<!DOCTYPE html>
		<html>
		<head>
			<title>{ title }</title>
			{ Style }
		<head>";
		}

		protected string Body()
		{
			return $@"<body>
				<table border='1'>
					{ RenderTableHeader() }
					{ RenderTableBody() }
				</table>";

		}

		protected string RenderTableHeader()
		{
			return new TableHeader(_scenarios).generate();
		}

		protected string RenderTableBody()
		{
			return new TableBody(_testResults).generate();
		}		

		protected string Style = @" < style type='text/css'>
		.passed {
		background: green;
		color: black;
		}

		.notPassed {
		background: red;
		color: black;
		}

		.testName {
		font-weight: bold;
		}

		.testHeader {
		background: grey;
		}

		thead {
		background: silver;
		}

		td,th {
		padding: .5em;
		}

		.state {
		text-align: center;
		}
		</style>";

	}
}