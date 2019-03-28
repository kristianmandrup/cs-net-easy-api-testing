using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Web.Http;
using Moq;
using StructureMap;
using Switchr.API.Tests.Common.Db;
using Switchr.Data.DataModel;
using Switchr.Logic.Services.Cds.interfaces;
using Switchr.Models;
using Switchr.Models.SSN;
using Switchr.Models.Gdpr;
using Switchr.Models.Permission;
using Switchr.Models.User;
using Switchr.Data.Interfaces;
using Switchr.Models.SMS;
using Switchr.Models.Handover;
using Switchr.Logic.Exceptions;
using ChangePassword = Switchr.Models.User.ChangePassword;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Security.Claims;
using System.Reflection;
using System.Linq;

namespace Switchr.API.Tests.Common
{
	abstract public class BaseControllerTest
	{
		// protected ApiController _controller;
		protected Dictionary<string, string> ArgStateMap = new Dictionary<string, string>();
		protected Dictionary<string, bool> ScenarioStates = new Dictionary<string, bool>();
		protected string ScenarioLabel;
		protected string TestLabel;

		public readonly MockFactory mockFactory = MockFactory.Create();
		protected ConfigHelper validConf;
		protected ConfigHelper invalidConf;

		// protected IList<string> keyList;
		// protected IDictionary<int, object> keyMap;

		protected List<string> errorCheckOrder;
		protected Dictionary<string, Dictionary<string, bool>> ScenariosMap => TestMap[TestLabel];

		protected Dictionary<string, Dictionary<string, Dictionary<string, bool>>> TestMap = new Dictionary<string, Dictionary<string, Dictionary<string, bool>>>();

		protected Dictionary<string, ErrorType> errorMap = new Dictionary<string, ErrorType>();
		protected Dictionary<string, string> dependencyMap = new Dictionary<string, string>();
		protected Dictionary<string, string[]> scenarioSets = new Dictionary<string, string[]>();

		protected Container container;
		protected Enum ParamsEnum;

		protected DependencyMocker dependencyMocker;

		protected FakeRegistry Registry = new FakeRegistry();

		protected SwitchrEntities Context;

		protected ApiController _controller;

		// TODO: We can update a map that shows for which types we used default value and which not (can be part of test output)
		protected T useOrDefault<T>(object value, object defaultValue = null) where T: class
		{
			defaultValue = defaultValue ?? GetInst<T>();
			return (value ?? defaultValue) as T;
		}
		

		protected T GetController<T>() where T: ApiController
		{
			return (T) _controller;
		}

		protected void SetController<T>(T controller) where T : ApiController
		{
			_controller = controller;
		}

		protected T GetProperty<T>(string name, object target = null)
		{
			target = target != null ? target : this;
			var type = target.GetType();
			var prop = type.GetProperty(name);
			return (T) prop.GetValue(target, null);
		}


		protected void SetScenarioLabel(string scenarioLabel)
		{
			ScenarioLabel = scenarioLabel;
			if (ScenariosMap.ContainsKey(scenarioLabel)) return;
			ScenariosMap.Add(scenarioLabel, new Dictionary<string, bool>());
		}

		protected void SetTestLabel(string testLabel)
		{
			TestLabel = testLabel;
			if (TestMap.ContainsKey(testLabel)) return;
			TestMap.Add(testLabel, new Dictionary<string, Dictionary<string, bool>>());
		}


		protected void SetScenario(string key, bool scenarioState)
		{
			if (GetCurrentScenarios.ContainsKey(key)) return;
			GetCurrentScenarios.Add(key, scenarioState);			
		}

		protected Dictionary<string, bool> CalcScenarioAllStates(bool allState)
		{
			var stateDict = new Dictionary<string, bool>();
			foreach (var scenario in GetCurrentScenarios)
			{
				stateDict.Add(scenario.Key, allState);
			}

			return stateDict;
		}

		protected void AddDependency(string key, string dependencyPath)
		{
			dependencyMap.Add(key, dependencyPath);
		}
		

