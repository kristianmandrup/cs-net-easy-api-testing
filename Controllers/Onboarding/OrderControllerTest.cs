using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using Switchr.API.Areas.Onboarding.Controllers;
using Switchr.API.Infrastructure.Hateoas.Interfaces;
using Switchr.API.Tests.Common;
using Switchr.Data.Interfaces;
using Switchr.Logic.Onboarding.Interfaces;

namespace Switchr.API.Tests.Controllers.Onboarding
{
	class OrderControllerTest: BaseControllerTest
	{
		protected IDictionary<Params, object> dict;

		private IOnboardingLogic _onboarding;
		private IHateoasHandlerFactory _hateosFactory;
		private IUserManager _userManager;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private OrderController controller => GetController<OrderController>();

		public T CreateTestControllerWith<T>(
			IOnboardingLogic onboarding = null, 
			IHateoasHandlerFactory hateosFactory = null,
			IUserManager userManager = null) where T : ApiController, new()
		{
			onboarding = Eval<IOnboardingLogic>(Params.Onboarding, onboarding);
			hateosFactory = Eval<IHateoasHandlerFactory>(Params.HateoasHandlerFactory, hateosFactory);
			userManager = Eval<IUserManager>(Params.UserManager, userManager);

			return new OrderController(onboarding, hateosFactory, userManager) as T;
		}

		[SetUp]
		public void SetUp()
		{			
			// IOnboardingLogic onboarding
			_onboarding = GetInst<IOnboardingLogic>();
			// IHateoasHandlerFactory _hateosFactory;
			_hateosFactory = GetInst<IHateoasHandlerFactory>();
			// IUserManager _userManager ;
			_userManager = GetInst<IUserManager>();

			dict = new Dictionary<Params, object>()
			{
				{Params.Onboarding, _onboarding},
				{Params.HateoasHandlerFactory, _hateosFactory},
				{Params.UserManager, _userManager},
			};

			base.Setup();
		}

		override protected object GetLookupValue(Enum key)
		{
			return dict[(Params)key];
		}

		public enum Params : int
		{
			Onboarding,
			HateoasHandlerFactory,
			UserManager
		}

		// Orders()
		[Test]
		// GET JsonSuccessResponse<List<OnboardingModel>>
		public async Task Orders()
		{
			////// Arrange

			// _onboardingLogic.GetOrders(User.UserId())

			// onboardingLogic, userManager
			// _userManager.GetUser(userId)
			// user == null - SwitchrForbiddenException(ErrorCode.InvalidUserId

			//// Act
			var result = await controller.Orders();

			//// Assert
			Assert.IsNotNull(result);
		}

		// OrderLines(string orderId)
		[Test]
		// GET JsonSuccessResponse<IEnumerable<OrderGuide>>
		public async Task OrderLines(bool isValidOrderId)
		{
			////// Arrange
			var orderId = GetOrderId(isValidOrderId);

			// _onboardingLogic.GetOrderGuides(userId, orderId)

			// onboardingLogic, userManager
			// _userManager.GetUser(userId)
			// user == null - SwitchrForbiddenException(ErrorCode.InvalidUserId

			//// Act
			var result = await controller.OrderLines(orderId);

			//// Assert
			Assert.IsNotNull(result);
		}


		// GetOrderActionDetails(string orderId, string orderAction)
		[Test]
		public async Task GetOrderActionDetails(bool isValidOrderId, bool isValidOrderAction)
		{
			////// Arrange
			var orderId = GetOrderId(isValidOrderId);
			var orderAction = GetOrderAction(isValidOrderAction);

			//// Act
			var result = await controller.GetOrderActionDetails(orderId, orderAction);

			//// Assert
			Assert.IsNotNull(result);
		}

		// CompleteGuideLine(string orderId, string orderAction)
		[Test]
		public async Task CompleteGuideLine(bool isValidOrderId, bool isValidOrderAction)
		{
			////// Arrange
			var orderId = GetOrderId(isValidOrderId);
			var orderAction = GetOrderAction(isValidOrderAction);

			// GetOrderById(orderId, userId)
			// currentOrder == null || currentOrder.Id != orderId
			// - SwitchrForbiddenException(ErrorCode.CouldNotFindOnboardingGuide

			// _completedOrderActionManager.CompletedOnboardingGuide(orderId, actionId, statusId).Any()
			// - SwitchrForbiddenException(ErrorCode.OnboardingGuideAlreadyCompleted

			// _completedOrderActionManager.CompleteOnboardingGuide(userId, orderId, actionId, statusId, DateTime.UtcNow)
			// - SwitchrForbiddenException(ErrorCode.ErrorCompletingOnboardingGuide

			//// Act
			var result = await controller.CompleteGuideLine(orderId, orderAction);

			//// Assert
			Assert.IsNotNull(result);
		}


		// GetReturnLabel(string orderId, string deliveryId)
		[Test]
		public async Task GetReturnLabel(bool isValidOrderId, bool isValidDeliveryId)
		{
			////// Arrange
			var orderId = GetOrderId(isValidOrderId);
			var deliveryId = GetDeliveryId(isValidDeliveryId);


			// onboardingLogic
			// WARNING: Spelling error!
			// _onboardingLogic.GetRetunLabel(orderId, User.UserId(), deliveryId)

			// userManager
			// _userManager.GetUser(userId)
			// user == null - SwitchrForbiddenException(ErrorCode.InvalidUserId

			// trackandTraceLogic
			// _trackandTraceLogic.GetRetunLabel(orderId, user.Name, user.LastName, userId, deliveryId, (Brand) user.Brand.Id,
			// (Segment)user.Segment.Id)

			//// Act
			var result = await controller.GetReturnLabel(orderId, deliveryId);

			//// Assert
			Assert.IsNotNull(result);
		}
	}
}
