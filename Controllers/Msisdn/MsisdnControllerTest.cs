using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using Switchr.API.Areas.Msisdn.Controllers;
using Switchr.API.Tests.Common;
using Switchr.Logic.Handover;
using Switchr.Logic.Msisdn;
using Switchr.Logic.Sim;
using Switchr.Logic.Users;
using Switchr.Logic.Validation;

namespace Switchr.API.Tests.Controllers.Msisdn
{
	class MsisdnControllerTest: BaseControllerTest
	{
		protected IDictionary<Params, object> dict;

		private MsisdnLogic _msisdn;
		private ISimLogic _sim;
		private IAccessRightLogic _accessRight;
		private IUserProfileLogic _userProfile;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private MsisdnController controller => GetController<MsisdnController>();

		public T CreateTestControllerWith<T>(
			MsisdnLogic msisdn = null,
			ISimLogic sim = null,
			IAccessRightLogic accessRight = null,
			IUserProfileLogic userProfile = null
		) where T : ApiController, new()
		{
			msisdn = Eval<MsisdnLogic>(Params.Msisdn, msisdn);
			sim = Eval<ISimLogic>(Params.Sim, sim);
			accessRight = Eval<IAccessRightLogic>(Params.AccessRight, accessRight);
			userProfile = Eval<IUserProfileLogic>(Params.UserProfile, userProfile);

			return new MsisdnController(msisdn, sim, accessRight, userProfile) as T;
		}

		[SetUp]
		public void SetUp()
		{			
			_msisdn = GetInst<MsisdnLogic>();
			_sim = GetInst<ISimLogic>();
			_accessRight = GetInst<IAccessRightLogic>();
			_userProfile = GetInst<IUserProfileLogic>();

			dict = new Dictionary<Params, object>()
			{
				{Params.Msisdn, _msisdn},
				{Params.Sim, _sim},
				{Params.AccessRight, _accessRight},
				{Params.UserProfile, _userProfile},

			};

			base.Setup();
		}

		override protected object GetLookupValue(Enum key)
		{
			return dict[(Params)key];
		}

		public enum Params : int
		{
			Msisdn,
			Sim,
			AccessRight,
			UserProfile
		}

		// 

		// ValidateForCreateB2C(string msisdn)
		[Test]
		[TestCase(true)]
		public async Task ValidateForCreateB2C(bool isValidMsisdn)
		{
			////// Arrange
			var msisdn = GetMsisdn(isValidMsisdn);

			// msisdnLogic
			// _msisdnLogic.ValidateForCreate(msisdn, Role.Subscriber, Segment.Private)
			// !validateForCreate.IsValid
			// SwitchrArgumentException(validateForCreate.ErrorCode

			//// Act
			var result = await controller.ValidateForCreateB2C(msisdn);

			//// Assert
			Assert.IsNotNull(result);
		}

		// ValidateForCreateB2BSubscriber(string msisdn)
		[Test]
		[TestCase(true)]
		public async Task ValidateForCreateB2BSubscriber(bool isValidMsisdn)
		{
			////// Arrange
			var msisdn = GetMsisdn(isValidMsisdn);

			// msisdnLogic
			// _msisdnLogic.ValidateForCreate(msisdn, Role.Subscriber, Segment.Business)
			// !validateForCreate.IsValid
			// SwitchrArgumentException(validateForCreate.ErrorCode

			//// Act
			var result = await controller.ValidateForCreateB2BSubscriber(msisdn);

			//// Assert
			Assert.IsNotNull(result);
		}

		// ValidateForClaimB2C(string msisdn)
		[Test]
		[TestCase(true)]
		public async Task ValidateForClaimB2C(bool isValidMsisdn)
		{
			////// Arrange
			var msisdn = GetMsisdn(isValidMsisdn);

			// msisdnLogic
			// _msisdnLogic.ValidateForClaim(msisdn, Role.Subscriber, Segment.Private)
			// !validateForCreate.IsValid
			// SwitchrArgumentException(validateForCreate.ErrorCode

			//// Act
			var result = await controller.ValidateForClaimB2C(msisdn);

			//// Assert
			Assert.IsNotNull(result);
		}

