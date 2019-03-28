using System;
using System.Collections.Generic;
using Switchr.API.Tests.Common;
using Switchr.Data.Interfaces;
using Switchr.Logic.Accounts;
using Switchr.Logic.Configuration;
using Switchr.Logic.Email;
using Switchr.Logic.Services.CBM;
using Switchr.Logic.Services.Switchr.Interfaces;
using Switchr.Logic.Sim;
using Switchr.Logic.Subscriptions;
using Switchr.Logic.TopUp;
using Switchr.Logic.Usage;
using Switchr.Logic.Validation;

namespace Switchr.API.Tests.Controllers.Email
{
	public class EmailTest: BaseControllerTest
	{
		protected IDictionary<Params, object> dict;

		protected IEmailLogic Email;
		protected ITeliaTVUsernameLogic TeliaTvUsername;

		public override T CreateTestController<T>()
		{
			return null;
		}

		protected int GetSubscriptionId(bool isValid = true)
		{
			return Config(isValid).GetInt("subscriptionId");
		}

		protected string GetEmail(bool isValidEmail)
		{
			return Config(isValidEmail).GetStr("email");
		}

		public new void Setup()
		{					
			Email = GetInst<IEmailLogic>();
			
			dict = new Dictionary<Params, object>()
			{
				{ Params.Email, Email },
				{ Params.TeliaTVUsername, TeliaTvUsername }
			};

			base.Setup();
		}

		override protected object GetLookupValue(Enum key)
		{
			return dict[(Params)key];
		}

		public enum Params : int
		{
			Email,
			TeliaTVUsername
		}
	}
}
