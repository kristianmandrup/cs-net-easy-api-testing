using System;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using Switchr.API.Areas.User.Controllers;
using Switchr.API.Infrastructure.Hateoas.Interfaces;
using Switchr.Data.Interfaces;
using Switchr.Logic.Common;
using Switchr.Logic.Exceptions;
using Switchr.Logic.Helpers;
using Switchr.Logic.Subscriptions;
using Switchr.Logic.Users;
using Switchr.Logic.Validation;
using Switchr.Models;
using Switchr.Models.Enums;
using Switchr.Models.Subscription;
using Switchr.Models.VAS;

namespace Switchr.API.Tests.Controllers.User {
	class ValueAddedServicesControllerTest : UserTest {
		private UserValidationLogic _userValidationLogic;
		private UserServicesLogic _userServicesLogic;
		private UserVASLogic _userVasLogic;
		private TeliaTvLogic _teliaTvLogic;
		private UserValueAddedServiceStatusLogic _userVasStatusLogic;
		private SubscriptionVasLogic _subscriptionVasLogic;
		private IHateoasHandlerFactory _hateoasHandlerFactory;
		private IUserManager _userManager;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private ValueAddedServicesController controller => GetController<ValueAddedServicesController>();

		protected T CreateTestControllerWith<T>(
			UserValidationLogic userValidationLogic = null,
			UserServicesLogic userServicesLogic = null,
			UserVASLogic userVasLogic = null,
			TeliaTvLogic teliaTvLogic = null,
			UserValueAddedServiceStatusLogic userVasStatusLogic = null,
			SubscriptionVasLogic subscriptionVasLogic = null,
			IUserManager userManager = null,
			IHateoasHandlerFactory hateoasHandlerFactory = null
		) where T : ApiController, new()
		{
			_userValidationLogic = userValidationLogic ?? _userValidationLogic;
			_userServicesLogic = userServicesLogic ?? _userServicesLogic;
			_userVasLogic = userVasLogic ?? _userVasLogic;
			_teliaTvLogic = teliaTvLogic ?? _teliaTvLogic;
			_userVasStatusLogic = userVasStatusLogic ?? _userVasStatusLogic;
			_subscriptionVasLogic = subscriptionVasLogic ?? _subscriptionVasLogic;
			_userManager = userManager ?? _userManager;
			_hateoasHandlerFactory = hateoasHandlerFactory ?? _hateoasHandlerFactory;
			return new ValueAddedServicesController (
				_userValidationLogic,
				_userServicesLogic,
				_userVasLogic,
				_teliaTvLogic,
				_userVasStatusLogic,
				_subscriptionVasLogic,
				_userManager,
				_hateoasHandlerFactory
			) as T;
		}

		[SetUp]
		public void SetUp () {			
			_userValidationLogic = GetInst<UserValidationLogic> ();
			_userServicesLogic = GetInst<UserServicesLogic> ();
			_userVasLogic = GetInst<UserVASLogic> ();
			_teliaTvLogic = GetInst<TeliaTvLogic> ();
			_userVasStatusLogic = GetInst<UserValueAddedServiceStatusLogic> ();
			_subscriptionVasLogic = GetInst<SubscriptionVasLogic> ();
			_hateoasHandlerFactory = GetInst<IHateoasHandlerFactory> ();
			_userManager = GetInst<IUserManager> ();

			base.Setup();
		}

		// Get (int subscriptionId)
		[Test]
		[TestCase(true, true, true)]
		public async Task Get(bool isValidSubscriptionId)
		{
			//// Arrange			
			var subscriptionId = GetSubscriptionId(isValidSubscriptionId);

			// userServicesLogic.GetValueAddedServices (User.UserId (), subscriptionId)
			// var dbUser = _userManager.GetUser(userId);
			// missing check if user is returned

			// - subscriptionService.GetSubscriptionDataWithProducts(subscriptionId)
			// - catalogService.GetAllValueAddedServices
			// - subscriptionProductLogic.GetObtainableVasProducts
			// - userBanService.GetClaimedAccessRights

			//// Act
			var result = await controller.Get(subscriptionId);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}

		// GetObtainables (int subscriptionId)
		[Test]
		[TestCase(true)]
		public async Task GetObtainables(bool isValidSubscriptionId)
		{
			//// Arrange			
			var subscriptionId = GetSubscriptionId(isValidSubscriptionId);

			// userServicesLogic.GetObtainableValueAddedServices (User.UserId (), subscriptionId)
			// var dbUser = _userManager.GetUser(userId);
			// missing check if user is returned

			// missing proper error handling for the following...
			// subscriptionService.GetSubscriptionDataWithProducts(subscriptionId)
			// catalogService.GetValueAddedServices
			// subscriptionProductLogic.GetObtainableVasProducts

			//// Act
			var result = await controller.GetObtainables(subscriptionId);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}

