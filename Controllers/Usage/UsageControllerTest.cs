
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using FakeItEasy;
using Switchr.API.Areas.Usage.Controllers;
using Switchr.API.Infrastructure.Hateoas.Interfaces;
using Switchr.Data.Interfaces;
using Switchr.Logic.Usage;
using Switchr.Logic.Validation;
using System.Web.Mvc;
using Switchr.API.Tests.Common;
using System.Collections.Generic;
using System;

namespace Switchr.API.Tests.Controllers
{
	[TestFixture]
	public class UsageControllerTest : BaseControllerTest
	{
		protected IDictionary<Params, object> dict;

		private IUsageLogic _usageLogic;
		private IUserManager _userManager;
		private AccessRightLogic _accessRightLogic;
		private IHateoasHandlerFactory _hateoasHandlerFactory;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private UsageController controller => GetController<UsageController>();

		protected T CreateTestControllerWith<T>(
			IUserManager userManager = null,
			IUsageLogic usageLogic = null,
			IHateoasHandlerFactory hateoasHandlerFactory = null,
			AccessRightLogic accessRightLogic = null
		) where T : ApiController, new()
		{
			userManager = useOrDefault<IUserManager>(userManager, _userManager);
			usageLogic = useOrDefault<IUsageLogic>(usageLogic, _usageLogic);
			hateoasHandlerFactory = useOrDefault<IHateoasHandlerFactory>(hateoasHandlerFactory, _hateoasHandlerFactory);
			accessRightLogic = useOrDefault<AccessRightLogic>(accessRightLogic, _accessRightLogic);

			return new UsageController(userManager, usageLogic, hateoasHandlerFactory, accessRightLogic) as T;
		}

		[SetUp]
		public void SetUp()
		{
			_usageLogic = GetInst<IUsageLogic>();
			_accessRightLogic = GetInst<AccessRightLogic>();
			_hateoasHandlerFactory = GetInst<IHateoasHandlerFactory>();
			_userManager = GetInst<IUserManager>();

			base.Setup();
		}

		override protected object GetLookupValue(Enum key)
		{
			return dict[(Params)key];
		}

		protected enum Params : int { }

		// GetAggregatedUsage(string msisdn, int year, int month, string subscriptionType)
		[Test]
		[TestCase(true)]
		public async Task GetAggregatedUsage(bool isValidMsisdn, bool isValidYear, bool isValidMonth, bool isValidSubscriptionType)
		{
			//// Arrange
			var msisdn = GetMsisdn(isValidMsisdn);
			var year = GetYear(isValidYear);
			var month = GetMonth(isValidMonth);
			var subscriptionType = GetSubscriptionType(isValidSubscriptionType);

			//// Act

			// cbmService
			// accessRightLogic.CheckAccessRight(msisdn, userId)
			// no access: SwitchrForbiddenException(ErrorCode.ErrorWhenAccesstoSubscription
			// usageLogic.GetAggregatedUsage(user, msisdn, year, month, subscriptionType)
			// cbmService.GetDataQuota
			// usageResourceModel (DB)

			var result = await controller.GetAggregatedUsage(msisdn, year, month, subscriptionType); // as ViewResult;

			//// Assert
			// var expected = "Forbrug";
			Assert.IsNotNull(result);
			// Assert.AreEqual(expected, result.ViewBag.Title);
		}
		// accessRightLogic.CheckAccessRight
		// SwitchrForbiddenException(ErrorCode.ErrorWhenAccesstoSubscription

		// GetDetailedUsage(string msisdn, int year, int month, int offset, int pagesize, bool isadditional, string category)
		[Test]
		[TestCase(true)]
		public async Task GetDetailedUsage(bool isValidMsisdn, bool isValidYear, bool isValidMonth, bool isValidOffset, bool isValidPageSize, bool isAdditional, bool isValidCategory)
		{
			//// Arrange
			var msisdn = GetMsisdn(isValidMsisdn);
			var year = GetYear(isValidYear);
			var month = GetMonth(isValidMonth);
			var pageSize = GetPagSize(isValidPageSize);
			var offset = GetOffset(isValidOffset);
			var category = GetCategory(isValidCategory);

			//// Act

			// accessRightLogic.CheckAccessRight(msisdn, userId)
			// no access: SwitchrForbiddenException(ErrorCode.ErrorWhenAccesstoSubscription
			// usageLogic.GetDetailedUsage(...)
			// subscriptionService.GetSubscriptionDataForMsisdn(msisdn)
			// - SwitchrException(ErrorCode.UsageError
			// usageProvider.GetAccountDetailedUsageForMont
			// - SwitchrException(ErrorCode.UsageError
			// UsageDetailsResourceModel (DB)

			var result = await controller.GetDetailedUsage(msisdn, year, month, offset, pageSize, isAdditional, category); // as ViewResult;

			//// Assert
			// var expected = "Forbrug";
			Assert.IsNotNull(result);
			// Assert.AreEqual(expected, result.ViewBag.Title);
		}