		protected Dictionary<string, bool> CalcScenarioStates(string stateScenario, string[] scenarioKeys)
		{
			var stateDict = new Dictionary<string, bool>();
			foreach (var key in scenarioKeys)
			{
				stateDict.Add(key, key != stateScenario);
			}

			return stateDict;
		}

		protected void SetScenarios(Dictionary<string, bool> scenarioStates, bool allUser = true)
		{
			ScenarioStates = scenarioStates;
			foreach (var scenarioState in scenarioStates)
			{
				SetScenario(scenarioState.Key, scenarioState.Value);
			}

			if (allUser)
			{
				AddAllUser();
			}			
		}

		protected Dictionary<string, bool> GetCurrentScenarios => ScenarioLabel != null ? ScenariosMap[ScenarioLabel] : new Dictionary<string, bool>();

		protected Dictionary<string, Dictionary<string, bool>> GetCurrentTestScenarios => TestMap[TestLabel];

		protected object RunStateMethod(string stateKey, string methodName, object param = null)
		{
			if (ScenarioStates.Count == 0)
			{
				throw new Exception($"Missing scenario states");
			}

			if (!ScenarioStates.ContainsKey(stateKey))
			{
				throw new Exception($"Missing scenario state method { methodName } for { stateKey }");
			}

			var state = ScenarioStates[stateKey];
			var args = new List<object> { state };
			if (param != null)
			{
				param = (string) param == "<null>" ? null : param;
				args.Add(param);
			}

			var argsArr = args.ToArray();

			return RunMethod<object>(methodName, argsArr, this);			
		}

		protected string capitalize(string str)
		{
			return char.ToUpper(str[0]) + str.Substring(1);
		}

		protected object GetArgValue(string argKey, string stateKey)
		{
			var method = $"Get{capitalize(argKey)}";
			return RunStateMethod(stateKey, method);
		}

		protected List<object> scenarioArgValue(List<object> acc, string pkey, string pval) {
			var value = GetArgValue(pkey, pval);
			if (value != null) acc.Add(value);
			return acc;
		}

		protected object[] ScenarioMethodArgs()
		{
			var empty = new List<object>();
			return ArgStateMap.Aggregate(empty, (acc, pair) => scenarioArgValue(acc, pair.Key, pair.Value)).ToArray();
		}

		protected IHttpActionResult RunControllerMethod<T>(string methodName, object controller = null) where T : ApiController
		{
			controller = controller != null ? controller : GetController<T>();
			var methodArgs = ScenarioMethodArgs();
			var result = RunMethod<IHttpActionResult>(methodName, methodArgs, controller);
			return result;
		}

		protected Task<IHttpActionResult> RunAsyncControllerMethod<T>(string methodName, object controller = null) where T : ApiController
		{
			controller = controller != null ? controller : GetController<T>();
			var methodArgs = ScenarioMethodArgs();
			var result =RunMethod<Task<IHttpActionResult>>(methodName, methodArgs, controller);
			return result;
		}

		protected T RunMethod<T>(string methodName, object[] args, object target = null) where T : class
		{
			target = target != null ? target : this;
			if (!HasMethod(methodName, target))
			{
				throw new Exception($"{ target }No method named: { methodName }");
			}
			var type = target.GetType();

			// A bitmask comprised of one or more BindingFlags that specify how the search is conducted.
			// The access can be one of the BindingFlags such as Public, NonPublic, Private, InvokeMethod, GetField, and so on.
			// The type of lookup need not be specified.
			// If the type of lookup is omitted, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static are used.
			// https://docs.microsoft.com/en-us/dotnet/api/system.type.invokemember?view=netframework-4.7.2
			var flags = BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			try
			{
				MethodInfo method = type.GetMethod(methodName, flags);
				var result = method.Invoke(target, args);
				return result as T;
				// return type.InvokeMember(methodName, flags, null, target, args);
			}
			catch (MissingMethodException ex)
			{
				Debug.WriteLine($"{target} of type {type} missing instance method {methodName} to invoke");
				throw ex;
			}
			catch (InvalidResultException ex)
			{
				Debug.WriteLine($"Invalid result");
				return null;
			}

			catch (TargetInvocationException ex)
			{
				Debug.WriteLine($"{target} of type {type} method {methodName} could not be invoked: {args}. Reason: { ex.Message}");
				return null;
			}

		}

