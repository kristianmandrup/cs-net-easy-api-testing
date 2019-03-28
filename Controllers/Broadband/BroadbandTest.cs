using System;
using System.Collections.Generic;
using Switchr.API.Tests.Common;
using Switchr.Logic.Broadband;

namespace Switchr.API.Tests.Controllers.Ban
{
	class BroadBandTest : BaseControllerTest
	{
		protected IDictionary<Params, object> dict;

		private IBroadbandInstallationLogic _broadbandInstallationLogic;

		public override T CreateTestController<T>()
		{
			return null;
		}

		public void Setup()
		{			
			_broadbandInstallationLogic = GetInst<IBroadbandInstallationLogic>();

			dict = new Dictionary<Params, object>()
			{
				{Params.Broadband, _broadbandInstallationLogic}
			};

			base.Setup();
		}

		override protected object GetLookupValue(Enum key)
		{
			return dict[(Params)key];
		}

		public enum Params : int
		{
			Broadband
		}
	}
}