		// GetUsageControl(string msisdn)
		[Test]
		[TestCase(true)]
		public async Task GetUsageControl(bool isValidMsisdn)
		{
			//// Arrange
			var msisdn = GetMsisdn(isValidMsisdn);

			//// Act

			// accessRightLogic.CheckAccessRight(msisdn, userId)
			// no access: SwitchrForbiddenException(ErrorCode.ErrorWhenAccesstoSubscription
			// usageLogic.GetUsageControl(userId, msisdn)
			// ninjaGenericInterfaceClient

			var result = await controller.GetUsageControl(msisdn); // as ViewResult;

			//// Assert
			// var expected = "Forbrug";
			Assert.IsNotNull(result);
			// Assert.AreEqual(expected, result.ViewBag.Title);
		}

		// SetUsageControl(string msisdn, Request<SetUsageControlRequest> setUsageControl)
		[Test]
		[TestCase(true)]
		public async Task SetUsageControl(bool isValidMsisdn)
		{
			//// Arrange
			var msisdn = GetMsisdn(isValidMsisdn);
			var usageControlRequest = GetRequest<Models.SetUsageControlRequest>();

			//// Act

			// accessRightLogic.CheckAccessRight(msisdn, userId)
			// no access: SwitchrForbiddenException(ErrorCode.ErrorWhenAccesstoSubscription
			// usageLogic.GetUsageControl(userId, msisdn)
			// ninjaGenericInterfaceClient

			var result = await controller.SetUsageControl(msisdn, usageControlRequest); // as ViewResult;

			//// Assert
			// var expected = "Forbrug";
			Assert.IsNotNull(result);
			// Assert.AreEqual(expected, result.ViewBag.Title);
		}

		// GetHeaderUsageDetailInfo(string msisdn, int year, int month, string category)
		[Test]
		[TestCase(true)]
		public async Task GetHeaderUsageDetailInfo(bool isValidMsisdn, bool isValidYear, bool isValidMonth, bool isAdditional, bool isValidCategory)
		{
			//// Arrange
			var msisdn = GetMsisdn(isValidMsisdn);
			var year = GetYear(isValidYear);
			var month = GetMonth(isValidMonth);
			var category = GetCategory(isValidCategory);

			//// Act

			// accessRightLogic.CheckAccessRight(msisdn, userId)
			// no access: SwitchrForbiddenException(ErrorCode.ErrorWhenAccesstoSubscription
			// usageLogic.GetUsageControl(userId, msisdn)
			// ninjaGenericInterfaceClient

			var result = await controller.GetHeaderUsageDetailInfo(msisdn, year, month, category); // as ViewResult;

			//// Assert
			// var expected = "Forbrug";
			Assert.IsNotNull(result);
			// Assert.AreEqual(expected, result.ViewBag.Title);
		}

		// SetBarring(string msisdn, Request<List<SetBarringsRequest>> barringRequests)
		[Test]
		[TestCase(true)]
		public async Task SetBarring(bool isValidMsisdn, bool isValidBarringRequest)
		{
			//// Arrange
			var msisdn = GetMsisdn(isValidMsisdn);
			var barringRequest = GetRequest<System.Collections.Generic.List<Models.ResourceModel.SetBarringsRequest>>(isValidBarringRequest);

			//// Act

			// accessRightLogic.CheckAccessRight(msisdn, userId)
			// no access: SwitchrForbiddenException(ErrorCode.ErrorWhenAccesstoSubscription
			// usageLogic.GetUsageControl(userId, msisdn)
			// ninjaGenericInterfaceClient

			var result = await controller.SetBarring(msisdn, barringRequest); // as ViewResult;

			//// Assert
			// var expected = "Forbrug";
			Assert.IsNotNull(result);
			// Assert.AreEqual(expected, result.ViewBag.Title);
		}

		// GetBarrings(string msisdn, string subscriptionType)
		[Test]
		[TestCase(true)]
		public async Task GetBarrings(bool isValidMsisdn, bool isValidSubscriptionType)
		{
			//// Arrange
			var msisdn = GetMsisdn(isValidMsisdn);
			var subscriptionType = GetSubscriptionType(isValidSubscriptionType);

			//// Act

			// accessRightLogic.CheckAccessRight(msisdn, userId)
			// no access: SwitchrForbiddenException(ErrorCode.ErrorWhenAccesstoSubscription
			// usageLogic.GetUsageControl(userId, msisdn)
			// ninjaGenericInterfaceClient

			var result = await controller.GetBarrings(msisdn, subscriptionType); // as ViewResult;

			//// Assert
			// var expected = "Forbrug";
			Assert.IsNotNull(result);
			// Assert.AreEqual(expected, result.ViewBag.Title);
		}
	}
}