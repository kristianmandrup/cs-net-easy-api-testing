using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using Switchr.API.Areas.Handover.Controllers;
using Switchr.API.Tests.Common;
using Switchr.Logic.Exceptions;
using Switchr.Logic.Handover;
using Switchr.Models;
using Switchr.Models.Handover;

namespace Switchr.API.Tests.Controllers.Handover
{
	class HandoverControllerTest: BaseControllerTest
	{
		private IHandoverLogic _handover;
		protected IDictionary<Params, object> dict;
		
		public T CreateTestControllerWith<T>(IHandoverLogic handover = null) where T : ApiController, new()
		{
			handover = Eval<IHandoverLogic>(Params.Handover, handover);
			return new HandoverController(handover) as T;
		}

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		private HandoverController controller => GetController<HandoverController>();


		[SetUp]
		public void SetUp()
		{	
			_handover = GetInst<IHandoverLogic>();

			dict = new Dictionary<Params, object>()
			{
				{Params.Handover, _handover}
			};

			base.Setup();
		}

		override protected object GetLookupValue(Enum key)
		{
			return dict[(Params)key];
		}

		public enum Params : int
		{
			Handover
		}

		// StartHandover(Request<StartHandoverData> startHandoverRequest)
		[Test]
		[TestCase(true)]
		public async Task StartHandover(bool isValidHandoverData)
		{
			////// Arrange
			var handoverData = GetStartHandoverData(isValidHandoverData);

			// !ModelState.IsValid
			// HandleModelStateErrors()

			// _handoverLogic.StartHandover(User.UserId(), startHandoverRequest.Data)

			// subscriptionService
			// await _subscriptionService.GetSubscriptionDataWithProducts(data.SubscriptionId);

			// GetSubmitterStatus(userId, subscriptionDataWithProducts))?.IsPendingBpm == true
			// - SwitchrNotFoundException(ErrorCode.SubscriptionDetailsNotFound

			// _passwordLogic.VerifyPassword(userId, data.Password) == false
			// - SwitchrForbiddenException(ErrorCode.SubscriptionIsPendingHandover

			// _userBanManager.IsLegalOwner(userId, banEntity.Id) == false
			// - SwitchrForbiddenException(ErrorCode.InvalidPassword

			// subscriptionDataWithProducts.CommitmentEndDate.HasValue && DateTime.Today <= subscriptionDataWithProducts.CommitmentEndDate
			// - SwitchrForbiddenException(ErrorCode.UserIsNotLegalOwner

			// _subscriptionService.IsEmployeeSubscription(subscriptionDataWithProducts)
			// - SwitchrForbiddenException(ErrorCode.HandoverEmployeeSubscription

			// _bpmClient.HandoverStartAsync(OriginatingSystem, Channel, Dealer, submitterInfo, receiverInfo, subscriptionInfo)
			// !response.IsSuccessFull()
			// - SwitchrBadGatewayException(ErrorCode.ExternalCallFailed


			//// Act
			var result = await controller.StartHandover(handoverData);

			//// Assert
			Assert.IsNotNull(result);
		}
		
		// CancelHandover(string id)
		[Test]
		[TestCase(true)]
		public async Task CancelHandover(bool isValidHandoverId)
		{
			////// Arrange
			var handoverId = GetHandoverId(isValidHandoverId);

			// no validation of ID
			// _handoverLogic.CancelHandover(User.UserId(), id)

			// _bpmClient
			// SubmitterWaitingForReceiverByOrderReference(userId, orderReference)
			//  _bpmClient.HandoverSubmitterCancelAsync(orderReference, OriginatingSystem, Channel, Dealer)

			// IsWaitingForReceiverByOrderReference(userId, orderReference)
			//  _bpmClient.HandoverReceiverCancelAsync(orderReference, OriginatingSystem, Channel, Dealer)

			// else - SwitchrArgumentException(ErrorCode.HandoverError
			// !response.IsSuccessFull()
			// - SwitchrException(ErrorCode.ExternalCallFailed

			//// Act
			var result = await controller.CancelHandover(handoverId);

			//// Assert
			Assert.IsNotNull(result);
		}

		// AcceptHandoverAsync(string id, Request<PatchDocument<AcceptHandoverData>> request)
		[Test]
		[TestCase(true)]
		public async Task AcceptHandoverAsync(bool isValidHandoverId)
		{
			//// Arrange
			var handoverData = GetHandoverData();
			var handoverId = GetHandoverId(isValidHandoverId);

			// request.Data.Op != PatchOperation.Update - SwitchrArgumentException(ErrorCode.InvalidParameter
			// !ModelState.IsValid - HandleModelStateErrors()

			// _handoverLogic.AcceptHandover(User.UserId(), id, request.Data.Value)

			// handoverLogic
			// IsWaitingForReceiverByOrderReference(userId, orderReference)
			// SwitchrNotFoundException(ErrorCode.HandoverError

			// ????
			// data.PaymentInfo.PaymentMethod == Models.Handover.PaymentMethod.PBS && (string.IsNullOrWhiteSpace(data.PaymentInfo.BankCode) || string.IsNullOrWhiteSpace(data.PaymentInfo.AccountNumber))
			// - SwitchrArgumentException(ErrorCode.InvalidParameter

			// userManager, creditCheckLogic, bpmClient

			// _userManager.GetUser(userId)
			// _creditCheckLogic.CheckCredit(socialSecurityNumber, personInfo.Person.FirstName, personInfo.Person.LastName, data.Customer.Accomodation, data.Customer.Employment)
			// creditCheckResponse.Approved == false
			// SwitchrForbiddenException(ErrorCode.CreditCheckLookupFailed

			// _subscriptionService.IsEmployeeSubscription(subscription)
			// SwitchrForbiddenException(ErrorCode.HandoverEmployeeSubscription

			// HasSubscriptionChanged(subscription, data.Subscription)
			// SwitchrArgumentException(ErrorCode.SubscriptionHasChanged

			// _bpmClient.HandoverReceiverAcceptAsync(orderReference, OriginatingSystem, accountInfo, subscriptionInfo, Channel, Dealer)
			// response.IsSuccessFull() == false
			// SwitchrBadGatewayException(ErrorCode.HandoverError

			// !_subscriptionManager.RemoveSubscriptionBySubscriptionId(subscription.Id)
			// logs warning

			//// Act
			var result = await controller.AcceptHandoverAsync(handoverId, handoverData);

			//// Assert
			Assert.IsNotNull(result);
		}
	}
}
