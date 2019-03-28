using System;
using System.Collections.Generic;
using System.Web.Http;
using NUnit.Framework;
using Switchr.API.Areas.Catalog.Controllers;
using Switchr.API.Tests.Common;
using Switchr.Data.Interfaces;
using Switchr.Logic.Services.Switchr;

namespace Switchr.API.Tests.Controllers.Catalog
{
	class CatalogControllerTest: BaseControllerTest
	{
		protected IDictionary<Params, object> dict;

		private IUserManager _userManager;
		private CatalogService _catalogService;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		public T CreateTestControllerWith<T>(CatalogService catalogService = null, IUserManager userManager = null) where T : ApiController, new()
		{
			catalogService = Eval<CatalogService>(Params.CatalogService, catalogService);
			userManager = Eval<IUserManager>(Params.UserManager, userManager);
			return new CatalogController(catalogService, userManager) as T;
		}

		private CatalogController controller => GetController<CatalogController>();


		[SetUp]
		public void SetUp()
		{			
			_catalogService = GetInst<CatalogService>();
			_userManager = GetInst<IUserManager>();

			dict = new Dictionary<Params, object>()
			{
				{Params.CatalogService, _catalogService},
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
			CatalogService,
			UserManager
		}

		// Get()
		[Test]
		public void Get()
		{
			////// Arrange

			// No error handling, just builds a model using args
			// _userManager.GetUser(User.UserId())
			// _catalogService.GetValueAddedServiceGroups()
			// _catalogService.GetValueAddedServices((Segment)user.Segment.Id, (Role)user.Role.Id,brand)
			// _catalogService.GetBenefits()

			////// Act
			var result = controller.Get();

			////// Assert
			Assert.IsNotNull(result);
		}
	}
}