		public bool HasMethod(string methodName, object target = null)
		{
			target = target != null ? target : this;
			var type = target.GetType();
			var hasIt = type.GetMethod(methodName) != null;
			return hasIt;
		}

		// TODO: this map should be configured in Setup
		protected void AddAllUser()
		{
			// TODO: perhaps make RunStateMethod more generic by allowing passing params array to be concatenated
			RunStateMethod("validUser", "AddValidUser");
			RunStateMethod("matchingUser", "AddMatchingUser", "<null>");
			RunStateMethod("userFound", "AddUserFound");
		}

		protected void AddScenarioSet(string scenarioSetName, string[] scenarioKeys)
		{
			scenarioSets.Add(scenarioSetName, scenarioKeys);
		}

		private string GetDependencyClass(string key)
		{
			return dependencyMap.ContainsKey(key) ? dependencyMap[key] : null;
		}

		// TODO: pass (or configure) list of scenario sets to be used instead of passing bool
		protected void TestScenarios<T>(string testMethod, bool allUser = false) where T : ApiController
		{
			// true for allSetS
			SetTestLabel(testMethod);
			string[] scenarioKeys = allUser ? scenarioSets["allUser"] : GetCurrentScenarios.Keys.ToArray();
			if (scenarioKeys.Length == 0)
			{
				HandleNoTestScenarios();
			}

			// run test with false for one of each specific scenario
			foreach (var scenarioKey in scenarioKeys)
			{
				SetScenarioLabel(scenarioKey);
				var scenarioStates = CalcScenarioStates(scenarioKey, scenarioKeys);
				RunTestScenario<T, object>(testMethod, scenarioStates);
			}
			SetScenarioLabel("OK");
			RunTestScenario<T, object>(testMethod, CalcScenarioAllStates(true));
		}

		protected void HandleNoTestScenarios()
		{
			var msg = $"Missing scenarios to test for: {ScenarioLabel}";
			Debug.WriteLine(msg);
			// throw new Exception(msg);
		}

		protected void RunTestScenario<T, V>(string testMethod, Dictionary<string, bool> scenarioStates, bool allUser = false) 
		where T: ApiController
		where V : class
		{
			SetScenarios(scenarioStates, allUser);
			var args = new object[] {ScenarioLabel, scenarioStates};
			if (HasMethod(testMethod))
			{
				RunMethod<V>(testMethod, args, this);
			}
			else
			{
				RunDefaultTestScenario<T>(testMethod, scenarioStates, allUser);
			}
		}

		protected void RunDefaultTestScenario<T>(string testMethod, Dictionary<string, bool> scenarioStates, bool allUser = false) where T : ApiController
		{
			SetScenarios(scenarioStates, allUser);
			var result = RunControllerMethod<T>(testMethod);
			AssertIt(result as IHttpActionResult);
		}

		protected void AddArgState(string argKey, string stateKey)
		{
			ArgStateMap.Add(argKey, stateKey);
		}
		
		protected int GetScenarioUserId()
		{
			var isValidUserId = ScenarioStates["validUserId"];
			return GetUserId(isValidUserId);

		}

		public void AddValidUser(bool isInvalidUser)
		{
			AddMatchingUser(isInvalidUser, "validUser");
		}

		public void AddMatchingUser(bool isMatchingUser, string scenarioLabel = null)
		{
			scenarioLabel = scenarioLabel ?? "matchingUser";
			AddScenario(scenarioLabel, isMatchingUser);
			MockMatchingUser(isMatchingUser);
		}

		public void AddUserFound(bool userCanBeFound)
		{
			AddScenario("userFound", userCanBeFound);			
			MockFindUser(userCanBeFound);
		}
		
