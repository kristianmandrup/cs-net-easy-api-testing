using System;
using System.Collections.Generic;
using Switchr.API.Tests.Common;
using Switchr.Data.Interfaces;
using Switchr.Logic.Accounts;
using Switchr.Logic.Configuration;
using Switchr.Logic.Services.CBM;
using Switchr.Logic.Services.Switchr.Interfaces;
using Switchr.Logic.Sim;
using Switchr.Logic.Subscriptions;
using Switchr.Logic.TopUp;
using Switchr.Logic.Usage;
using Switchr.Logic.Validation;

namespace Switchr.API.Tests.Controllers
{
	public class SubscriptionTest: BaseControllerTest
	{
		protected IDictionary<Params, object> dict;

		protected IUserManager _userManager;
		private IUsageLogic _usageLogic;
		private UserValidationLogic _userValidationLogic;

		private IAccessRightLogic _accessRightLogic;

		private ISubscriptionService _subscriptionService;
		private ISubscriptionLogic _subscriptionLogic;
		private SubscriptionVasLogic _subscriptionVasLogic;
		private ISubscriptionStatusLogic _subscriptionStatusLogic;
		private IChangeSubscriptionLogic _changeSubscriptionLogic;
		private ISubscriptionOfferingLogic _subscriptionOfferingLogic;

		private IApplicationSettings _appSettings;
		private ICatalogService _catalogService;

		private IOrderSimLogic _orderSimLogic;
		
		private ICbmService _cbmService;
		private IAccountLogic _accountLogic;

		protected ITopUpLogic _topUpLogic;


		public SubscriptionTest()
		{
			validConf = ConfigHelper.Create("valid.ini");
			invalidConf = ConfigHelper.Create("invalid.ini");
		}

		public new void Setup()
		{
			base.Setup();
			
			_topUpLogic = GetInst<ITopUpLogic>();

			_userManager = GetInst<IUserManager>();
			_usageLogic = GetInst<IUsageLogic>();
			_userValidationLogic = GetInst<UserValidationLogic>();
			
			_accessRightLogic = GetInst<IAccessRightLogic>();

			_subscriptionService = GetInst<ISubscriptionService>();
			_subscriptionLogic = GetInst<ISubscriptionLogic>();
			_subscriptionVasLogic = GetInst<SubscriptionVasLogic>();
			_subscriptionStatusLogic = GetInst<ISubscriptionStatusLogic>();
			_changeSubscriptionLogic = GetInst<IChangeSubscriptionLogic>();
			_subscriptionOfferingLogic = GetInst<ISubscriptionOfferingLogic>();

			_appSettings = GetInst<IApplicationSettings>();
			_catalogService = GetInst<ICatalogService>();
			
			_cbmService = GetInst<ICbmService>();
			_orderSimLogic = GetInst<IOrderSimLogic>();
			_accountLogic = GetInst<IAccountLogic>();

			dict = new Dictionary<Params, object>()
			{
				{ Params.TopUp, _topUpLogic },
				{ Params.UserManager, _userManager },
				{ Params.Usage, _usageLogic },
				{ Params.UserValidation, _userValidationLogic},
				{ Params.AccessRights, _accessRightLogic},
				{ Params.SubscriptionService, _subscriptionService}, 
				{ Params.Subscription, _subscriptionLogic},
				{ Params.SubscriptionVas, _subscriptionVasLogic},
				{ Params.SubscriptionStatus, _subscriptionStatusLogic},
				{ Params.ChangeSubscription, _changeSubscriptionLogic},
				{ Params.SubscriptionOffering, _subscriptionOfferingLogic},
				{ Params.AppSettings, _appSettings},
				{ Params.CatalogService, _catalogService},
				{ Params.CbmService, _cbmService},
				{ Params.OrderSim, _orderSimLogic},
				{ Params.Account, _accountLogic}
			};
		}

		override protected object GetLookupValue(Enum key)
		{
			return dict[(Params)key];
		}

		public override T CreateTestController<T>()
		{
			throw new NotImplementedException();
		}

		public enum Params : int
		{
			TopUp,
			UserManager,
			Usage,
			UserValidation,
			AccessRights,
			Subscription,
			SubscriptionService, 
			SubscriptionVas,
			SubscriptionStatus,
			ChangeSubscription,
			SubscriptionOffering,
			AppSettings,
			CatalogService,
			CbmService,
			OrderSim,
			Account
		}
	}
}