		// Add (int subscriptionId, Request<AddVasData> addVasRequest)
		[Test]
		[TestCase(true, true)]
		public async Task Add(bool isValidSubscriptionId, bool isValidVasRequest)
		{
			//// Arrange			
			var subscriptionId = GetSubscriptionId(isValidSubscriptionId);
			var vasRequest = GetRequest<AddVasData>(isValidVasRequest);

			// userId = User.UserId ()
			// !ModelState.IsValid - HandleModelStateErrors

			// missing product codes
			// addVasRequest.Data.ProductCodes == null || !addVasRequest.Data.ProductCodes.Any ()
			// - SwitchrArgumentException (ErrorCode.InvalidParameter
			// userValidationLogic.ThrowIfUserHaveNotClaimedSubscription
			// userVasLogic.Add
			// userServicesLogic.GetUserEngagement

			//// Act
			var result = await controller.Add(subscriptionId, vasRequest);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}

		// Delete (int subscriptionId, string productKey)
		[Test]
		[TestCase(true, true, true)]
		public async Task Delete(bool isValidSubscriptionId, bool isValidProductKey)
		{
			//// Arrange			
			var subscriptionId = GetSubscriptionId(isValidSubscriptionId);

			// string.IsNullOrWhiteSpace (productKey) - SwitchrArgumentException (ErrorCode.InvalidParameter
			// _userValidationLogic.ThrowIfUserHaveNotClaimedSubscription
			// - userManager.GetUser(userId)
			// - user has no matching subscription (id)
			// - user.UserSubscriptions.All(s => s.SubscriptionId != subscriptionId) - SwitchrNotFoundException(ErrorCode.ErrorSubscriptionRelationNotFound

			// userVasLogic.Remove
			// userServicesLogic.GetServicesSubscription

			var productKey = GetProductKey(isValidProductKey);

			//// Act
			var result = await controller.Delete(subscriptionId, productKey);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}

		// ReplaceAddedService (int subscriptionId, string productKey, Request<PatchDocument<ReplaceVasData>> patchRequest)
		[Test]
		[TestCase(true, true, true)]
		public async Task ReplaceAddedService(bool isValidSubscriptionId, bool isValidProductKey, bool isValidRequest)
		{
			//// Arrange			
			var subscriptionId = GetSubscriptionId(isValidSubscriptionId);

			var productKey = GetProductKey(isValidProductKey);
			var request = GetRequest<PatchDocument<ReplaceVasData>>(isValidRequest);

			// ModelState.IsValid - HandleModelStateErrors
			// patchRequest.Data.Op != PatchOperation.Replace - SwitchrArgumentException (ErrorCode.InvalidParameter
			// userValidationLogic.ThrowIfUserHaveNotClaimedSubscription
			// - userManager.GetUser(userId)
			// - user has no matching subscription (id)
			// - user.UserSubscriptions.All(s => s.SubscriptionId != subscriptionId) - SwitchrNotFoundException(ErrorCode.ErrorSubscriptionRelationNotFound

			// userVasLogic.Replace
			// userServicesLogic.GetUserEngagement

			//// Act
			var result = await controller.ReplaceAddedService(subscriptionId, productKey, request);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}

		// IsFinalized (int subscriptionId, string productKey)
		[Test]
		[TestCase(true, true, true)]
		public async Task IsFinalized(bool isValidSubscriptionId, bool isValidProductKey)
		{
			//// Arrange			
			var subscriptionId = GetSubscriptionId(isValidSubscriptionId);
			var productKey = GetProductKey(isValidProductKey);

			// userValidationLogic.ThrowIfUserHaveNotClaimedSubscription
			// - userManager.GetUser(userId)
			// - user has no matching subscription (id)
			// - user.UserSubscriptions.All(s => s.SubscriptionId != subscriptionId) - SwitchrNotFoundException(ErrorCode.ErrorSubscriptionRelationNotFound

			// userVasStatusLogic.IsFinalized

			//// Act
			var result = await controller.IsFinalized(subscriptionId, productKey);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}

