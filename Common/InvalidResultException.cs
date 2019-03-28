using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switchr.API.Tests.Common
{
	public class InvalidResultException : Exception
	{
		public InvalidResultException(string name)
			: base($"Invalid result for: {name}")
		{

		}

	}
}
