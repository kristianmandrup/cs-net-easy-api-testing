using System;
using System.Collections.Generic;
using Switchr.API.Tests.Common;
using Switchr.Logic.Gdpr;
using Switchr.Logic.Permissions;
using Switchr.Logic.Users;
using Switchr.Logic.Users.Interfaces;
using Switchr.Logic.Users.OTAC;
using Switchr.Models;
using Switchr.Models.User;

namespace Switchr.API.Tests.Controllers.User {
	public abstract class UserTest : BaseControllerTest {
		protected IDictionary<Params, object> dict;

		protected UserLogic User;
		protected IPasswordLogic Password;
		protected UserServicesLogic UserServices;
		protected SynchronizeUserLogic SyncUser;
		protected IUserProfileLogic UserProfile;
		protected IConsentLogic Consent;
		protected OtacLogic Otac;
		protected IHandoverCreditCheckLogic HandoverCreditCheck;
		protected ILegacyBroadbandService LegacyBroadband;
		protected IAccessPersonalDataLogic AccessPersonalData;

		protected Request<PasswordResetDetails> GetPasswordResetDetails (bool isValid = true) {
			return GetRequest<PasswordResetDetails> ();
		}

		public new void Setup () {
			base.Setup ();

			User = GetInst<UserLogic>();
			Password = GetInst<IPasswordLogic>();
			UserServices = GetInst<UserServicesLogic>();
			SyncUser = GetInst<SynchronizeUserLogic>();
			UserProfile = GetInst<IUserProfileLogic>();
			Consent = GetInst<IConsentLogic>();
			Otac = GetInst<OtacLogic>();
			HandoverCreditCheck = GetInst<IHandoverCreditCheckLogic>();
			LegacyBroadband = GetInst<ILegacyBroadbandService>();
			AccessPersonalData = GetInst<IAccessPersonalDataLogic>();

			dict = new Dictionary<Params, object>() {
				{ Params.User, User },
				{ Params.Password, Password },
				{ Params.UserServices, UserServices },
				{ Params.SyncUser, SyncUser },
				{ Params.UserProfile, UserProfile },
				{ Params.Consent, Consent },
				{ Params.Otac, Otac },
				{ Params.HandoverCreditCheck, HandoverCreditCheck },
				{ Params.LegacyBroadband, LegacyBroadband },
				{ Params.AccessPersonalData, AccessPersonalData },
			};
		}

		override protected object GetLookupValue(Enum key)
		{
			return dict[(Params)key];
		}

		public enum Params : int {
			User,
			UserServices,
			SyncUser,
			UserProfile,
			Consent,
			Password,
			Otac,
			HandoverCreditCheck,
			LegacyBroadband,
			AccessPersonalData
		}
	}
}