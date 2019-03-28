using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using Switchr.API.Areas.User.Controllers;
using Switchr.Logic.UserAreaAccess.Interfaces;
using Switchr.Logic.Users.Interfaces;

namespace Switchr.API.Tests.Controllers.User
{
	class UserBanControllerTest: UserTest
	{
		private IUserBanService _userBanService;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private UserBanController controller => GetController<UserBanController>();

		protected T CreateTestControllerWith<T>(IUserBanService userBanService = null)
			where T : ApiController, new()
		{
			userBanService = useOrDefault<IUserBanService>(userBanService, _userBanService);
			return new UserBanController(userBanService) as T;
		}

		[SetUp]
		public void SetUp()
		{
			_userBanService = GetInst<IUserBanService>();
			base.Setup();
		}

		// Get(int userId)
		[Test]
		[TestCase(true)]
		public void Get(bool isValidUserId)
		{
			//// Arrange
			var userId = GetUserId(isValidUserId);

			// cdsService, userBanService

			// userBanService.GetBans(userId)
			// - userManager.GetUser(userId)
			// user is null
			// - SwitchrException(ErrorCode.InvalidUserId
			// cdsService.GetSubscriptionsForAccountIdAsync(banId)

			//// Act
			var result = controller.Get(userId);

			//// Assert
			Assert.IsNotNull(result);
		}
	}
}
