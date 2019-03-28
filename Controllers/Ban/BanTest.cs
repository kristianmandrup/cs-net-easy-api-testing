using System;
using System.Collections.Generic;
using Switchr.API.Tests.Common;
using Switchr.Logic.Accounts;
using Switchr.Logic.CreditAgreement;
using Switchr.Logic.Invoice;
using Switchr.Logic.Subscriptions;
using Switchr.Logic.Users;

namespace Switchr.API.Tests.Controllers.Ban
{
	class BanTest: BaseControllerTest
	{
		protected IDictionary<Params, object> dict;

		protected UserLogic _userLogic;
		private ISubscriptionAccessRightsLogic _subscriptionAccessRightsLogic;
		private IAccountLogic _accountLogic;
		private BanAccessRightsLogic _banAccessRightsLogic;
		private IUserProfileLogic _userProfileLogic;
		private ICreditAgreementLogic _creditAgreementLogic;
		private IInvoiceService _invoiceService;

		public override T CreateTestController<T>()
		{
			return null;
		}

		public new void Setup()
		{		
			_userLogic = GetInst<UserLogic>();
			_banAccessRightsLogic = GetInst<BanAccessRightsLogic>();
			_userProfileLogic = GetInst<IUserProfileLogic>();
			_accountLogic = GetInst<IAccountLogic>();
			_subscriptionAccessRightsLogic = GetInst<ISubscriptionAccessRightsLogic>();
			_creditAgreementLogic = GetInst<ICreditAgreementLogic>();
			_invoiceService = GetInst<IInvoiceService>();

			dict = new Dictionary<Params, object>()
			{
				{Params.User, _userLogic},
				{Params.BanAccessRights, _banAccessRightsLogic},
				{Params.UserProfile, _userProfileLogic},
				{Params.Account, _accountLogic},
				{Params.SubscriptionAccessRights, _subscriptionAccessRightsLogic},
				{Params.CreditAgreement,_creditAgreementLogic },
				{Params.InvoiceService,_invoiceService }
			};

			base.Setup();
		}

		override protected object GetLookupValue(Enum key)
		{
			return dict[(Params)key];
		}

		public enum Params : int
		{
			User,
			BanAccessRights,
			UserProfile,
			Account,
			SubscriptionAccessRights,
			CreditAgreement,
			InvoiceService,
		}
	}
}