		protected BaseControllerTest AddScenario(string key, bool condition)
		{
			if (!errorCheckOrder.Contains(key))
			{
				errorCheckOrder.Add(key);
			}

			var scenarios = GetCurrentScenarios;
			if (scenarios.ContainsKey(key)) return this;
			scenarios.Add(key, condition);
			return this;
		}

		protected BaseControllerTest AddException<T>(string key, ErrorCode errorCode, string msg = null) where T: Exception
		{
			AddError(key, Exception<T>(errorCode, msg));
			return this;
		}

		protected BaseControllerTest AddArgException(string key, ErrorCode errorCode, string msg = null)
		{
			AddError(key, Exception<SwitchrArgumentException>(errorCode, msg));
			return this;
		}

		protected BaseControllerTest AddError(string key, ErrorType errorType)
		{
			errorMap.Add(key, errorType);
			return this;
		}

		protected IPrincipal MockedPrincipal(int userId)
		{
			return new TestPrincipal(new Claim("sub", userId.ToString()));
		}

		protected void MockDependencyMethodCall<T>(string dependencyName, string methodName, object value) where T : class
		{
			dependencyMocker.MockDependencyMethodCall<T>(dependencyName, methodName, value);
		}

		protected void MockDependencyProperty<T>(string dependencyName, string propName, object value) where T : class
		{
			dependencyMocker.MockDependencyProperty<T>(dependencyName, propName, value);
		}

		protected void MockStaticCall<T>(string dependencyName, string methodName, object value) where T : class
		{
			dependencyMocker.MockDependencyStaticCall<T>(dependencyName, methodName, value);
		}


		public ErrorType Validation(string name)
		{
			return ErrorType.CreateValidation(name);
		}

		public ErrorType Exception<T>(ErrorCode code, string msg = null) where T: Exception
		{
			return ErrorType.CreateException<T>(code, msg);
		}

		public BaseControllerTest MockUserCases(bool isMatchingUser, bool userCanBeFound)
		{
			AddMatchingUser(isMatchingUser);
			AddUserFound(userCanBeFound);

			MockMatchingUser(isMatchingUser);
			MockFindUser(userCanBeFound);
			return this;
		}

		public BaseControllerTest MockMatchingUser(bool isMatchingUser)
		{
			int userId = GetUserId(isMatchingUser);
			dependencyMocker.MockProperty<IPrincipal>("User", MockedPrincipal(userId));
			return this;
		}


		public BaseControllerTest MockFindUser(bool userCanBeFound)
		{
			if (!userCanBeFound)
			{
				var dependencyClass = GetDependencyClass("userFound");
				var members = new Dictionary<string, object>(); 
				members.Add("GetUser", null);

				// TODO: make generic or use param from ArgMap
				var argVal = GetArgValue("userId", "validUser");
				var anyInt = It.IsAny<int>();
				var arg = (int) (argVal ?? anyInt);

				Debug.WriteLine($"userId: { argVal} { arg }");

				var mockedUserManager = new Mock<IUserManager>();
				mockedUserManager.Setup(m => m.GetUser(arg)).Returns((User) null);

				var userManager = mockedUserManager.Object;
				// Registry.Use(userManager);
				container.Inject<IUserManager>(userManager);
				// var mockedUserManager = DependencyMocker.MockClassMethods<IUserManager>(dependencyClass, members);

				// need to Configure new Container with new Registry
				// ReconfigureRegistry();
			}
			return this;
		}

		protected bool IsInvalidScenario(Dictionary<string, bool> scenarioMap, string scenarioKey)
		{
			return !scenarioMap[scenarioKey];
		}
		

		protected ResultExpectation GetResultExpectation(Dictionary<string, bool> scenarioMap, List<string> errorCheckOrder)
		{
			var errorKey = GetFirstMatchingErrorKey(scenarioMap, errorCheckOrder);
			return (errorKey != null) ? CreateError(errorKey) : CreateOK();
		}

