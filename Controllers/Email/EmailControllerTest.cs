using NUnit.Framework;
using Switchr.API.Areas.Email;
using Switchr.Logic.Email;

namespace Switchr.API.Tests.Controllers.Email
{
	class EmailControllerTest: EmailTest
	{

		private EmailController _controller;

		public EmailController CreateTestController(
			IEmailLogic email = null,
			ITeliaTVUsernameLogic teliaTvUsername = null
		)
		{
			email = Eval<IEmailLogic>(Params.Email, email);
			teliaTvUsername = Eval<ITeliaTVUsernameLogic>(Params.TeliaTVUsername, teliaTvUsername);
			
			return new EmailController(email, teliaTvUsername);
		}

		protected EmailController CreateController()
		{
			return CreateTestController();
		}

		[SetUp]
		public void SetUp()
		{
			base.Setup();
			_controller = CreateController();
		}

		// CheckUsername(string username)
		[Test]
		[TestCase(true)]
		public void CheckUsername(bool isValidUsername)
		{
			////// Arrange
			var username = GetUsername(isValidUsername);

			// _teliaTVUsernameLogic
			// available = _teliaTVUsernameLogic.UsernameAvailable(username)

			////// Act
			var result = _controller.CheckUsername(username);

			////// Assert
			Assert.IsNotNull(result);
		}
	}
}
