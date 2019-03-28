using NUnit.Framework;
using Switchr.API.Areas.User.Controllers;
using Switchr.Logic.Users;
using Switchr.Models;
using Switchr.Models.User;
using System.Web.Http;

namespace Switchr.API.Tests.Controllers.User
{
	class B2BUserControllerTest: UserTest
	{
		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private B2BUserController controller => GetController<B2BUserController>();

		protected T CreateTestControllerWith<T>(PasswordLogic password = null) where T : ApiController, new()
		{
			password = Eval<PasswordLogic>(Params.Password, password);			
			return new B2BUserController(password) as T;
		}

		[SetUp]
		public void SetUp()
		{
			base.Setup();
		}

		[Test]
		[TestCase(true)]
		public void ForgotPassword(bool isValidRequest)
		{
			//// Arrange
			var passwordResetDetails = GetPasswordResetDetails(isValidRequest);

			// passwordResetDetails?.Data == null
			// - SwitchrArgumentException(ErrorCode.InvalidParameter

			//// Act
			var result = controller.ForgotPassword(passwordResetDetails);

			// Ok(new SuccessResponse<bool>(true))

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual("Glemt", result.ViewBag.Title);
		}
	}
}
