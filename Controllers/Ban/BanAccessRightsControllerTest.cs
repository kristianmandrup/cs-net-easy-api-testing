using System;
using FakeItEasy;
using NUnit.Framework;
using StructureMap;
using Switchr.API.Areas.Ban.Controllers;
using Switchr.Logic.Users;
using Switchr.Models;

namespace Switchr.API.Tests.Controllers.Ban
{
	class BanAccessRightsControllerTest: BanTest
	{

		private BanAccessRightsController _controller;

		public BanAccessRightsController CreateTestController(
			BanAccessRightsLogic banAccessRights  = null, 
			IUserProfileLogic userProfile = null)
		{
			banAccessRights = Eval<BanAccessRightsLogic>(Params.BanAccessRights, banAccessRights);
			userProfile = Eval<IUserProfileLogic>(Params.UserProfile, userProfile);
			return new BanAccessRightsController(banAccessRights, userProfile);
		}


		protected BanAccessRightsController CreateController()
		{
			return CreateTestController();
		}

		[SetUp]
		public void SetUp()
		{
			base.Setup();
			_controller = CreateController();
		}

		//SELECT TOP(10) [Id]
		//,[User_Id]
		//,[Ban_Id]
		//,[Requested]
		//,[Approved]
		//,[Rejected]
		//FROM[Switchr].[dbo].[Users_Bans_AccessRightsRequests]

		// For User 617
		//SELECT TOP(10) [Id]
		//,[Requested]
		//,[Approved]
		//,[Rejected]
		//FROM[Switchr].[dbo].[Users_Bans_AccessRightsRequests]
		//where User_Id = 617

		// Valid: 24

		[Test]
		// POST
		public void AccessRightsRequest()
		{
			//// Arrange
			var permissionRequest = GetRequest<UserBanAccessRightsRequest>();

			// SwitchrArgumentException(ErrorCode.InvalidParameter
			// permissionRequest?.Data == null

			// SwitchrForbiddenException(ErrorCode.InvalidUserId
			// User.UserId() != permissionRequest.Data.UserId

			// _banAccessRightsLogic
			// CheckUserSubscriptionsBelongsToBan
			// :UserHasNotClaimedSubscriptionOnBan
			// subscriptions.All(sub => sub.Subscription.BAN.BANNumber != ban.ToString()) - SwitchrArgumentException(ErrorCode.UserHasNotClaimedSubscriptionOnBan

			// what if BAD BAN!?
			// :InvalidBan
			// SwitchrNotFoundException(ErrorCode.InvalidBan

			// _banAccessRightsLogic.RequestAccessRights(permissionRequest.Data.UserId, permissionRequest.Data.Ban)

			// _banAccessRightsLogic
			// CheckCurrentLegalOwnerPermission(userId, ban, banData)
			// :UserIsAlreadyLegalOwner
			// banData.UserBans.Any(x => x.UserId == userId) - SwitchrArgumentException(code: ErrorCode.UserIsAlreadyLegalOwner

			// CheckPendingPermissionRequest(userId, ban, banData)
			// :UserHasPendingAccessRightsRequest
			// _userBanPermissionRequestManager.Get(userId, banData.Id).Any(p => p.Approved == null && p.Rejected == null) - SwitchrArgumentException(ErrorCode.UserHasPendingAccessRightsRequest

			//// Act
			var result = _controller.AccessRightsRequest(permissionRequest);

			//// Assert
			Assert.IsNotNull(result);
		}

		[Test]
		// POST
		public void AcceptRequest(bool isValidAccessRightsRequestId)
		{
			//// Arrange
			var accessRightsRequestId = GetAccessRightsRequestId(isValidAccessRightsRequestId);

			// mock
			// _banAccessRightsLogic.AcceptAccessRightsRequest should return mocked UserBanAccessRightsRequestResult

			// SwitchrNotFoundException(ErrorCode.AccessRightsRequestNotFound
			// userAccessRightsRequestResult == null

			// request = _userBanPermissionRequestManager.Get(permissionRequestId)
			// request.Rejected.HasValue - SwitchrArgumentException(ErrorCode.AccessRightsRequestRejected
			// request.Approved.HasValue - SwitchrArgumentException(ErrorCode.AccessRightsRequestApproved
			// !_userBanManager.IsLegalOwner(userId, request.Ban_Id) - SwitchrForbiddenException(ErrorCode.UserIsNotLegalOwner


			//// Act
			var result = _controller.AcceptRequest(accessRightsRequestId);

			// Bad ID
			// SwitchrNotFoundException(ErrorCode.AccessRightsRequestNotFound

			//// Assert
			Assert.IsNotNull(result);
		}



		[Test]
		// POST
		[TestCase(true)]
		public void RejectRequest(bool isValidAccessRightsRequestId)
		{
			//// Arrange
			var accessRightsRequestId = GetAccessRightsRequestId(isValidAccessRightsRequestId);

			// mock
			// _banAccessRightsLogic.AcceptAccessRightsRequest should return mocked UserBanAccessRightsRequestResult

			// SwitchrNotFoundException(ErrorCode.AccessRightsRequestNotFound
			// userAccessRightsRequestResult == null

			// request = _userBanPermissionRequestManager.Get(permissionRequestId)
			// null - null

			// !request.BAN.UserBans.Any(ub => ub.UserId == userId && ub.BanAccessRightsId == (int)BanAccessRight.LegalOwner) - null
			// request.Rejected.HasValue - SwitchrArgumentException(ErrorCode.AccessRightsRequestRejected

			// rejectedRequest = _userBanPermissionRequestManager.RejectRequest(permissionRequestId, DateTime.Now)
			// null - null

			//// Act
			var result = _controller.RejectRequest(accessRightsRequestId);

			//// Assert
			Assert.IsNotNull(result);
		}

		[Test]
		// DELETE
		[TestCase(true, true)]
		public void RemoveLegalOwnerPermissionRelation(bool isValidBan, bool isValidOwnerId)
		{
			//// Arrange
			var ban = GetBan(isValidBan); // valid
			var ownerId = GetOwnerId(isValidOwnerId); // valid

			// BAD Ban
			// banEntity = _banManager.GetBan(ban)
			// banEntity == null - SwitchrNotFoundException(ErrorCode.ErrorBanRelationNotFound

			// NOT LEGAL OWNER
			// !_userBanManager.IsLegalOwner(userId, banEntity.Id) - SwitchrForbiddenException(ErrorCode.UserHasNoLegalOwnerAccess

			// !_userBanManager.RemoveLegalOwnerAccessRights(banEntity.Id, userIdToRemove) - SwitchrException(ErrorCode.InternalServerError

			//// Act
			Assert.DoesNotThrow(() => _controller.RemoveLegalOwnerPermissionRelation(ban, ownerId), "should not throw");			
		}
	}
}
