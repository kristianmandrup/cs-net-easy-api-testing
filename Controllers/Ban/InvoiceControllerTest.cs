using Switchr.API.Areas.Ban.Controllers;
using System;
using NUnit.Framework;
using Switchr.Logic.Invoice;

namespace Switchr.API.Tests.Controllers.Ban
{
	class InvoiceControllerTest : BanTest
	{

		private InvoiceController _controller;

		public InvoiceController CreateInvoiceController(IInvoiceService invoiceService = null)
		{
			invoiceService = Eval<IInvoiceService>(Params.InvoiceService, invoiceService);
			return new InvoiceController(invoiceService);
		}


		protected InvoiceController CreateController()
		{
			return CreateInvoiceController();
		}

		// GetCreditAgreements(string ban)
		[SetUp]
		public void SetUp()
		{
			base.Setup();
			_controller = CreateController();
		}

		// GetInvoices(string ban)
		[Test]
		[TestCase(true, true)]
		public void GetInvoices(bool isValidBan)
		{
			//// Arrange
			var ban = GetBanStr(isValidBan);

			// invoiceService.GetInvoices(User.UserId(), ban)

			//// Act
			var result = _controller.GetInvoices(ban);

			//// Assert
			Assert.IsNotNull(result);
		}

		// GetInvoice(string ban, string invoiceNumber)
		[Test]
		[TestCase(true, true)]
		public void GetInvoice(bool isValidBan, bool isValidInvoiceNumber)
		{
			//// Arrange
			var ban = GetBanStr(isValidBan);
			var invoiceNumber = GetInvoiceNumber(isValidInvoiceNumber);

			// // invoiceService.GetInvoice(User.UserId(), ban, invoiceNumber)

			//// Act
			var result = _controller.GetInvoice(ban, invoiceNumber);

			//// Assert
			Assert.IsNotNull(result);
		}

		// ValidateInvoicePaymentParameters(string ban, string invoiceNumber, int userId, string amount)
		[Test]
		[TestCase(true, true)]
		public void ValidateInvoicePaymentParameters(bool isValidBan, bool isValidInvoiceNumber, bool isValidUserId, bool isValidAmount)
		{
			//// Arrange
			var ban = GetBanStr(isValidBan);
			var invoiceNumber = GetInvoiceNumber(isValidInvoiceNumber);
			var userId = GetUserId(isValidUserId);
			var amount = GetAmountStr(isValidAmount);

			// invoiceService.ValidateInvoicePaymentParameters(User.UserId(), ban, invoiceNumber, userId, amount);

			//// Act
			var result = _controller.ValidateInvoicePaymentParameters(ban, invoiceNumber, userId, amount);

			//// Assert
			Assert.IsNotNull(result);
		}

		[Test]
		[TestCase(true, true)]
		public void DownloadInvoice(bool isValidBan, bool isValidInvoiceNumber)
		{
			//// Arrange
			var ban = GetBanStr(isValidBan);
			var invoiceNumber = GetInvoiceNumber(isValidInvoiceNumber);

			// pdfStream == null
			// Error(HttpStatusCode.NotFound.ToString(), ErrorCode.InvoiceNotFound

			// catch (HttpRequestException ex)
			// new Error(HttpStatusCode.NotFound.ToString(), ErrorCode.InternalServerError

			//// Act
			var result = _controller.DownloadInvoice(ban, invoiceNumber);

			//// Assert
			Assert.IsNotNull(result);
		}
	}
}
