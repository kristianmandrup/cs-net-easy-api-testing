using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switchr.API.Tests.Common
{
	public class TestResult
	{
		public string TestName;
		public bool Passed = false;
		public ScenarioResult[] scenarioResults;
	}
}
