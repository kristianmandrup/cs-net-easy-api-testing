using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switchr.API.Tests.Common.TestOutput
{
	public class TableHeader
	{
		private string[] _scenarios;

		public TableHeader(string[] scenarios)
		{
			_scenarios = scenarios;
		}

		public string generate()
		{
			return "<thead><th class='testHeader'>Test</th>"
			       + ScenarioHeaders(_scenarios) +
			       "</thead>";
		}

		protected string ScenarioHeaders(string[] scenarios)
		{
			return scenarios.Aggregate("", ScenarioHeader);
		}

		protected string ScenarioHeader(string acc, string label)
		{
			return acc + $"<th>{label}</td>";
		}
	}

}
