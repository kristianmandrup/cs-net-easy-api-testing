using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using Switchr.API.Areas.Subscription.Controllers;
using Switchr.Data.Interfaces;
using Switchr.Logic.Accounts;
using Switchr.Logic.Services.Switchr.Interfaces;
using Switchr.Logic.Sim;
using Switchr.Logic.Subscriptions;
using Switchr.Logic.Validation;
using Switchr.Models;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using StructureMap;
using Switchr.API.Areas.User.Controllers;
using Switchr.API.Infrastructure.IoC;
using Switchr.API.Tests.Common;

namespace Switchr.API.Tests.Controllers
{
	[TestFixture]
	public class SubscriptionControllerTest : SubscriptionTest
	{
		private SubscriptionController _controller;


		/// <summary>
		/// Allows fine grained control over how the controller is instantiated for a particular test
		/// Each of the optional arguments lets you override the default mock that will be used if arg not passed
		/// </summary>
		/// <param name="subscriptionVas"></param>
		/// <param name="userValidation"></param>
		/// <param name="subscriptionService"></param>
		/// <param name="subscription"></param>
		/// <param name="orderSim"></param>
		/// <param name="subscriptionStatus"></param>
		/// <param name="subscriptionOffering"></param>
		/// <param name="changeSubscription"></param>
		/// <param name="userManager"></param>
		/// <param name="catalogService"></param>
		/// <param name="account"></param>
		/// <returns></returns>
		public SubscriptionController CreateTestController(
			SubscriptionVasLogic subscriptionVas = null,
			UserValidationLogic userValidation = null,
			ISubscriptionService subscriptionService = null,
			ISubscriptionLogic subscription = null,
			IOrderSimLogic orderSim = null,
			ISubscriptionStatusLogic subscriptionStatus = null,
			ISubscriptionOfferingLogic subscriptionOffering = null,
			IChangeSubscriptionLogic changeSubscription = null,
			IUserManager userManager = null,
			ICatalogService catalogService = null,
			IAccountLogic account = null
		)
		{
			subscriptionVas = Eval<SubscriptionVasLogic>(Params.SubscriptionVas, subscriptionVas);
			userValidation = Eval<UserValidationLogic>(Params.UserValidation, userValidation);
			subscriptionService = Eval<ISubscriptionService>(Params.SubscriptionService, subscriptionService);
			subscription = Eval<ISubscriptionLogic>(Params.Subscription, subscription);
			orderSim = Eval<IOrderSimLogic>(Params.OrderSim, orderSim);
			subscriptionStatus = Eval<ISubscriptionStatusLogic>(Params.SubscriptionStatus, subscriptionStatus);
			subscriptionOffering = Eval<ISubscriptionOfferingLogic>(Params.SubscriptionOffering, subscriptionOffering);
			changeSubscription = Eval<IChangeSubscriptionLogic>(Params.ChangeSubscription, changeSubscription);
			userManager = Eval<IUserManager>(Params.UserManager, userManager);
			catalogService = Eval<ICatalogService>(Params.CatalogService, catalogService);
			account = Eval<IAccountLogic>(Params.Account, account);
			return new SubscriptionController(
				subscriptionVas, 
				userValidation,
				subscriptionService,
				subscription, 
				orderSim,
				subscriptionStatus, 
				subscriptionOffering,
				changeSubscription,
				userManager,
				catalogService,
				account
			);
		}

		protected SubscriptionController CreateController()
		{
			return CreateTestController();
		}

		[SetUp]
		public void SetUp()
		{
			base.Setup();
			_controller = CreateController();
		}


		[Test]
		[TestCase(true, false)]
		// - Valid Subscription
		// - Invalid OrderSim
		public async Task OrderNewSimCard(bool isValidSubscription, bool isValidOrderSim)
		{
			//// Arrange
			var subscriptionId = GetSubscriptionId(isValidSubscription);
			var orderSimInfo = GetOrderSimInfo(isValidOrderSim);

			// orderSimLogic, handoverLogic, subscriptionService, etrayProxy

			// orderSimLogic.OrderSim(userId, subscriptionId, request.Data)
			// - handoverLogic.GetSubmitterStatus(userId, subscriptionId))
			// - subscriptionService.GetSubscriptionData(subscriptionId)
			// - etrayProxy.SendOrderNewSimMail(

			//// Act
			var result = await _controller.OrderNewSimCard(subscriptionId, orderSimInfo);

			//// Assert
			Assert.IsNotNull(result);

			// Model Invalid (ie bad Request)
			// ResponseMessageResult.StatusCode  == HttpStatusCode.BadRequest (400)
			// ResponseMessageResult.Response = HttpResponseMessage with Content that is the list of Error (ModelState validation)
			// otherwise: SwitchrArgumentException(ErrorCode.InvalidParameter) .Code : ErrorCode

			// AssertResult(result, "OrderNewSimCard")

			//var checker = ResponseChecker.Create(result, "OrderNewSimCard");
			//Assert.IsTrue(checker.IsResponseInvalid);
			//Assert.IsTrue(checker.IsBadRequest);
		}