		protected string GetFirstMatchingErrorKey(Dictionary<string, bool> scenarioMap, List<string> errorCheckOrder)
		{
			return errorCheckOrder.Find(key => IsInvalidScenario(scenarioMap, key));
		}

		protected ResultExpectation CreateError(string errorKey)
		{
			return ResultExpectation.CreateError(errorMap[errorKey], errorKey);
		}

		protected ResultExpectation CreateOK()
		{
			return ResultExpectation.CreateOK();
		}

		protected void AssertResult(IHttpActionResult result, string label = null)
		{
			label = label ?? ScenarioLabel;
			var resultExpectation = GetResultExpectation(GetCurrentScenarios, errorCheckOrder);
			var checker = ResponseChecker.Create(result, label);
			checker.SetScenarioLabel(ScenarioLabel);
			Debug.WriteLine($"Check response for: { ScenarioLabel} expecting { resultExpectation} to match result { result }");
			checker.CheckResponse(resultExpectation);
		}

		protected void AssertIt(IHttpActionResult result, string label = null)
		{
			try
			{
				AssertResult(result);
			}
			catch (Exception ex)
			{
				AssertResult((dynamic) ex);
			}
		}

		protected void AssertIt(Exception ex, string label = null)
		{
			AssertResult(ex);
		}

		protected void AssertIt(Task<IHttpActionResult> task, string label = null)
		{
			try
			{
				AssertIt((dynamic) task.Result);
			}
			catch (Exception ex)
			{
				AssertResult((dynamic)ex);
			}
		}


		protected void AssertResult(Exception result, string label = null)
		{
			label = label ?? ScenarioLabel;
			var resultExpectation = GetResultExpectation(GetCurrentScenarios, errorCheckOrder);
			var checker = ResponseChecker.Create(result, label);
			checker.CheckResponse(resultExpectation);
		}

		protected void ResetDependencyMap()
		{
			dependencyMap = new Dictionary<string, string>();
		}

		protected void ResetScenarioStates()
		{
			ScenarioStates = new Dictionary<string, bool>();
		}
			

		protected void Cleanup()
		{
			ResetDependencyMap();
			ResetScenarioStates();
		}

		protected void AssertResult(Task task, string label = null)
		{
			throw new InvalidResultException("Error: Result is an async task that must be awaited to provide the result to be tested");
		}


		protected static Mock<DbSet<T>> CreateMockedDbSet<T>(IEnumerable<T> dataSet) where T : class
		{			
			var dbSet = CreateMocked<DbSet<T>>();
			DbMocker<T> dbMocker = new DbMocker<T>(dbSet, dataSet);
			string[] values = new[] { "Provider", "Expression", "ElementType", ":GetEnumerator" };			
			dbMocker.CreateMockedDb<T>(values);
			return dbSet.MockObj;
		}

		// TODO: Use dependencyMocker to mock nested object
		public static MockMoq<T> CreateMockedProp<T>(string name, object value = null) where T : class
		{
			var mocker = new DependencyMocker();
			mocker.SetPropPath(name, value);
			return mocker.mockTarget as MockMoq<T>;
		}

		public static MockMoq<T> CreateMockedDepProp<T>(string dependencyName, string methodName, object value = null) where T : class
		{
			var mocker = new DependencyMocker();
			mocker.MockDependencyMethodCall<T>(dependencyName, methodName, value);
			return mocker.mockTarget as MockMoq<T>;
		}

		public static MockMoq<T> AddMockedProp<T>(MockMoq<T> mockObj, string name, object value = null) where T : class
		{
			return mockObj.SetProperty(name, value);
		}

		public static MockMoq<T> CreateMocked<T>() where T: class
		{
			return MockMoq<T>.Create();
		}

		public void MockDb()
		{
			// Mock the DB
			Context = CreateMock<SwitchrEntities>();
			Registry.AddContext(Context);

			// ARRANGE
			var fixtures = new FixtureLoader().LoadJson("data.json");
			var devDeta = fixtures.GetEnv("dev").data;
			var users = devDeta.users;

			// Mock the user table
			// generate from JSON file
			var userDataSet = new DbDataBuilder().BuildUsers(users);

			var userDbSet = CreateMockedDbSet(userDataSet);
		}

