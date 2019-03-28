using System;
using NUnit.Framework;
using Switchr.API.Areas.Ban.Controllers;
using Switchr.Logic.CreditAgreement;

namespace Switchr.API.Tests.Controllers.Ban
{
	class CreditAgreementControllerTest: BanTest
	{

		private CreditAgreementController _controller;

		public CreditAgreementController CreateCreditAgreementController(ICreditAgreementLogic creditAgreement = null)
		{
			creditAgreement = Eval<ICreditAgreementLogic>(Params.CreditAgreement, creditAgreement);
			return new CreditAgreementController(creditAgreement);
		}


		protected CreditAgreementController CreateController()
		{
			return CreateCreditAgreementController();
		}

		// GetCreditAgreements(string ban)
		[SetUp]
		public void SetUp()
		{
			base.Setup();
			_controller = CreateController();
		}

		[Test]
		[TestCase(true)]
		public void GetCreditAgreements(bool isValidBan)
		{
			//// Arrange
			var ban = GetBanStr(isValidBan);

			// no error handling in switchr - simply bubbles any exception from paymentProvider
			// var agreements = await _paymentProvider.GetAccountCreditAgreements(ban)

			//// Act
			var result = _controller.GetCreditAgreements(ban);

			//// Assert
			Assert.IsNotNull(result);
		}

		// PayoffCreditAgreementOnNextBill(string ban, string creditAgreementId)
		[Test]
		[TestCase(true, true)]
		public void PayoffCreditAgreementOnNextBill(bool isValidBan, bool isValidCreditAgreement)
		{
			//// Arrange
			var ban = GetBanStr(isValidBan);
			var creditAgreementId = GetCreditAgreementId(isValidCreditAgreement);


			// PayoffCreditAgreementOnNextBill(string ban, string creditagreementId) - ALWAYS simply returns TRUE!!
			// Currently no way to get a negative result or handle invalid args etc

			//// Act
			var result = _controller.PayoffCreditAgreementOnNextBill(ban, creditAgreementId);

			//// Assert
			Assert.IsNotNull(result);
		}

	}
}
