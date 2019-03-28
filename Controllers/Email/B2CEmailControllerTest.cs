using NUnit.Framework;
using Switchr.API.Areas.Email;
using Switchr.Logic.Email;

namespace Switchr.API.Tests.Controllers.Email
{
	class B2CEmailControllerTest: EmailTest
	{
		private B2CEmailController _controller;

		public B2CEmailController CreateTestController(
			IEmailLogic email = null
		)
		{
			email = Eval<IEmailLogic>(Params.Email, email);
			return new B2CEmailController(email);
		}

		protected B2CEmailController CreateController()
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

			////// Act
			var result = _controller.Exists(email);

			////// Assert
			Assert.IsNotNull(result);
		}
	}
}
