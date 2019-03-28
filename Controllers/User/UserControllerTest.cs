using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using Switchr.API.Areas.User.Controllers;
using Switchr.API.Tests.Common;
using Switchr.Logic.Exceptions;
using Switchr.Logic.Gdpr;
using Switchr.Logic.Permissions;
using Switchr.Logic.Users;
using Switchr.Logic.Users.Interfaces;
using Switchr.Logic.Users.OTAC;
using Switchr.Models;
using Switchr.Models.CreditCheck;
using Switchr.Models.Permission;
using Switchr.Models.User;
using Role = Switchr.Models.Enums.Role;

namespace Switchr.API.Tests.Controllers.User
{
	[TestFixture]
	class UserControllerTest : UserTest {

		public override T CreateTestController<T>()
		{
			return CreateTestControllerWith<T>();
		}

		public T CreateTestControllerWith<T>(
			UserLogic user = null,
			UserServicesLogic userServices = null,
			SynchronizeUserLogic syncUser = null,
			IUserProfileLogic userProfile = null,
			IConsentLogic consent = null,
			IPasswordLogic password = null,
			OtacLogic otac = null,
			IHandoverCreditCheckLogic handoverCreditCheck = null,
			ILegacyBroadbandService legacyBroadband = null,
			IAccessPersonalDataLogic accessPersonalData = null
		) where T : ApiController, new()		
		{
			user = Eval<UserLogic> (Params.User, user);
			userServices = Eval<UserServicesLogic> (Params.UserServices, userServices);
			syncUser = Eval<SynchronizeUserLogic>(Params.SyncUser, syncUser);
			userProfile = Eval<IUserProfileLogic>(Params.UserProfile, userProfile);
			consent = Eval<IConsentLogic>(Params.Consent, consent);
			password = Eval<IPasswordLogic>(Params.Password, password);
			otac = Eval<OtacLogic>(Params.Otac, otac);
			handoverCreditCheck = Eval<IHandoverCreditCheckLogic>(Params.HandoverCreditCheck, handoverCreditCheck);
			legacyBroadband = Eval<ILegacyBroadbandService>(Params.LegacyBroadband, legacyBroadband);
			accessPersonalData = Eval<IAccessPersonalDataLogic>(Params.AccessPersonalData, accessPersonalData);

			var ctrl = new UserController (
				user,
				userServices,
				syncUser,
				userProfile,
				consent,
				password,
				otac,
				handoverCreditCheck,
				legacyBroadband,
				accessPersonalData
			);
			return ctrl as T;
		}

		// ======================================================================

		private UserController controller => GetController<UserController>();

		[TearDown]
		public void TearDown()
		{
			Cleanup();
		}

		[SetUp]
		public void SetUp () {
			base.Setup();
			
			// map for what type of error expected for each individual case
			errorMap = new Dictionary<string, ErrorType>();

			AddArgState("userId", "validUser");

			AddException<SwitchrForbiddenException>("validUser", ErrorCode.InvalidUserId);
			AddException<SwitchrForbiddenException>("matchingUser", ErrorCode.InvalidUserId);
			AddException<SwitchrException>("userFound", ErrorCode.UserNotFound);

			AddScenarioSet("allUser", new string[] { "validUser", "matchingUser", "userFound"});

			//// CreateUser
			AddArgException("userDetails", ErrorCode.ErrorWhenAddingUser, "data");
			AddArgException("customerInfo", ErrorCode.ErrorWhenAddingUser, "customer");
			AddArgException("adminConfig", ErrorCode.ErrorWhenAddingUser, "admin");

			// AddUserSubscription
			AddArgException("userSubscriptionData", ErrorCode.InvalidPhonenumber);
			AddArgException("phoneNumber", ErrorCode.InvalidPhonenumber);

			// ChangePassword
			AddArgException("emptyPassword", ErrorCode.InvalidParameter, "parameter");
			AddArgException("validPassword", ErrorCode.InvalidPassword, "password");
			AddArgException("matchingPasswords", ErrorCode.InvalidPassword, "password");

			// ChangeName
			AddArgException("userData", ErrorCode.InvalidParameter);
			AddArgException("firstName", ErrorCode.InvalidParameter);
			AddArgException("lastName", ErrorCode.InvalidPassword);

			// ChangeEmail
			AddArgException("privateEmail", ErrorCode.EmailIsAlreadyRegistered);
			AddArgException("businessEmail", ErrorCode.EmailIsAlreadyRegistered);

			// ChangeConsent
			AddArgException("changePermission", ErrorCode.ChangePermissionError);

			// RequestPersonalData
			AddArgException("validAccess", ErrorCode.InvalidParameter);
		}

