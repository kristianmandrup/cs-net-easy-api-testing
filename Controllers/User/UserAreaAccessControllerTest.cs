using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using Switchr.API.Areas.User.Controllers;
using Switchr.Logic.UserAreaAccess.Interfaces;

namespace Switchr.API.Tests.Controllers.User
{
	class UserAreaAccessControllerTest: UserTest
	{
		private IUserAreaAccessService _userAreaAccessService;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private UserAreaAccessController controller => GetController<UserAreaAccessController>();

		protected T CreateTestControllerWith<T>(IUserAreaAccessService userAreaAccessService = null)
			where T : ApiController, new()
		{
			userAreaAccessService = useOrDefault<IUserAreaAccessService>(userAreaAccessService, _userAreaAccessService);
			return new UserAreaAccessController(userAreaAccessService) as T;
		}

		[SetUp]
		public void SetUp()
		{
			_userAreaAccessService = GetInst<IUserAreaAccessService>();
			base.Setup();
		}

		// GenererateOtac()
		[Test]
		[TestCase(true)]
		public void Get(bool isValidUserId)
		{
			//// Arrange
			var userId = GetUserId(isValidUserId);

			// userAreaAccessService.Get
			// - userManager.GetUser(userId) (DB)

			//// Act
			var result = controller.Get(userId);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual("Glemt", result.ViewBag.Title);
		}
	}
}