		[Test]
		// [TestCase(true, true)] // TODO
		[TestCase(true, false)]
		public async Task GetSubscriptionAdditionalInfo(bool isValidSubscription, bool isValidProductCode)
		{
			var subscriptionId = GetSubscriptionId(isValidSubscription);
			var productCode = GetProductCode(isValidProductCode);

			// userValidationLogic.ThrowIfUserHaveNotClaimedSubscription(User.UserId(), subscriptionId)
			// - userManager.GetUser(userId) (DB)
			// subscriptionVasLogic.GetAdditionalInfo(subscriptionId, productCode, callbackUrl, contextId)
			// - vasManager.GetValueAddedService(productCode)
			// - subscriptionVASAdditionalInfoManager.GetAdditionalInfo(subscriptionId, service.ServiceName, productCode)
			// - GetThirdPartiApiAdditionalInfo(subscriptionId, productCode)

			var result = await _controller.GetSubscriptionAdditionalInfo(subscriptionId, productCode);

			// AssertResult(result, expectation)

			// var checker = ResponseChecker.Create(result, "GetSubscriptionAdditionalInfo");

			// TODO: assert logic should auto-reflect TestCase data to avoid duplication

			// TODO: unify assertions under one method
			Assert.IsNotNull(result);
			//Assert.IsTrue(checker.IsResponseInvalid);
			//Assert.IsTrue(checker.IsBadRequest);

			//// ModelState validation error (arguments)
			//// test that one of the error messages is regarding the product
			//Assert.IsTrue(checker.IsInvalidResult(errorMsg: "prod"));

			// for errors not caught by ModelState
			// Assert.IsTrue(checker.IsInvalidResultWith(code: ErrorCode.InvalidParameter));
		}

		[Test]
		[TestCase(true)]
		public async Task GetSubscriptionDetails(bool isValidSubscriptionId)
		{
			var subscriptionId = GetSubscriptionId(isValidSubscriptionId);

			// subscriptionLogic.GetSubscriptionDetails(subscriptionId, User.UserId())
			// - accessRightLogic.IsLegalOwnerOrHasAccessRightsOrHasClaimed(userId, subscriptionId))
			// -- SwitchrForbiddenException(ErrorCode.ErrorWhenAccesstoSubscription

			// - subscriptionService.GetSubscriptionDataWithProducts(subscriptionId)
			// -- SwitchrNotFoundException(ErrorCode.SubscriptionDetailsNotFound

			// - cbmService.GetDataQuota(subscriptionData.Ban.ToString(), subscriptionData.Msisdn, UsageConstants.DataUnit.Mb)

			var result = await _controller.GetSubscriptionDetails(subscriptionId);
			Assert.IsNotNull(result);
		}

		[Test]
		[TestCase(true)]
		public async Task Suspend(bool isValidSubscriptionId)
		{	
			var subscriptionId = GetSubscriptionId();

			// subscriptionStatusLogic.Suspend(User.UserId(), subscriptionId)
			// - subscriptionManager.GetSubscription(subscriptionId) - SwitchrEntities (DB)

			// CdsService
			// - cdsManager.GetSubscription(subscriptionId)

			// - SwitchrArgumentException(ErrorCode.SubscriptionNotActive

			var result = await _controller.Suspend(subscriptionId);
			Assert.IsNotNull(result);
		}

		[Test]
		[TestCase(true)]
		public async Task Reactivate(bool isValidSubscriptionId)
		{
			var subscriptionId = GetSubscriptionId();

			// subscriptionStatusLogic.Reactivate(User.UserId(), subscriptionId)

			// - subscriptionManager.GetSubscription(subscriptionId) - SwitchrEntities (DB)

			// CdsService
			// - cdsManager.GetSubscription(subscriptionId)

			var result = await _controller.Reactivate(subscriptionId);
			Assert.IsNotNull(result);
		}

		[Test]
		[TestCase(true)]
		public async Task Offerings(bool isValidSubscriptionId)
		{
			var subscriptionId = GetSubscriptionId();

			// subscriptionOfferingLogic.GetOfferings((Models.Enums.Segment)dbUser.Segment.Id, (Models.Enums.Role)dbUser.Role.Id, brand, subscriptionId)

			// subscription == null
			// - SwitchrNotFoundException(ErrorCode.SubscriptionDetailsNotFound

			// subscription.SubscriptionStatus != SubscriptionStatus.Active
			// - SwitchrArgumentException(ErrorCode.SubscriptionNotSuspended
			// subscription.TelePhoneNumber == null
			// - SwitchrArgumentException(ErrorCode.InvalidSubscription

			// subscriptionDetailsByMsisdn == null
			// - SwitchrNotFoundException(ErrorCode.InvalidSubscription

			// catalogService.GetAllValueAddedServices(segment, role, brand)

			var result = await _controller.Offerings(subscriptionId);
			Assert.IsNotNull(result);
		}

		[Test]
		[TestCase(true)]
		public async Task OfferingsV1(bool isValidSubscriptionId)
		{
			var subscriptionId = GetSubscriptionId();

			// subscriptionOfferingLogic.GetOfferings((Models.Enums.Segment)dbUser.Segment.Id, (Models.Enums.Role)dbUser.Role.Id, brand, subscriptionId)

			// subscription == null
			// - SwitchrNotFoundException(ErrorCode.SubscriptionDetailsNotFound

			// subscription.SubscriptionStatus != SubscriptionStatus.Active
			// - SwitchrArgumentException(ErrorCode.SubscriptionNotSuspended
			// subscription.TelePhoneNumber == null
			// - SwitchrArgumentException(ErrorCode.InvalidSubscription

			// subscriptionDetailsByMsisdn == null
			// - SwitchrNotFoundException(ErrorCode.InvalidSubscription

			// catalogService.GetAllValueAddedServices(segment, role, brand)

			var result = await _controller.OfferingsV1(subscriptionId);
			Assert.IsNotNull(result);
		}
	}
}