		[Test]
		public void DummyControllerTest()
		{
			Assert.IsTrue(true);
		}

		// CreditCheck(AccomodationType accomodation, EmploymentType employment)
		[Test]
		[TestCase (AccomodationType.CONDOMINIUM, EmploymentType.EMPLOYEE)]
		public void CreditCheck(AccomodationType accommodationType, EmploymentType employmentType) {

			// creditCheckLogic.CheckCredit (ie. HandoverCheckCreditLogic.CheckCredit)
			// - SwitchrException(Models.ErrorCode.CreditCheckLookupFailed

			// _userManager.GetUser(userId)

			// personalIdentityLookupClient.LookupIndividual
			// DoCreditCheck
			// - joiceClient.EvaluateCredit

			// SwitchrException(Models.ErrorCode.CreditCheckLookupFailed

			//// Act
			var result = controller.CreditCheck (accommodationType, employmentType);

			//// Assert
			Assert.IsNotNull (result);
		}

		// UserEngagement(int userId)
		[Test]
		// [TestCase (true, true, true)]
		public void UserEngagementTest ()
		{
			AddDependency("userFound", "IUserManager");
			TestScenarios<UserController>("UserEngagement", allUser: true);			
		}

		public void UserEngagement(string scenarioLabel, Dictionary<string, bool> scenarioStates)
		{
			//// Arrange
			SetScenarioLabel(scenarioLabel);
			SetScenarios(scenarioStates);

			// ensure Fake Registry is fully configured at this point!
			SetupController<UserController>();

			// var userId = GetScenarioUserId();
			// AddAllUser(userFindDependency: "userServicesLogic.userManager");
			// User.UserId() != userId
			// - SwitchrForbiddenException(ErrorCode.InvalidUserId
			// userServicesLogic.GetUserEngagement(User.UserId())
			// - userManager.GetUser(userId)
			// -- user is null: SwitchrException(ErrorCode.UserNotFound

			// handoverLogic.GetReceiverStatus(userId)
			// log error (NOTE: No real Exception handling!)

			// Ok(new SuccessResponse<SwitchrUserServicesModel>(userModel));

			//// Act

			// assert depending on whether we expect it to return some kind of error result or OK response			
			// var result = _controller.UserEngagement(userId)
			var result = RunAsyncControllerMethod<UserController>("UserEngagement");
			AssertIt(result);
		}

		private Exception InvalidResultException(string v)
		{
			throw new NotImplementedException();
		}

		// UserBroadbandAccounts(int userId)
		[Test]
		[TestCase (true)]
		public async Task UserBroadbandAccounts (bool isValidUserId, bool userCanBeFound) {
			//// Arrange
			var userId = GetUserId (isValidUserId);

			// cdsService
			// userLegacyBroadbandManager.GetBroadbandSubscriptions(User.UserId())
			// - GetCustomerBroadband(userId)

			// user not found
			// - userManager.GetUser(userId)
			// -- SwitchrArgumentException(ErrorCode.InvalidUserId
			AddUserFound(userCanBeFound);
			// - cdsService.GetLegacyBroadbands

			// Ok(new SuccessResponse<BroadbandSubscriptionsResponse>(response))

			//// Act
			var result = await controller.UserBroadbandAccounts (userId);

			//// Assert
			AssertResult(result);
		}

		// UserInfo(int userId, string systemId = null)
		[Test]
		[TestCase (true, true, true, true)]
		public async Task UserInfo (bool isValidUserId, bool isValidSystemId, bool isMatchingUser, bool userCanBeFound) {
			//// Arrange
			var userId = GetUserId (isValidUserId);
			var systemId = GetSystemId (isValidSystemId);

			// User.UserId() != userId
			AddMatchingUser(isMatchingUser);

			// - SwitchrForbiddenException(ErrorCode.InvalidUserId
			// userProfileLogic.GetUserProfile
			// - userManager.GetUser(userId)
			// -- user is null: SwitchrNotFoundException(ErrorCode.UserNotFound
			AddUserFound(userCanBeFound);

			// - accountLogic.GetAccounts
			// - GetProfileConsentsAndChannelsAsync
			// - consentLogic.GetCustomerConsentsAndChannelsAsync
			// consentClient.GetCustomerConsentsAndChannelsAsync (ConsentApiClient)

			// Ok(new UserResponse<UserProfile>(userModel))

			//// Act
			var result = await controller.UserInfo (userId, systemId);

			//// Assert
			AssertResult(result);
		}


