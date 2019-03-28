using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using Switchr.API.Areas.SMS.Controllers;
using Switchr.API.Tests.Common;
using Switchr.Logic.SMS;
using Switchr.Models;
using Switchr.Models.SMS;

namespace Switchr.API.Tests.Controllers.SMS
{
	class SMSControllerTest: BaseControllerTest
	{
		protected IDictionary<Params, object> dict;

		private ISMSLogic _sms;

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private SMSController controller => GetController<SMSController>();

		public T CreateTestControllerWith<T>(ISMSLogic sms = null) where T : ApiController, new()
		{
			sms = useOrDefault<ISMSLogic>(sms, _sms);
			return new SMSController(sms) as T;
		}

		[SetUp]
		public void SetUp()
		{
			_sms = GetInst<ISMSLogic>();
			base.Setup();
		}

		override protected object GetLookupValue(Enum key)
		{
			return dict[(Params)key];
		}

		protected enum Params : int { }

		//  Add(Request<SMSModel> smsRequest)
		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public async Task GetUserConsentsBySubscription(bool isValidRequest)
		{
			////// Arrange
			var smsRequest = GetRequest<SMSModel>(isValidRequest);

			// smsLogic.SendSMSAsync(smsRequest.Data.MobileNumber, smsRequest.Data.SMSText)
			// SMSCodeManager
			// SwitchrEntities (DB)

			//// Act
			var result = await controller.Add(smsRequest);

			//// Assert
			Assert.IsNotNull(result);
		}

		// Validate([FromUri]MobileNumberCodesModel smsModel)
		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void Validate(bool isValidModel)
		{
			////// Arrange
			var smsModel = GetSmsModel(isValidModel);

			//// Act
			var result = controller.Validate(smsModel);

			//// Assert
			Assert.IsNotNull(result);
		}
	}
}