		void MockCDSService()
		{
			var mockCDS = CreateMocked<ICdsService>();
			Registry.Add<ICdsService>(mockCDS.Value);
			// mockCDS.OnCall(...)
		}


		void SetupHateos(ApiController controller)
		{
			var urlHelper = new System.Web.Http.Routing.UrlHelper();
			urlHelper.Request = new System.Net.Http.HttpRequestMessage();
			urlHelper.Request.RequestUri = new System.Uri("http://localhost/test");
			urlHelper.Request.Method = System.Net.Http.HttpMethod.Get;
			controller.RequestContext.Url = urlHelper;
		}

		public T GetInst<T>()
		{
			return container.GetInstance<T>();
		}

		abstract public T CreateTestController<T>() where T : ApiController, new();
		

		public void SetupController<T>() where T: ApiController, new()
		{
			var controller = CreateTestController<T>();
			SetController<T>(controller);
			dependencyMocker = new DependencyMocker(controller);
		}

		protected void ResetScenariosMap()
		{
			TestMap[TestLabel] = new Dictionary<string, Dictionary<string, bool>>();
		}

		protected void ResetArgStateMap()
		{
			ArgStateMap = new Dictionary<string, string>();
		}

		protected void ResetErrorCheckOrder()
		{
			errorCheckOrder = new List<string>();
		}

		protected void ReconfigureRegistry()
		{
			container = new Container(x => x.AddRegistry(Registry));
		}
		
		public void Setup()
		{
			ReconfigureRegistry();
			validConf = ConfigHelper.Create("valid.ini");
			invalidConf = ConfigHelper.Create("invalid.ini");

			ResetErrorCheckOrder();
			// ResetScenariosMap();
			ResetArgStateMap();

			// registering FAKES.
			// _container.EjectAllInstancesOf<IOrderingService>(); //ejecting FAKE SUT
			// _container.Configure(x => x.AddType(typeof(IOrderingService), typeof(OrderingService))); //registering SUT			
		}

		protected ConfigHelper Config(bool isValid)
		{
			return isValid ? validConf : invalidConf;
		}


		abstract protected object GetLookupValue(Enum key);

		protected T Eval<T>(Enum enumId, object value)
		{
			var defaultValue = GetLookupValue(enumId);
			var eval = value != null ? value : defaultValue;
			return (T) eval;
		}

		protected T CreateMock<T>(bool isValid = true) where T: class
		{
			return isValid ? mockFactory.CreateMock<T>() : null;
		}

		protected Request<T> CreateMockRequest<T>(bool isValid = true) where T : class
		{
			return isValid ? CreateMock <Request<T>>() : InvalidRequest<T>();
		}

		protected Request<T> CreateMockRequestFor<T>(object value = null) where T : class
		{
			return value as Request<T>;
		}

		// alias: delegate
		protected Request<T> GetRequest<T>(bool isValid = true) where T : class
		{
			return CreateMockRequest<T>(isValid);
		}

		protected Request<T> GetRequestFor<T>(object value = null) where T : class
		{
			return CreateMockRequestFor<T>(value);
		}

		protected Request<DateTime> GetDateTimeRequest(bool isValid = true)
		{
			return isValid ? CreateMock<Request<DateTime>>() : null;
		}


		protected Request<T> InvalidRequest<T>() where T : class
		{
			return null;
		}

		protected int GetTimeStamp(bool isValidTimeStamp)
		{
			return Config(isValidTimeStamp).GetTimeSecs("timestamp");
		}

		protected string GetNonce(bool isValidNonce)
		{
			return Config(isValidNonce).GetStr("nonce");
		}

		protected string GetSignature(bool isValidSignature)
		{
			return Config(isValidSignature).GetStr("signature");
		}

		protected string GetSim(bool isValid = true)
		{
			return Config(isValid).GetStr("sim");
		}

