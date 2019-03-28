using System.Web.Http;
using System.Web.Mvc;
using FakeItEasy;
using NUnit.Framework;
using Switchr.API.Areas.User.Controllers;
using Switchr.Logic.Users;
using Switchr.Models;
using Switchr.Models.User;

namespace Switchr.API.Tests.Controllers.User
{
	class B2CUserControllerTest: UserTest
	{
		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private B2CUserController controller => GetController<B2CUserController>();

		protected T CreateTestControllerWith<T>(IPasswordLogic password = null) where T : ApiController, new()
		{
			password = Eval<IPasswordLogic>(Params.Password, password);
			return new B2CUserController(password) as T;
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
