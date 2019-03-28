using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using NUnit.Framework;
using Switchr.API.Areas.User.Controllers;
using Switchr.API.Areas.VAS.Controllers;
using Switchr.API.Tests.Common;
using Switchr.Logic.Configuration;
using Switchr.Logic.Exceptions;
using Switchr.Logic.Http;
using Switchr.Logic.Services.Spotify;
using Switchr.Logic.Subscriptions;
using Switchr.Models;
using Switchr.Models.VAS;

namespace Switchr.API.Tests.Controllers.VAS {

	class SpotifyControllerTest : BaseControllerTest {
		protected IDictionary<Params, object> dict;

		private SubscriptionVasLogic _subscriptionVasLogic;
		private IHttpWrapper _httpWrapper;
		
		private SpotifyService _spotifyService;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private SpotifyController controller => GetController<SpotifyController>();

		protected T CreateTestControllerWith<T>(
			SubscriptionVasLogic subscriptionVasLogic = null,
			IHttpWrapper httpWrapper = null,
			SpotifyService spotifyService = null) where T : ApiController, new()
		{
			subscriptionVasLogic = useOrDefault<SubscriptionVasLogic>(subscriptionVasLogic, _subscriptionVasLogic);
			httpWrapper = useOrDefault<IHttpWrapper>(httpWrapper, _httpWrapper);
			spotifyService = useOrDefault< SpotifyService>(spotifyService, _spotifyService);

			return new SpotifyController(
				subscriptionVasLogic,
				httpWrapper,
				spotifyService
			) as T;
		}

		[SetUp]
		public void SetUp () {		
			_subscriptionVasLogic = GetInst<SubscriptionVasLogic> ();
			_httpWrapper = GetInst<IHttpWrapper> ();
			_spotifyService = GetInst<SpotifyService> ();

			base.Setup();
		}

		override protected object GetLookupValue(Enum key)
		{
			return dict[(Params)key];
		}

		protected enum Params : int { }

		// GetSpotifyState(string state)
		[Test]
		[TestCase (true)]
		public void GetSpotifyState (bool isValidState) {
			//// Arrange			
			var state = GetState(isValidState);

			// on any exception
			// InternalServerError

			//// Act
			var result = controller.GetSpotifyState (state);

			//// Assert
			Assert.IsNotNull (result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}

		// AccessToken(string authorizationCode)
		[Test]
		[TestCase(true)]
		public async Task AccessToken(bool isValidAuthCode)
		{
			//// Arrange			
			var authCode = GetAuthCode(isValidAuthCode); ;

			// on any exception
			// InternalServerError


			//// Act
			var result = await controller.AccessToken(authCode);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}

		// CanonicalUsername(string accessToken)
		[Test]
		[TestCase(true)]
		public async Task CanonicalUsername(bool isValidAccessToken)
		{
			//// Arrange			
			var accessToken = GetAccessToken(isValidAccessToken); ;

			// on any exception
			// InternalServerError

			//// Act
			var result = await controller.CanonicalUsername(accessToken);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}

		// State(string state)
		[Test]
		[TestCase(true)]
		public async Task State(bool isValidState)
		{
			//// Arrange			
			var state = GetState(isValidState);

			// on any exception
			// SwitchrArgumentException (ErrorCode.InvalidParameter

			//// Act
			var result = await controller.State(state);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}

		// Resources(string code, string callbackUrl = null)
		[Test]
		[TestCase(true)]
		public async Task Resources(bool isValidCode, bool isValidCallbackUrl)
		{
			//// Arrange			
			var code = GetCode(isValidCode);
			var callbackUrl = GetCallbackUrl(isValidCallbackUrl);

			// code is empty
			// string.IsNullOrWhiteSpace (code) - SwitchrArgumentException (ErrorCode.InvalidParameter

			// spotifyService.GetAccessToken
			// no access token (null) - SwitchrArgumentException(ErrorCode.ExternalCallFailed

			// on any other exception
			// SwitchrArgumentException (ErrorCode.InvalidParameter

			//// Act
			var result = await controller.Resources(code, callbackUrl);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual ("Bruger", result.ViewBag.Title);
		}
	}
}