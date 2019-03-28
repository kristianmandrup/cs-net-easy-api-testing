using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Castle.Core.Resource;
using NUnit.Framework;
using Switchr.API.Areas.Permission;
using Switchr.API.Tests.Common;
using Switchr.Logic.Users;

namespace Switchr.API.Tests.Controllers.Permission
{
	class PermissionControllerTest: BaseControllerTest
	{
		protected IDictionary<Params, object> dict;

		IUserProfileLogic _userProfile;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private PermissionController controller => GetController<PermissionController>();

		public T CreateTestControllerWith<T>(IUserProfileLogic userProfile = null) where T : ApiController, new()
		{
			userProfile = useOrDefault< IUserProfileLogic>(userProfile, _userProfile);
			return new PermissionController(_userProfile) as T;
		}

		[SetUp]
		public void SetUp()
		{
			_userProfile = GetInst<IUserProfileLogic>();

			base.Setup();			
		}

		override protected object GetLookupValue(Enum key)
		{
			return dict[(Params)key];
		}

		protected enum Params : int { }

		// GetUserConsentsBySubscription(string msisdn, string systemId)
		[Test]
		[TestCase(true, true)]
		[TestCase(true, false)]
		[TestCase(false, false)]
		public async Task GetUserConsentsBySubscription(bool isValidMsisdn, bool isValidSystemId)
		{
			////// Arrange
			var msisdn = GetMsisdn(isValidMsisdn);
			var systemId = GetSystemId(isValidSystemId);

			// subscriptionService
			// userProfileLogic.GetSubscriptionConsentsAndChannelsAsync
			// subscriptionService.GetSubscriptionForMsisdn(msisdn)

			//// Act
			var result = await controller.GetUserConsentsBySubscription(msisdn, systemId);

			// If invalid
			Assert.Catch<Exception>(async () => await controller.GetUserConsentsBySubscription(msisdn, systemId));

			//// Assert
			Assert.IsNotNull(result);
		}


		// GetUserConsents(string userId, string systemId)
		[Test]
		[TestCase(true, true)]
		[TestCase(true, false)]
		[TestCase(false, false)]

		public async Task GetUserConsents(bool isValidUserId, bool isValidSystemId)
		{
			////// Arrange
			var userId = GetUserIdStr(isValidUserId);
			var systemId = GetSystemId(isValidSystemId);

			//// Act and Assert
			//  userProfileLogic.GetProfileConsentsAndChannelsAsync
			// subscriptionService.GetSubscriptionForMsisdn(msisdn)

			// if valid
			var result = await controller.GetUserConsents(userId, systemId);
			Assert.IsNotNull(result);

			// if invalid
			Assert.Catch<Exception>(async () => await controller.GetUserConsentsBySubscription(userId, systemId));
		}
	}
}