		// ValidateForClaimB2BSubscriber(string msisdn)
		[Test]
		[TestCase(true)]
		public async Task ValidateForClaimB2BSubscriber(bool isValidMsisdn)
		{
			////// Arrange
			var msisdn = GetMsisdn(isValidMsisdn);

			// msisdnLogic
			// _msisdnLogic.ValidateForClaim(msisdn, Role.Subscriber, Segment.Business)
			// !validateForCreate.IsValid
			// SwitchrArgumentException(validateForCreate.ErrorCode

			//// Act
			var result = await controller.ValidateForClaimB2BSubscriber(msisdn);

			//// Assert
			Assert.IsNotNull(result);
		}

		// IsPinCodeNeeded(string msisdn, string ssn = null,[FromUri]string brandId = "teliadk")
		[Test]
		[TestCase(true)]
		public async Task IsPinCodeNeeded(bool isValidMsisdn, bool isValidSsn)
		{
			////// Arrange
			var msisdn = GetMsisdn(isValidMsisdn);
			var ssn = GetSsn(isValidSsn);

			// msisdnLogic
			// msisdn != string.Empty
			//	_msisdnLogic.IsPinCodeNeededForCreate(msisdn, ssn)
			//	!validateForCreate.IsValid
			//	SwitchrArgumentException(validateForCreate.ErrorCode

			// SuccessResponse<bool>

			// false if msisdn is empty
			// true if logic says it is needed for given msisdn and ssn

			//// Act
			var result = await controller.IsPinCodeNeeded(msisdn);

			//// Assert
			Assert.IsNotNull(result);
		}

		// MarketingPermission(string msisdn)
		[Test]
		[TestCase(true)]
		public async Task MarketingPermission(bool isValidMsisdn)
		{
			////// Arrange
			var msisdn = GetMsisdn(isValidMsisdn);

			// msisdnLogic
			// string.IsNullOrWhiteSpace(msisdn) - SwitchrArgumentException(ErrorCode.InvalidParameter
			// userProfileLogic.HasMarketingPermission(msisdn)
			// GetSubscriptionConsentsAndChannelsAsync(msisdn, systemId)
			// subscriptionService.GetSubscriptionForMsisdn
			// subscription == null - SwitchrNotFoundException(ErrorCode.SubscriptionDetailsNotFound
			// SuccessResponse<bool>

			// false if msisdn is empty
			// true if logic says it is needed for given msisdn and ssn

			//// Act
			var result = await controller.MarketingPermission(msisdn);

			//// Assert
			Assert.IsNotNull(result);
		}

		// GetSimInformation(string msisdn)
		[Test]
		[TestCase(true)]
		public async Task GetSimInformation(bool isValidMsisdn, bool hasAccess)
		{
			////// Arrange
			var msisdn = GetMsisdn(isValidMsisdn);

			// string.IsNullOrWhiteSpace(msisdn) - SwitchrArgumentException(ErrorCode.InvalidParameter
			// hasAccess = accessRightLogic.CheckAccessRight(msisdn, userId)
			// !hasAccess - SwitchrForbiddenException(ErrorCode.ErrorWhenAccesstoSubscription
			// SuccessResponse<bool>

			// false if msisdn is empty
			// true if logic says it is needed for given msisdn and ssn

			//// Act
			var result = await controller.GetSimInformation(msisdn);

			//// Assert
			Assert.IsNotNull(result);
		}

		// ActivateSim(string msisdn, string sim)
		[Test]
		[TestCase(true)]
		public async Task ActivateSim(bool isValidMsisdn, bool isValidSim)
		{
			////// Arrange
			var msisdn = GetMsisdn(isValidMsisdn);
			var sim = GetSim(isValidSim);

			// sim.Length != 20 - SwitchrArgumentException(ErrorCode.InvalidParameter
			// !await _accessRightLogic.CheckAccessRight(msisdn, userId) - SwitchrForbiddenException(ErrorCode.ErrorWhenAccesstoSubscription

			// simLogic.ActivateSim(userId, msisdn, sim)
			// simLogic
			// - cdsService.GetSubscriptionForMsisdn(msisdn.ToLocalnumber())
			// subscription == null - SwitchrNotFoundException(ErrorCode.SubscriptionDetailsNotFound
			// handoverLogic.GetSubmitterStatus(...)?.IsPendingBpm == true - SwitchrException(ErrorCode.SubscriptionIsPendingHandover

			// OK SuccessResponse<bool>

			// false if msisdn is empty
			// true if logic says it is needed for given msisdn and ssn

			//// Act
			var result = await controller.ActivateSim(msisdn, sim);

			//// Assert
			Assert.IsNotNull(result);
		}

	}
}
