using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using Switchr.API.Areas.TopUp;
using Switchr.API.Tests.Common;
using Switchr.Logic.TopUp;
using Switchr.Models;

namespace Switchr.API.Tests.Controllers.TopUp
{
	class TopUpControllerTest : BaseControllerTest
	{
		protected IDictionary<Params, object> dict;

		protected ITopUpLogic _topUpLogic;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private TopUpController controller => GetController<TopUpController>();

		public T CreateTestControllerWith<T>(ITopUpLogic topUpLogic = null)
			where T : ApiController, new()
		{
			topUpLogic = useOrDefault<ITopUpLogic>(topUpLogic, _topUpLogic);
			return new TopUpController(_topUpLogic) as T;
		}

		[SetUp]
		public void SetUp()
		{
			_topUpLogic = GetInst<ITopUpLogic>();
			base.Setup();
		}

		override protected object GetLookupValue(Enum key)
		{
			return dict[(Params)key];
		}

		protected enum Params : int { }

		// AddTopUp(int subscriptionId, Request<AddTopUp> request)
		[Test]
		[TestCase(true)]
		// - Valid SubscriptionId
		public async Task AddTopUp(bool isValidSubscriptionId)
		{
			//// Arrange
			var subscriptionId = GetSubscriptionId(isValidSubscriptionId);
			//// Act
			var result = await controller.AvailableTopUps(subscriptionId);

			// ninjaService
			// topUpLogic.AvailableTopUps(User.UserId(), subscriptionId)
			// ninjaService.GetTopUpProducts(msisdn)

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual("Glemt", result.ViewBag.Title);
		}
	}
}