		// CreateUser(Request<UserDetailsRequest> userDetailRequest)
		[Test]
		[TestCase (true, true, true)]
		public async Task CreateUser (bool hasValidUserDetails, bool hasCustomerInfo, bool hasValidAdminConfig) {
			//// Arrange
			var userDetailsRequest = GetRequest< UserDetailsRequest>(hasValidUserDetails);

			// hasValidUserDetails
			// userLogic.CreateUserAsync(userDetailsRequest)
			AddScenario("userDetails", hasValidUserDetails);
			AddScenario("customerInfo", hasCustomerInfo);
			AddScenario("adminConfig", hasValidAdminConfig);

			// userDetailsRequest == null - SwitchrArgumentException(ErrorCode.ErrorWhenAddingUser

			// hasCustomerInfo
			// userDetailsRequest.CustomerInfo == null - SwitchrArgumentException(ErrorCode.ErrorWhenAddingUser
			if (!hasCustomerInfo)
			{
				var userDetailsReq = CreateMockedProp<object>("CustomerInfo", null);
				userDetailsRequest = CreateMockRequestFor<UserDetailsRequest>(userDetailsReq);
			}

			// hasValidAdminConfig
			// userDetailsRequest.CustomerInfo.Role == Role.Admin && string.IsNullOrEmpty(userDetailsRequest.OrganizationBan)
			// - SwitchrArgumentException(ErrorCode.ErrorWhenAddingUser
			if (!hasValidAdminConfig)
			{
				var userDetails= CreateMockedProp<object>("Role", Role.Admin);
				userDetails
						.SetNull("OrganizationBan")
						.SetNull("CustomerInfo");
				userDetailsRequest = CreateMockRequestFor<UserDetailsRequest>(userDetails);
			}

			//// Act
			var result = await controller.CreateUser (userDetailsRequest);

			//// Assert
			AssertResult(result);
		}

		// AddUserSubscription(Request<AddUserSubscription> userSubscription, int userId)
		[Test]
		[TestCase (true, true, true, true)]
		public async Task AddUserSubscription (bool isValidUserSubscription, bool isValidUserId, bool isMatchingUser, bool hasValidPhoneNumber) {
			//// Arrange
			var userId = GetUserId (isValidUserId);
			var userSubscriptionRequest = GetRequest<AddUserSubscription>(isValidUserSubscription);

			AddScenario("userSubscriptionData", isValidUserSubscription);
			AddScenario("phoneNumber", hasValidPhoneNumber);

			// hasMatchingUserId
			// User.UserId() != userId - SwitchrForbiddenException(ErrorCode.InvalidUserId
			AddMatchingUser(isMatchingUser);

			// hasValidPhoneNumber
			// string.IsNullOrEmpty(userSubscription?.Data?.Msisdn) - SwitchrArgumentException(ErrorCode.InvalidPhonenumber
			if (!isValidUserSubscription)
			{
				var userSub = CreateMockedProp<object>("Data", null);
				userSubscriptionRequest = CreateMockRequestFor<AddUserSubscription>(userSub);
			}

			if (!hasValidPhoneNumber)
			{
				var userSub = CreateMockedProp<object>("Data.Msisdn", null);
				userSubscriptionRequest = CreateMockRequestFor<AddUserSubscription>(userSub);
			}

			//// Act
			var result = await controller.AddUserSubscription (userSubscriptionRequest, userId);

			//// Assert
			AssertResult(result);
		}

		// ChangePassword(Request<ChangePassword> changePassword)
		[Test]
		[TestCase (true, true, true)]
		public void ChangePassword (bool isValidChangePassword, bool emptyPassword, bool isMatchingOldPassword) {
			//// Arrange
			var changePassword = GetChangePassword (isValidChangePassword);

			// emptyPassword
			// string.IsNullOrWhiteSpace(userdetailrequest?.Data?.NewPassword) || string.IsNullOrWhiteSpace(userdetailrequest.Data.OldPassword)
			// - SwitchrArgumentException(ErrorCode.InvalidParameter
			AddScenario("emptyPassword", emptyPassword);
			AddScenario("validPassword", isValidChangePassword);
			AddScenario("matchingPasswords", isMatchingOldPassword);

			// invalidPassword
			// passwordLogic
			// IsValidPassword(newpassword) - SwitchrArgumentException(ErrorCode.InvalidPassword
			if (!isValidChangePassword)
			{
				MockDependencyMethodCall<IPasswordLogic>("passwordLogic", "IsValidPassword", false);
			}

			// isMatchingOldPassword
			// passwordLogic
			// !ValidatePassword(oldpassword, _usermanager.GetPassword(oldpassword, userId))
			// SwitchrArgumentException(ErrorCode.InvalidPassword
			if (!isMatchingOldPassword)
			{
				MockDependencyMethodCall<IPasswordLogic>("passwordLogic", "ValidatePassword", false);
			}

			//// Act
			var result = controller.ChangePassword (changePassword);

			//// Assert
			AssertResult(result);
		}

