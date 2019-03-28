using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using FakeItEasy;
using NUnit.Framework;
using Switchr.API.Areas.User.Controllers;
using Switchr.Logic.Menu;

namespace Switchr.API.Tests.Controllers.User
{
	class MenuControllerTest: UserTest
	{
		IMenuLogic _menu;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private MenuController controller => GetController<MenuController>();

		protected T CreateTestControllerWith<T>(IMenuLogic menu = null) where T : ApiController, new()
		{
			menu = useOrDefault<IMenuLogic>(menu, _menu);
			return new MenuController(menu) as T;

		}

		[SetUp]
		public void SetUp()
		{
			_menu = GetInst<IMenuLogic>();
			base.Setup();
		}

		[Test]
		public async Task GetAllowedMenuItems()
		{
			//// Arrange
			//// Act
			var result = await controller.GetAllowedMenuItems();

			// menuLogic.GetAllowedMenuItems
			// - banAccessRightsLogic.IsLegalOwnerOrDelegateForAnyBan(userId)
			// - userManager.GetUser(userId)
			// - subscriptionManager.GetSubscriptionsByUserId(userId)
			// - cdsService.GetBansForSsn(user.SSN)
			// - userLegacyBroadbandService.HasBroadband(userId)
			// - handoverLogic.GetReceiverStatus(userId)
			// - userVASLogic.ValueAddedServiceExistsOnUser(userId))

			// NOTE: No Exception handling in MenuLogic

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual("Glemt", result.ViewBag.Title);
		}
	}
}
