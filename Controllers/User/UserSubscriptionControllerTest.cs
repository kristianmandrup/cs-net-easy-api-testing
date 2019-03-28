using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using AutoMapper;
using NUnit.Framework;
using Switchr.API.Areas.User.Controllers;
using Switchr.API.Infrastructure.Hateoas.Interfaces;
using Switchr.API.Tests.Controllers.User;
using Switchr.Data.Interfaces;
using Switchr.Logic.Configuration;
using Switchr.Logic.Handover;
using Switchr.Logic.Services.Cds.interfaces;
using Switchr.Logic.Users;
using TeliaDK.CDS.ClassLibrary.Clients;
using TeliaDK.Core.Cache;
using TeliaDK.Hades.ClassLibrary.Clients;

namespace Switchr.API.Tests.Controllers {
	[TestFixture]
	public class UserSubscriptionControllerTest : UserTest {
		private ICdsService _cdsService;
		private IUserManager _userManager;
		private ICache _cache;
		private ICDSClient _cdsClient;
		private IApplicationSettings _appSettings;
		private IBarringManager _barringManager;
		private IHandoverLogic _handoverLogic;
		private INinjaGenericInterfaceClient _ninjaClient;
		private IMapper _mapper;
		private IHateoasHandlerFactory _hateoasHandlerFactory;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private UserSubscriptionController controller => GetController<UserSubscriptionController>();

		protected T CreateTestControllerWith<T>(
			UserSubscriptionService userSubscriptionService = null
		) where T : ApiController, new()
		{
			userSubscriptionService = useOrDefault<UserSubscriptionService>(userSubscriptionService);
			return new UserSubscriptionController(userSubscriptionService, _hateoasHandlerFactory) as T;
		}

		[SetUp]
		public void SetUp () {
			_cdsService = GetInst<ICdsService> ();
			_userManager = GetInst<IUserManager> ();
			_cache = GetInst<ICache> ();
			_cdsClient = GetInst<ICDSClient> ();
			_appSettings = GetInst<IApplicationSettings> ();
			_barringManager = GetInst<IBarringManager> ();
			_handoverLogic = GetInst<IHandoverLogic> ();
			_ninjaClient = GetInst<INinjaGenericInterfaceClient> ();
			_mapper = GetInst<IMapper> ();

			_hateoasHandlerFactory = GetInst<IHateoasHandlerFactory> ();

			base.Setup();
		}

		protected UserSubscriptionService CreateUserSubscriptionService () {
			return new UserSubscriptionService (
				_cdsService,
				_userManager,
				_mapper,
				_ninjaClient,
				_appSettings,
				_barringManager,
				_cdsClient,
				_handoverLogic
			);
		}

		// Get(int userId)
		[Test]
		[TestCase (true)]
		public async Task Get (bool isValidUserId) {
			//// Arrange			
			var userId = GetUserId (isValidUserId);

			// userSubscriptionService.GetSubscriptions(userId)
			// userManager.GetUser(userId): user == null - SwitchrException(ErrorCode.UserNotFound
			// foreach (var userBan in user.UserBans)
			// !int.TryParse(userBan.BAN?.BANNumber - SwitchrException(ErrorCode.InvalidBan

			//// Act
			var result = await controller.Get (userId);

			//// Assert
			Assert.IsNotNull (result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}

		// Get(int userId, int subscriptionId)
		[Test]
		[TestCase (true)]
		public async Task GetByUserAndSubscription (bool isValidUserId, bool isValidSubscriptionId) {
			//// Arrange			
			var userId = GetUserId (isValidUserId);
			var subscriptionId = GetSubscriptionId (isValidSubscriptionId);

			// userSubscriptionService.GetSubscription(userId, subscriptionId)
			// userManager.GetUser(userId): user == null - SwitchrException(ErrorCode.UserNotFound

			//// Act
			var result = await controller.Get (userId, subscriptionId);

			//// Assert
			Assert.IsNotNull (result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}
	}
}