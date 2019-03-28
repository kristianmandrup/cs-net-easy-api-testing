using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using Switchr.API.Areas.User.Controllers;
using Switchr.Data.Interfaces;
using Switchr.Logic.Services.Cds.interfaces;
using Switchr.Models;

namespace Switchr.API.Tests.Controllers.User {
	class UserCreditAgreementControllerTest : UserTest {
		private IUserManager _userManager;
		private ICdsManager _cdsManager;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private UserCreditAgreementController controller => GetController<UserCreditAgreementController>();

		protected T CreateTestControllerWith<T>(
			IUserManager userManager = null,
			ICdsManager cdsManager = null
		) where T : ApiController, new()
		{
			// NOTE: will use GetInst<T> by default to create dependency
			userManager = useOrDefault<IUserManager>(userManager);
			cdsManager = useOrDefault<ICdsManager>(cdsManager);

			return new UserCreditAgreementController (userManager, cdsManager) as T;
		}

		[SetUp]
		public void SetUp () {
			// custom dependencies if needed
			//_userManager = GetInst<IUserManager> ();
			//_cdsManager = GetInst<ICdsManager> ();

			base.Setup();
		}

		// GetHasCreditAgreement(int userId)
		[Test]
		[TestCase (true)]
		public async Task GenererateOtac (bool isValidUserId) {
			//// Arrange
			var userId = GetUserId (isValidUserId);

			// userManager.GetUser(userId)

			// Missing check if user returned or not!

			// cdsManager.GetCreditAccounts(user.SSN)
			// Enumerable.Empty<string>() if no accounts returned

			// cdsService.GetCreditAccounts(ssn)
			// - jsonClient.Get cdsUrl


			//// Act
			var result = await controller.GetHasCreditAgreement (userId);

			//// Assert
			Assert.IsNotNull (result);
			// Assert.AreEqual("Glemt", result.ViewBag.Title);
		}
	}
}