		// ResetPassword(Request<PasswordResetDetails> passwordResetDetails)
		[Test]
		[TestCase (true)]
		public void ResetPassword (bool isValidPasswordReset) {
			//// Arrange
			var passwordReset = GetPasswordReset (isValidPasswordReset);

			// passwordLogic.ResetPassword
			// UserManager - DB error (SwitchrEntities context) - DataException

			//// Act
			var result = controller.ResetPassword (passwordReset);

			//// Assert
			AssertResult(result);
		}

		// RemoveUserSubscriptionRelation(int userId, int subscriptionId)
		[Test]
		[TestCase (true)]
		public async Task RemoveUserSubscriptionRelation (bool isValidUserId, bool isValidSubscriptionId, bool userCanBeFound) {
			//// Arrange
			var userId = GetUserId (isValidUserId);
			var subscriptionId = GetSubscriptionId (isValidSubscriptionId);

			// userLogic.RemoveSubscriptionRelation

			// missing check is user matches? (see Profile below) - ie. InvalidUserId

			// isValidUserId
			// userProfileLogic.GetUserProfile
			// userManager.GetUser(userId) - user null - SwitchrNotFoundException(ErrorCode.UserNotFound
			AddUserFound(userCanBeFound);

			//// Act
			var result = await controller.RemoveUserSubscriptionRelation (userId, subscriptionId);

			//// Assert
			AssertResult(result);
		}

		// Profile(int userId)
		[Test]
		[TestCase (true)]
		public async Task Profile (bool isValidUserId, bool isMatchingUser, bool userCanBeFound) {
			//// Arrange
			var userId = GetUserId (isValidUserId);

			// User.UserId() != userId - SwitchrForbiddenException(ErrorCode.InvalidUserId
			AddMatchingUser(isMatchingUser);

			// isValidUserId
			// userProfileLogic.GetUserProfile
			// userManager.GetUser(userId) - user null - SwitchrNotFoundException(ErrorCode.UserNotFound
			AddUserFound(userCanBeFound);

			//// Act
			var result = await controller.Profile (userId);

			//// Assert
			AssertResult(result);
		}

		// ChangeName(Request<UserName> userName)
		[Test]
		[TestCase (true, true, true)]
		public async Task ChangeName (bool hasUserData, bool isValidFirstName, bool isValidLastName) {
			//// Arrange
			var userNameRequest = GetUserName (hasUserData);

			AddScenario("userData", hasUserData);
			AddScenario("firstName", isValidFirstName);
			AddScenario("lastName", isValidLastName);

			// name is empty
			// string.IsNullOrWhiteSpace(userName?.Data?.FirstName) || string.IsNullOrWhiteSpace(userName.Data.LastName)
			// - SwitchrArgumentException(ErrorCode.InvalidParameter
			if (!isValidFirstName)
			{
				var userName = CreateMockedProp<object>("Data.FirstName", null);
				userNameRequest = CreateMockRequestFor<UserName>(userName);
			}

			if (!isValidLastName)
			{
				var userName = CreateMockedProp<object>("Data.LastName", null);
				userNameRequest = CreateMockRequestFor<UserName>(userName);
			}

			// userProfileLogic.ChangeName - false - SwitchrException(ErrorCode.ExternalCallFailed

			//// Act
			var result = await controller.ChangeName (userNameRequest);

			//// Assert
			AssertResult(result);
		}

		// SynchronizeUserEngagements()
		[Test]
		[TestCase (true)]
		public async Task SynchronizeUserEngagements (bool userCanBeFound) {
			//// Arrange

			// synchronizeUserLogic.SynchronizeUserEngagement
			// User user = this._userManager.GetUser(userId);
			// user == null - SwitchrException(ErrorCode.InvalidUserId
			AddUserFound(userCanBeFound);

			//// Act
			var result = await controller.SynchronizeUserEngagements ();

			//// Assert
			AssertResult(result);
		}

