using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using FakeItEasy;
using NUnit.Framework;
using Switchr.API.Areas.Payment.Controllers;
using Switchr.API.Tests.Common;
using Switchr.Logic.Payment;
using Switchr.Models;

namespace Switchr.API.Tests.Controllers.Payment
{
	[TestFixture]
	public class PaymentControllerTest: BaseControllerTest
	{
		protected IDictionary<Params, object> dict;

		private IPaymentLogic _paymentLogic;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private PaymentController controller => GetController<PaymentController>();

		// Moq
		// - http://hamidmosalla.com/2017/08/03/moq-working-with-setupget-verifyget-setupset-verifyset-setupproperty/
		// Setup ASPNET using DI registry with s
		// - http://mscodingblog.blogspot.com/2015/11/making-setup-phase-of-unit-testing.html
		// - https://weblogs.asp.net/garrypilkington/setting-up-ioc-with-mvc-quickly
		// Custom DI container
		// - https://asp.net-hacker.rocks/2017/05/08/add-custom-ioc-in-aspnetcore.html
		protected T CreateTestControllerWith<T>(IPaymentLogic payment = null) where T : ApiController, new()
		{
			payment = payment != null ? payment : _paymentLogic;
			return new PaymentController(payment) as T;
		}

		[SetUp]
		public void SetUp()
		{
			_paymentLogic = GetInst<IPaymentLogic>();
			base.Setup();
		}

		override protected object GetLookupValue(Enum key)
		{
			return dict[(Params)key];
		}

		protected enum Params: int { }

		[Test]
		[TestCase(true, true, true, true)]
		public void AddPayment(bool isValidBan, bool isValidInvoice, bool isValidDepositDate, bool isValidAmount)
		{
			//// Arrange
			var ban = GetBanStr(isValidBan);
			var invoiceNumber = GetInvoiceNumber(isValidInvoice);
			var depositDate = GetDepositDate(isValidDepositDate);
			var amount = GetAmountStr(isValidAmount);

			// string.IsNullOrWhiteSpace(ban) || string.IsNullOrWhiteSpace(invoiceNumber) || string.IsNullOrWhiteSpace(amount)
			// SwitchrArgumentException(ErrorCode.InvalidParameter

			// paymentLogic
			// _paymentLogic.AddPayment(ban, amount, invoiceNumber, depositDate)

			// ninjaService
			// _ninjaService.AddPayment(ban, amount, invoiceNumber, depositDate)

			//// Act
			var result = controller.AddPayment(ban, invoiceNumber, depositDate, amount);

			//// Assert
			Assert.IsNotNull(result);
		}

		[Test]
		[TestCase(true)]
		public async Task CanChangeToPBS(bool isValidBan)
		{
			//// Arrange
			var ban = GetBan(isValidBan);

			// paymentLogic
			// _paymentLogic.CanChangeToPBS(ban)

			// ninjaGenericInterfaceClient, applicationSettings
			//  _ninjaGenericInterfaceClient.GenericNinja_ChangePaymentMethod

			//// Act
			var result = await controller.CanChangeToPBS(ban);

			//// Assert
			Assert.IsNotNull(result);
		}

		[Test]
		[TestCase(true)]
		public async Task ChangeToPBS(bool isValidBan)
		{
			var ban = GetBan(isValidBan);

			// paymentLogic.ChangeToPbs
			// applicationSettings

			//// Arrange			
			var request = GetRequest<CanChangeToPbsModel>();

			//// Act
			var result = await controller.ChangeToPBS(ban, request);

			//// Assert
			Assert.IsNotNull(result);
		}

		// SendPayoutInfo(string ban, Request<PayoutModel> sendPayoutInfo)
		[Test]
		[TestCase(true)]
		public void SendPayoutInfo(bool isValidBan, bool isValidPayoutModel)
		{

			//// Arrange			
			var ban = GetBanStr(isValidBan);
			var request = GetRequest<PayoutModel>(isValidPayoutModel);

			// sendPayoutInfo.Data.Email == null
			// SwitchrArgumentException(ErrorCode.InvalidEmail

			// paymentLogic
			// _paymentLogic.SendPayoutInfo

			//// Act
			var result = controller.SendPayoutInfo(ban, request);

			//// Assert
			Assert.IsNotNull(result);
		}

		
	}
}