		protected int GetBan(bool isValid = true)
		{
			return Config(isValid).GetInt("ban");
		}

		protected string GetBanStr(bool isValid = true)
		{
			return GetBan(isValid).ToString();
		}

		protected string GetInvoiceNumber(bool isValidInvoice = true)
		{
			return Config(isValidInvoice).GetStr("invoiceNumber");
		}

		protected string GetDepositDate(bool isValidDepositDate = true)
		{
			return Config(isValidDepositDate).GetStr("depositDate");
		}

		protected int GetAmount(bool isValidAmount = true)
		{
			return Config(isValidAmount).GetInt("amount");
		}

		protected string GetAmountStr(bool isValidAmount = true)
		{
			return GetAmount(isValidAmount).ToString();
		}

		// required for reflection (Dynamic method invoke) to work
		// See: https://docs.microsoft.com/en-us/dotnet/api/system.reflection.methodbase.invoke?view=netframework-4.7.2
		public string GetUserIdStr(bool isValid = true)
		{
			return GetUserId(isValid).ToString();
		}

		public int GetUserId(bool isValid = true)
		{
			return Config(isValid).GetInt("userId");
		}

		protected string GetMsisdn(bool isValid = true)
		{
			return Config(isValid).GetVal("msisdn");
		}

		protected string GetSystemId(bool isValid = true)
		{
			return Config(isValid).GetVal("systemId");
		}

		protected string GetSsn(bool isValidSsn)
		{
			return Config(isValidSsn).GetStr("ssn");
		}

		protected string GetOrderId(bool isValid = true)
		{
			return Config(isValid).GetVal("orderId");
		}

		protected string GetDeliveryId(bool isValid = true)
		{
			return Config(isValid).GetVal("deliveryId");
		}

		protected string GetOrderAction(bool isValid = true)
		{
			return Config(isValid).GetVal("orderAction");
		}

		protected string GetToken(bool isValidToken = true)
		{
			return Config(isValidToken).GetVal("token");
		}

		protected string GetProductCode(bool isValid)
		{
			return Config(isValid).GetVal("productCode");
		}

		protected int GetOwnerId(bool isValidOwnerId = true)
		{
			return Config(isValidOwnerId).GetInt("ownerId");
		}

		protected int GetAccessRightsRequestId(bool isValidAccessRightsRequestId)
		{
			return Config(isValidAccessRightsRequestId).GetInt("accessRightsRequestId");
		}

		protected int GetCvr(bool isValid)
		{
			return Config(isValid).GetInt("cvr");
		}

		protected string GetEmail(bool isValidEmail)
		{
			return Config(isValidEmail).GetStr("email");
		}

		protected string GetCreditAgreementId(bool isValid)
		{
			return Config(isValid).GetVal("creditAgreementId");
		}

		protected string GetUsername(bool isValidUsername)
		{
			return Config(isValidUsername).GetStr("userName");
		}

		protected MobileNumberCodesModel GetSmsModel(bool isValidModel)
		{
			return CreateMock<MobileNumberCodesModel>(isValidModel);
		}

		protected Request<PatchDocument<AcceptHandoverData>> GetHandoverData(bool isValid = true)
		{
			return GetRequest<PatchDocument<AcceptHandoverData>>(isValid);
		}

		protected Request<StartHandoverData> GetStartHandoverData(bool isValid = true)
		{
			return GetRequest<StartHandoverData>(isValid);
		}

		protected Request<OrderSimInfo> GetOrderSimInfo(bool isValid = true)
		{
			return GetRequest<OrderSimInfo>(isValid);
		}

		protected Request<NemIdSSNDetail> NemIdSSNRequest(bool isValidNemIdSSN = true)
		{
			return GetRequest<NemIdSSNDetail>(isValidNemIdSSN);
		}

		protected Request<AddTopUp> GetTopUpRequest(bool isValidTopUp = true)
		{
			return GetRequest<AddTopUp>(isValidTopUp);
		}

		protected int GetYear(bool isValid = true)
		{
			return Config(isValid).GetInt("year");
		}

