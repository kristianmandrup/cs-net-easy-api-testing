using NUnit.Framework;
using Switchr.API.Areas.Email;
using Switchr.Data.Interfaces;
using Switchr.Logic.Email;
using Switchr.Logic.Services.Switchr;

namespace Switchr.API.Tests.Controllers.Email
{
	class B2BEmailControllerTest: EmailTest
	{
		private B2BEmailController _controller;

		public B2BEmailController CreateTestController(
			IEmailLogic email = null
		)
		{
			email = Eval<IEmailLogic>(Params.Email, email);
			return new B2BEmailController(email);
		}

		protected B2BEmailController CreateController()
		{
			return CreateTestController();
		}

		[SetUp]
		public void SetUp()
		{
			base.Setup();
			_controller = CreateController();
		}

		// Validate(string email)
		[Test]
		[TestCase(true)]
		public void Validate(bool isValidEmail)
		{
			////// Arrange
			var email = GetEmail(isValidEmail);

			// isValid = email != null && _emailLogic.ValidateEmail(email, Segment.Business, Role.Subscriber, brand)

			////// Act
			var result = _controller.Validate(email);

			////// Assert
			Assert.IsNotNull(result);
		}

		// Exists(string email)
		[Test]
		[TestCase(true)]
		public void Exists(bool isValidEmail)
		{
			////// Arrange
			var email = GetEmail(isValidEmail);

			// email == null - SwitchrArgumentException(ErrorCode.InvalidEmail
			// _emailLogic.EmailExists(System.Uri.UnescapeDataString(email), Segment.Business, Role.Subscriber, brand)

			////// Act
			var result = _controller.Exists(email);

			////// Assert
			Assert.IsNotNull(result);
		}
	}
}
