using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using Switchr.API.Areas.SocialSecurityNumber;
using Switchr.API.Tests.Common;
using Switchr.Logic.SSN;
using Switchr.Logic.TelephoneNumbers;
using Switchr.Models;
using Switchr.Models.SSN;

namespace Switchr.API.Tests.Controllers.SocialSecurityNumber
{
	class SocialSecurityNumberControllerTest : BaseControllerTest
	{
		protected IDictionary<Params, object> dict;

		private IUserTelephoneNumberLogic _telephoneNumber;
		private ISSNLogic _ssn;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private SocialSecurityNumberController controller => GetController<SocialSecurityNumberController>();

		public T CreateTestControllerWith<T>(IUserTelephoneNumberLogic tn = null, ISSNLogic ssn = null) where T : ApiController, new()
		{
			return new SocialSecurityNumberController(_telephoneNumber, _ssn) as T;
		}

		[SetUp]
		public void SetUp()
		{			
			_telephoneNumber = GetInst<IUserTelephoneNumberLogic>();
			_ssn = GetInst<ISSNLogic>();

			base.Setup();
		}

		override protected object GetLookupValue(Enum key)
		{
			return dict[(Params)key];
		}

		protected enum Params : int { }		

		// Get(string ssn, string token)
		[Test]
		[TestCase(true, true)]
		public async Task Get(bool isValidSSN, bool isValidToken)
		{
			////// Arrange
			string ssn = GetSsn(isValidSSN);
			string token = GetToken(isValidToken);

			// UserTelephoneNumberLogic
			// SubscriptionService, CdsManager
			// userTelephoneNumberLogic.GetBanEngagementsBySSN(ssn, token)
			// - cdsManager.GetBansForSsn(ssn)
			// -- cdsService.GetBansForSsn(ssn)

			//// Act
			var result = await controller.Get(ssn, token);

			//// Assert
			Assert.IsNotNull(result);
		}


		// ValidateSSNB2C(Request<NemIdSSNDetail> ssnDetailsRequest)
		[Test]
		[TestCase(true)]
		public void ValidateSSNB2C(bool isValidNemIdSSN)
		{
			////// Arrange
			var nemIdSsnRequest = NemIdSSNRequest(isValidNemIdSSN);

			// ssnLogic.ValidateSSN(ssnDetailsRequest.Data.SSN, Segment.Private, Role.Subscriber, brand);
			// userManager.GetUserBySSN(ssn, segment, role, brand)
			// -- dbContext.Users. (DB)

			// ssnLogic.Add(ssnDetailsRequest.Data.AccessToken, ssnDetailsRequest.Data.SSN);
			// - nemIdSSNManager.AddNemIdToken(CreateNemIdSSNDetail(token, ssn))
			// -- dbContext.NemIdTokens.AddOrUpdate(nemIdToken) (DB)

			//// Act
			var result = controller.ValidateSSNB2C(nemIdSsnRequest);

			//// Assert
			Assert.IsNotNull(result);
		}

		// Exists(string ssn)
		[Test]
		[TestCase(true)]
		public void Exists(bool isValidSSN)
		{
			////// Arrange
			string ssn = GetSsn(isValidSSN);

			// ssnLogic.SSNExists(ssn, User.UserId())
			// - userManager.GetUser(userId)
			// - userManager.GetUserBySSN(ssn, ...)

			//// Act
			var result = controller.Exists(ssn);

			//// Assert
			Assert.IsNotNull(result);
		}
	}
}
