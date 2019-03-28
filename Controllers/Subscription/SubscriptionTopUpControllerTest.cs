using NUnit.Framework;
using System.Threading.Tasks;
using Switchr.API.Areas.Subscription.Controllers;
using Switchr.Models;

namespace Switchr.API.Tests.Controllers.User
{
	class SubscriptionTopUpControllerTest : SubscriptionTest
	{
	
		private SubscriptionTopUpController _controller;

		protected SubscriptionTopUpController createController()
		{
			return new SubscriptionTopUpController(_topUpLogic, _userManager);
		}

		[SetUp]
		public void SetUp()
		{			
			_controller = createController();
		}

		// AddTopUp(int subscriptionId, Request<AddTopUp> request)
		[Test]
		// TopUp, SubscriptionId
		[TestCase(true, true)]
		[TestCase(false, true)]
		public async Task AddTopUp(bool isValidTopUp, bool isValidSubscriptionId)
		{
			//// Arrange
			var addTopUpRequest = GetTopUpRequest(isValidTopUp);
			var subscriptionId = GetSubscriptionId(isValidSubscriptionId);

			// topUpLogic.AddTopUp(User.UserId(), subscriptionId, request.Data)
			// - handoverLogic.GetSubmitterStatus(userId, subscriptionId))?.IsPendingBpm == true)
			// .IsPendingBpm
			// SwitchrForbiddenException(ErrorCode.SubscriptionIsPendingHandover
			// hasUnlimitedData
			// SwitchrForbiddenException(ErrorCode.CanNotBuyExtraDataForUnlimitedDataSubscription
			// VerifyPassword: false
			// SwitchrArgumentException(ErrorCode.InvalidPassword

			//// Act
			var result = await _controller.AddTopUp(subscriptionId, addTopUpRequest);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual("Glemt", result.ViewBag.Title);
		}
	}
}