		// IsActivated (int subscriptionId, string productKey)
		[Test]
		[TestCase(true, true, true)]
		public async Task IsActivated(bool isValidSubscriptionId, bool isValidProductKey)
		{
			//// Arrange			
			var subscriptionId = GetSubscriptionId(isValidSubscriptionId);
			var productKey = GetProductKey(isValidProductKey);

			// userValidationLogic.ThrowIfUserHaveNotClaimedSubscription
			// - userManager.GetUser(userId)
			// - user has no matching subscription (id)
			// - user.UserSubscriptions.All(s => s.SubscriptionId != subscriptionId) - SwitchrNotFoundException(ErrorCode.ErrorSubscriptionRelationNotFound

			// userVasStatusLogic.IsActivated

			//// Act
			var result = await controller.IsActivated(subscriptionId, productKey);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}

		// Finalized (int subscriptionId, Request<FinalizedVAS> finalizedRequest)
		[Test]
		[TestCase(true, true, true)]
		public async Task Finalized(bool isValidSubscriptionId, bool isValidRequest)
		{
			//// Arrange			
			var subscriptionId = GetSubscriptionId(isValidSubscriptionId);
			var request = GetRequest<FinalizedVAS>(isValidRequest);

			// finalizedRequest?.Data == null - BadRequest
			// subscriptionVasLogic.Finalized

			//// Act
			var result = await controller.Finalized(subscriptionId, request);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}

		// ProductActivate (int subscriptionId, string productKey, Request<ProductActivateAdditionalParametersRequest> productActivateAdditionalParametersRequest)
		[Test]
		[TestCase(true, true, true)]
		public async Task ProductActivate(bool isValidSubscriptionId, bool isValidProductKey, bool isValidRequest)
		{
			//// Arrange			
			var subscriptionId = GetSubscriptionId(isValidSubscriptionId);
			var productKey = GetProductKey(isValidProductKey);
			var activatedRequest = GetDateTimeRequest(isValidRequest);

			// userValidationLogic.ThrowIfUserHaveNotClaimedSubscription
			// - userManager.GetUser(userId)
			// - user has no matching subscription (id)
			// - user.UserSubscriptions.All(s => s.SubscriptionId != subscriptionId) - SwitchrNotFoundException(ErrorCode.ErrorSubscriptionRelationNotFound

			// subscriptionVasLogic.EvaluateActivationStatus

			//// Act
			var result = await controller.Activated(subscriptionId, productKey, activatedRequest);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}

		// Activated (int subscriptionId, string serviceName, Request<DateTime> activatedRequest)
		[Test]
		[TestCase(true, true, true)]
		public async Task Activated(bool isValidSubscriptionId, bool isValidServiceName, bool isValidRequest)
		{
			//// Arrange			
			var subscriptionId = GetSubscriptionId(isValidSubscriptionId);
			var serviceName = GetServiceName(isValidServiceName);			
			var activatedRequest = GetDateTimeRequest(isValidRequest);

			// subscriptionVasLogic.Activated

			//// Act
			var result = await controller.Activated(subscriptionId, serviceName, activatedRequest);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}

		// Removed (int subscriptionId, string serviceName, Request<DateTime> removedRequest)
		[Test]
		[TestCase (true)]
		public async Task Removed (bool isValidSubscriptionId, bool isValidServiceName, bool isValidRequest)
		{
			//// Arrange			
			var serviceName = GetServiceName(isValidServiceName);
			var subscriptionId = GetSubscriptionId (isValidSubscriptionId);
			var removedRequest = GetDateTimeRequest(isValidRequest);

			// subscriptionVasLogic.Removed

			//// Act
			///  removedRequest
			var result = await controller.Removed (subscriptionId, serviceName, removedRequest);

			//// Assert
			Assert.IsNotNull (result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}

		// Activate (int subscriptionId, string callingApp, string serviceId, string requestType, string requestValue)
		[Test]
		[TestCase(true)]
		public void Activate(bool isValidSubscriptionId, bool isValidServiceId, bool isValidCallingApp, bool isValidRequestType, bool isValidRequestValue)
		{
			//// Arrange			
			var subscriptionId = GetSubscriptionId(isValidSubscriptionId);
			var serviceId = GetServiceId(isValidServiceId);
			var callingApp = GetCallingApp(isValidCallingApp);
			var requestType = GetRequestType(isValidRequestType);
			var requestValue = GetRequestValue(isValidRequestValue);

			// userVasLogic.ActivateService

			//// Act
			///  removedRequest
			var result = controller.Activate(subscriptionId, callingApp, serviceId, requestType, requestValue);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}
	}
}