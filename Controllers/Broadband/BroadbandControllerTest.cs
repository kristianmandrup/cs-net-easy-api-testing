using System;
using NUnit.Framework;
using Switchr.API.Areas.Broadband.Controllers;
using Switchr.API.Tests.Controllers.Ban;
using Switchr.Logic.Broadband;
using Switchr.Logic.Exceptions;

namespace Switchr.API.Tests.Controllers.Broadband
{
	class BroadbandControllerTest: BroadBandTest
	{
		private BroadbandController _controller;

		public BroadbandController CreateTestController(IBroadbandInstallationLogic broadbandInstallation = null)
		{
			broadbandInstallation = Eval<IBroadbandInstallationLogic>(Params.Broadband, broadbandInstallation);
			return new BroadbandController(broadbandInstallation);
		}

		protected BroadbandController CreateController()
		{
			return CreateTestController();
		}

		[SetUp]
		public void SetUp()
		{
			base.Setup();
			_controller = CreateController();
		}

		// GetCreditAgreements(int userId)
		[Test]
		[TestCase(true)]
		public void GetCreditAgreements(bool isValidUserId)
		{
			//// Arrange
			var userId = GetUserId(isValidUserId);

			// User.UserId() != userId - ResponseMessage:HttpStatusCode.Forbidden
			// broadbandSubscription = await _broadbandInstallationLogic.GetBroadbandInstallations(userId)
			// broadbandSubscription == null - NotFound()

			//// Act
			var result = _controller.Get(userId);

			//// Assert
			Assert.IsNotNull(result);
		}
	}
}
