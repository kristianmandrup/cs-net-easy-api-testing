using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq.Language.Flow;

namespace Switchr.API.Tests.Common.Moq
{
	public class MoqSetup<TS> where TS: class
	{
		private readonly ISetup<TS, object> _setup;
		private readonly MockMoq<TS> _ctx;


		public MoqSetup(MockMoq<TS> ctx, object setup)
		{
			_setup = (ISetup<TS, object>) setup;
			_ctx = ctx;
		}

		public MockMoq<TS> Returns(object returnValue)
		{
			_setup.Returns(returnValue);
			return _ctx;
		}

		public MockMoq<TS> ReturnsExplicit<RT>(object returnValue)
		{
			_setup.Returns((RT) returnValue);
			return _ctx;
		}
	}
}
