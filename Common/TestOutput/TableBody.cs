using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switchr.API.Tests.Common.TestOutput
{
	class TableBody
	{
		private TestResult[] _testResults;

		public TableBody(TestResult[] testResults)
		{
			_testResults = testResults;
		}

		public string generate()
		{
			return _testResults.Aggregate("", Row);
		}

		protected string Row(string acc, TestResult testResult)
		{
			string clazz = RowClass(testResult);
			return "<tr class='{clazz}'>" + RowBody(testResult) + "<tr>";
		}

		protected string RowBody(TestResult testResult)
		{
			return TestNameCell(testResult) + RenderScenarioResults(testResult.scenarioResults);
		}

		protected string RowClass(TestResult testResult)
		{
			return testResult.Passed ? "passed" : "notPassed";
		}

		protected string RenderScenarioResults(ScenarioResult[] scenarioResults)
		{
			return scenarioResults.Aggregate("", RenderScenarioResult);
		}

		protected string RenderScenarioResult(string acc, ScenarioResult scenarioResult)
		{
			return acc + StateCell(scenarioResult);
		}

		protected string Cell(string clazz, string label)
		{
			return $"<td class='{clazz}'>{label}</td>";
		}

		protected string TestNameCell(TestResult testResult)
		{
			return Cell("testName", testResult.TestName);
		}

		protected string StateCell(ScenarioResult scenario)
		{
			return Cell("state", ScenarioResultValue(scenario));
		}

		protected string ScenarioResultValue(ScenarioResult scenario)
		{
			return scenario.IsValid ? "✓" : "⚠";
		}
	}

}