		protected int GetMonth(bool isValid = true)
		{
			return Config(isValid).GetInt("month");
		}

		protected string GetSubscriptionType(bool isValid = true)
		{
			return Config(isValid).GetVal("subscriptionType");
		}

		protected int GetPagSize(bool isValid = true)
		{
			return Config(isValid).GetInt("pageSize");
		}

		protected int GetOffset(bool isValid = true)
		{
			return Config(isValid).GetInt("offset");
		}

		protected string GetCategory(bool isValid = true)
		{
			return Config(isValid).GetVal("category");
		}


		protected int GetSubscriptionId(bool isValidSubscriptionId = true)
		{
			return Config(isValidSubscriptionId).GetInt("subscriptionId");
		}

		protected Request<UserDetailsRequest> GetUserDetails(bool isValidUserDetails = true)
		{
			return GetRequest<UserDetailsRequest>(isValidUserDetails);
		}

		protected Request<AddUserSubscription> GetUserSubscription(bool isValidUserSubscription = true)
		{
			return GetRequest<AddUserSubscription>(isValidUserSubscription);
		}

		protected Request<ChangePassword> GetChangePassword(bool isValidChangePassword = true)
		{
			return GetRequest<ChangePassword>(isValidChangePassword);
		}

		protected Request<PasswordResetDetails> GetPasswordReset(bool isValidPasswordReset = true)
		{
			return GetRequest<PasswordResetDetails>(isValidPasswordReset);
		}

		protected Request<UserName> GetUserName(bool isValidUserName = true)
		{
			return GetRequest<UserName>(isValidUserName);
		}

		protected Request<UserEmail> GetUserEmail(bool isValidUserEmail = true)
		{
			return GetRequest<UserEmail>(isValidUserEmail);
		}

		protected Request<UserConsent> GetPermission(bool isValidPermission = true)
		{
			return GetRequest<UserConsent>(isValidPermission);
		}

		protected string GetPermissionId(bool isValidPermissionId = true)
		{
			return Config(isValidPermissionId).GetStr("permissionId");
		}

		protected string GetBrandId(bool isValidBrandId = true)
		{
			return Config(isValidBrandId).GetStr("brandId");
		}

		protected Request<AccessRequestModel> GetAccess(bool isValidAccess = true)
		{
			return GetRequest<AccessRequestModel>(isValidAccess);
		}

		// user handover

		protected string GetHandoverId(bool isValid)
		{
			return Config(isValid).GetStr("handoverId");
		}

		// user subscription

		// value added services

		protected string GetServiceName(bool isValidServiceName)
		{
			return Config(isValidServiceName).GetStr("serviceName");
		}

		protected string GetProductKey(bool isValidProductKey)
		{
			return Config(isValidProductKey).GetStr("productKey");
		}

		protected string GetRequestValue(bool isValidRequestValue)
		{
			return Config(isValidRequestValue).GetStr("requestValue");
		}

		protected string GetRequestType(bool isValidRequestType)
		{
			return Config(isValidRequestType).GetStr("requestType");
		}

		protected string GetCallingApp(bool isValidCallingApp)
		{
			return Config(isValidCallingApp).GetStr("callingApp");
		}

		protected string GetServiceId(bool isValidServiceId)
		{
			return Config(isValidServiceId).GetStr("serviceId");
		}

		// spotify

		protected string GetState(bool isValidState)
		{
			return Config(isValidState).GetStr("state");
		}

		protected string GetAuthCode(bool isValidAuthCode)
		{
			return Config(isValidAuthCode).GetStr("authCode");
		}

		protected string GetAccessToken(bool isValidAccessToken)
		{
			return Config(isValidAccessToken).GetStr("accessToken");
		}

		protected string GetCode(bool isValidCode)
		{
			return Config(isValidCode).GetStr("code");
		}

		protected string GetCallbackUrl(bool isValidCallbackUrl)
		{
			return Config(isValidCallbackUrl).GetStr("callbackUrl");
		}
	}
}