		// ChangeEmail(Request<UserEmail> useremail)
		[Test]
		[TestCase (true, true, true, true)]
		public async Task ChangeEmail (bool isValidUserEmail, bool isValidUser, bool userCanBeFound, bool isNewPrivateEmail, bool isNewBusinessEmail) {
			//// Arrange
			var userEmail = GetUserEmail (isValidUserEmail);

			// userProfileLogic.ChangeEmail
			// static: EmailHelper.IsValidEmail(email) - SwitchrArgumentException(ErrorCode.InvalidEmail
			// TODO: add static method that returns EmailHelper
			MockDependencyMethodCall<IUserProfileLogic>("userProfileLogic", "IsValidEmail", false);

			AddScenario("privateEmail", isNewPrivateEmail);
			AddScenario("businessEmail", isNewBusinessEmail);

			// userManager.GetUser(userId)
			// user == null - SwitchrNotFoundException(ErrorCode.InvalidUserId
			AddUserFound(userCanBeFound);

			// private email is already registered
			// EmailExistsFor(user, email, Segment.Private)
			// - SwitchrArgumentException(ErrorCode.EmailIsAlreadyRegistered
			if (!isNewPrivateEmail)
			{
				MockDependencyMethodCall<IUserProfileLogic>("userProfileLogic", "IsPrivateUser", true);
				MockDependencyMethodCall<IUserProfileLogic>("userProfileLogic", "IsBusinessUser", false);
				MockDependencyMethodCall<IUserProfileLogic>("userProfileLogic", "EmailExistsFor", false);
			}

			// public email is already registered
			// EmailExistsFor(user, email, Segment.Business)
			// - SwitchrArgumentException(ErrorCode.EmailIsAlreadyRegistered
			if (!isNewBusinessEmail)
			{
				MockDependencyMethodCall<IUserProfileLogic>("userProfileLogic", "IsPrivateUser", false);
				MockDependencyMethodCall<IUserProfileLogic>("userProfileLogic", "IsBusinessUser", true);
				MockDependencyMethodCall<IUserProfileLogic>("userProfileLogic", "EmailExistsFor", false);
			}

			//// Act
			var result = await controller.ChangeEmail (userEmail);

			//// Assert
			AssertResult(result);
		}

		// ChangeConsent(int userId, string permissionId, Request<UserConsent> userPermissionRequest, string systemId)
		[Test]
		[TestCase (true, true, true, true, true, true)]
		public async Task ChangeConsent(bool isValidUserId, bool isMatchingUser, bool isValidPermissionId, bool isValidPermission, bool isValidSystemId, bool hasPermissionData) {
			//// Arrange
			var userId = GetUserId (isValidUserId);
			var permissionId = GetPermissionId (isValidPermissionId);
			var userPermissionRequest = GetPermission (isValidPermission);
			var systemId = GetSystemId (isValidSystemId);

			// isMatchingUser
			// User.UserId() != userId - SwitchrForbiddenException(ErrorCode.InvalidUserId
			AddMatchingUser(isMatchingUser);
			AddScenario("changePermission", hasPermissionData);

			// hasPermissionData
			// userPermissionRequest?.Data == null - SwitchrArgumentException(ErrorCode.ChangePermissionError
			// consentLogic.ChangeConsentAsync - OK
			if (!hasPermissionData)
			{
				var userPermission = CreateMockedProp<object>("Data", null);
				userPermissionRequest = CreateMockRequestFor<UserConsent>(userPermission);
			}

			//// Act
			var result = await controller.ChangeConsent(userId, permissionId, userPermissionRequest, systemId);

			//// Assert
			AssertResult(result);
		}

		// RequestPersonalData([FromBody]Request<AccessRequestModel> accessRequest, [FromUri]string brandId = null)
		[Test]
		[TestCase (true, true)]
		public async Task RequestPersonalData (bool isValidAccess, bool isValidBrandId) {
			//// Arrange
			var accessRequest = GetAccess (isValidAccess);
			var brandId = GetBrandId (isValidBrandId);

			AddScenario("validAccess", isValidAccess);
			// accessRequest == null - SwitchrArgumentException(ErrorCode.InvalidParameter
			// accessPersonalDataLogic.RequestPersonalData - OK

			//// Act
			var result = await controller.RequestPersonalData (accessRequest, brandId);

			//// Assert
			AssertResult(result);
		}

		// UserSelfserviceProfile(string ssn)
		[Test]
		[TestCase (true)]
		public async Task UserSelfServiceProfile (bool isValidSsn) {
			//// Arrange
			var ssn = GetSsn (isValidSsn);

			// userProfileLogic.GetUserDetailsBySSN

			//// Act
			var result = await controller.UserSelfserviceProfile (ssn);

			//// Assert
			AssertResult(result);
		}
	}
}