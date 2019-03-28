using System;
using NUnit.Framework;
using Switchr.API.Areas.Ban.Controllers;
using Switchr.Logic.Accounts;
using Switchr.Logic.Subscriptions;
using Switchr.Logic.Users;

namespace Switchr.API.Tests.Controllers.Ban
{

	[SetUpFixture]
	public class MySetUpClass
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			// ...
		}

		[OneTimeTearDown]
		public void RunAfterAnyTests()
		{
			// ...
		}
	}

	class BanControllerTest: BanTest
	{

		private BanController _controller;

		// TODO: possibly use dynamic method invocation
		public BanController CreateTestController(
			UserLogic user = null, 
			ISubscriptionAccessRightsLogic subscriptionAccessRights = null, 
			IAccountLogic account = null
		)
		{
			user = Eval<UserLogic>(Params.User, user);
			subscriptionAccessRights = Eval<ISubscriptionAccessRightsLogic>(Params.SubscriptionAccessRights, subscriptionAccessRights);
			account = Eval<IAccountLogic>(Params.Account, account);
			return new BanController(user, subscriptionAccessRights, account);
		}

		protected BanController CreateController()
		{
			return CreateTestController();
		}

		[SetUp]
		public void SetUp()
		{
			base.Setup();
			_controller = CreateController();
		}

		[Test]
		public void AlwaysTrue()
		{
			Assert.AreEqual(4, 2 + 2);
		}

		[Test]
		[TestCase(true)]
		public void AccessRightsRequest(bool isValidBan)
		{
			//// Arrange
			var ban = GetBan(isValidBan);

			// !_userLogic.UserConnectedToBan(User.UserId(), ban) - ResponseMessage:Forbidden

			//// Act
			var result = _controller.GetB2BAdminsForBanAsync(ban);

			//// Assert
			Assert.IsNotNull(result);
		}

		// GetB2BAdminsForBanAsync(int ban)
		[Test]
		[TestCase(true)]
		public void GetB2BAdminsForBanAsync(bool isValidBan)
		{
			//// Arrange
			var ban = GetBan(isValidBan);

			// !_userLogic.UserConnectedToBan(User.UserId(), ban) - ResponseMessage:Forbidden

			//// Act
			var result = _controller.GetB2BAdminsForBanAsync(ban);

			//// Assert
			Assert.IsNotNull(result);
		}

		// IsLegalOwnerRegistered(int ban)
		[Test]
		[TestCase(true)]
		public void IsLegalOwnerRegistered(bool isValidBan)
		{
			//// Arrange
			var ban = GetBan(isValidBan);

			// !_userLogic.UserConnectedToBan(User.UserId(), ban) - ResponseMessage:Forbidden

			//// Act
			var result = _controller.IsLegalOwnerRegistered(ban);

			//// Assert
			Assert.IsNotNull(result);
		}

		// GetSubscriptions(int ban)
		[Test]
		public void GetSubscriptions(bool isValidBan)
		{
			//// Arrange
			int ban = GetBan(isValidBan);

			// Simply finds all subscriptions matching Ban for current user (no internal error checking)

			// can return empty or one or more subscriptions
			// _subscriptionManager.GetSubscriptions(subscriptionIds) - IEnumerable<Subscription>

			//// Act
			var result = _controller.GetSubscriptions(ban);

			//// Assert
			Assert.IsNotNull(result);
		}

		// ValidateBanAgainstCVR(int ban, int cvr)
		[Test]
		public void ValidateBanAgainstCVR(bool isValidBan, bool isValidCvr)
		{
			//// Arrange
			var ban = GetBan(isValidBan);
			int cvr = GetCvr(isValidCvr);

			//// Act
			var result = _controller.ValidateBanAgainstCVR(ban, cvr);

			// response.IsValid
			// account = await _cdsManager.GetAccount(ban)

			// false (invalid) cases
			// account == null
			// account.AccountType != accountType
			// account.Status.Equals("C")
			// cvr != orgNo where ornNo = account.LegalOwner.Organization.OrganizationNumber


			//// Assert
			Assert.IsNotNull(result);
		}

		// GetPasswordForLux(string ban)
		[Test]
		public void GetPasswordForLux(bool isValidBan)
		{
			//// Arrange
			var ban = GetBanStr(isValidBan);

			// VERY STRANGE ARG: userId is ban

			// invokingUser, userId, userType 
			// _zeusClient.GetPassword("Switchr", ban, UserType.BAN)
			// new GetPasswordRequest() { InvokingUser = invokingUser, UserId = userId, UserType = userType };

			//// Act
			var result = _controller.GetPasswordForLux(ban);

			//// Assert
			Assert.IsNotNull(result);
		}

	}
}
