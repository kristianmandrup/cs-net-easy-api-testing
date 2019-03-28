using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using Switchr.API.Areas.User.Controllers;
using Switchr.Logic.Menu;
using Switchr.Logic.Users;
using Switchr.Logic.Users.OTAC;
using Switchr.Models;
using TeliaDK.Hades.ClassLibrary.Models.NinjaMDWCInterface;

namespace Switchr.API.Tests.Controllers.User
{
	class OtacControllerTest: UserTest
	{
		private IOtacLogic _otac;
		private OtacSignatureLogic _otacSignature;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private OtacController controller => GetController<OtacController>();

		protected T CreateTestControllerWith<T>(IOtacLogic otac = null, OtacSignatureLogic otacSignature = null) where T : ApiController, new()
		{
			otac = useOrDefault<IOtacLogic>(otac, _otac);
			otacSignature = useOrDefault<OtacSignatureLogic>(otacSignature, _otacSignature);
			return new OtacController(otac, otacSignature) as T;

		}

		private Request<OtacRequest> GetRequestData(bool isValidData)
		{
			return null;
		}

		[SetUp]
		public void SetUp()
		{
			base.Setup();
		}

		// GenererateOtac(string signature, string nonce, int timestamp, Request<OtacRequest> requestData)
		[Test]
		[TestCase(true, true, true, true)]
		public void GenererateOtac(bool isValidSignature, bool isValidNonce, bool isValidTimeStamp, bool isValidData)
		{
			//// Arrange
			var signature = GetSignature(isValidSignature);
			var nonce = GetNonce(isValidNonce);
			var timestamp = GetTimeStamp(isValidTimeStamp);
			var requestData = GetRequestData(isValidData);

			// otacSignatureLogic.VerifySignature
			// - Unauthorized() - UnauthorizedResult
			// otacLogic.CreateOtacs
			// - userManager.GetUsers() (DB)
			// - otacManager.Add (DB)

			//// Act
			var result = controller.GenererateOtac(signature, nonce, timestamp, requestData);

			//// Assert
			Assert.IsNotNull(result);
			// Assert.AreEqual("Glemt", result.ViewBag.Title);
		}
	}
}
