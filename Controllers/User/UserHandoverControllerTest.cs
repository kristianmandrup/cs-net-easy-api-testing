using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using Switchr.API.Areas.User.Controllers;
using Switchr.Logic.Handover;

namespace Switchr.API.Tests.Controllers.User {
	class UserHandoverControllerTest : UserTest {
		private IHandoverLogic _handoverLogic;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private UserHandoverController controller => GetController<UserHandoverController>();

		protected T CreateTestControllerWith<T>(
			IHandoverLogic handoverLogic = null
		) where T : ApiController, new()
		{
			handoverLogic = useOrDefault<IHandoverLogic>(handoverLogic);
			return new UserHandoverController(handoverLogic) as T;
		}

		[SetUp]
		public void SetUp () {			
			// _handoverLogic = GetInst<IHandoverLogic> ();
			base.Setup();
		}

		// Get(int userId)
		[Test]
		[TestCase (true)]
		public async Task GenererateOtac (bool isValidUserId) {
			//// Arrange
			var userId = GetUserId (isValidUserId);

			// handoverLogic.GetReceiverStatus(userId)
			// handoverStatuList == null - NotFound() - NotFoundResult

			//// Act
			var result = await controller.Get (userId);

			//// Assert
			Assert.IsNotNull (result);
			// Assert.AreEqual("Glemt", result.ViewBag.Title);
		}

		// Get(int userId, string handoverId)
		[Test]
		[TestCase (true, true)]
		public async Task GetWithHandover (bool isValidUserId, bool isValidHandoverId) {
			//// Arrange
			var userId = GetUserId (isValidUserId);
			var handOverId = GetHandoverId (isValidHandoverId);

			// handoverLogic.GetHandoverStatusById(userId, handoverId)
			// handover == null - NotFound() - NotFoundResult

			//// Act
			var result = await controller.Get (userId, handOverId);

			//// Assert
			Assert.IsNotNull (result);
			// Assert.AreEqual("Glemt", result.ViewBag.Title);
		}
	}
}