using System;
using System.Diagnostics;
using Moq;
using Switchr.API.Tests.Common.Moq;

namespace Switchr.API.Tests.Common
{
	public class MockMoq<T> where T : class
	{
		private Mock<T> _mockObj;
		private MoqSetup<T> moqSetup;

		public Mock<T> MockObj => _mockObj;

		private MockMoq() { }

		public static MockMoq<T> Create()
		{
			var instance = new MockMoq<T>();
			instance._mockObj = new Mock<T>();
			return instance;
		}

		public MockMoq<T> As<TV>() where TV : class
		{
			_mockObj.As<TV>();
			return this;
		}

		public T Value => (T) _mockObj.Object;

		protected object invokeDynamic(object m, object target, string name, object[] args) {
			var type = m.GetType();
			var method = type.GetMethod(name);
			return method.Invoke(target, args);
		}

		public MoqSetup<T> OnCall(string name, object[] args = null)
		{
			// TODO: throw and debug if not virtual 
			try
			{
				var _setup = _mockObj.Setup(m => invokeDynamic(m, m, name, args));
				return CreateSetup(_setup);
			}
			catch (NotSupportedException ex)
			{
				// See https://stackoverflow.com/questions/21768767/why-am-i-getting-an-exception-with-the-message-invalid-setup-on-a-non-virtual
				Debug.WriteLine($"Unable to mock non-virtual method { name } for { _mockObj.GetType() }");
				throw ex;
			}
		}

		public MoqSetup<T> OnCallStatic(string name, object[] args = null)
		{
			var _setup = _mockObj.Setup(m => invokeDynamic(m, null, name, args));
			return CreateSetup(_setup);
		}

		protected object PropValue(object m, string name)
		{
			var type = m.GetType();
			var prop = type.GetProperty(name);
			return prop.GetValue(m, null);
		}

		public MoqSetup<T> OnGetProperty(string name)
		{
			var _setup = _mockObj.Setup(m => PropValue(m, name));
			return CreateSetup(_setup);
		}

		public MockMoq<T> SetProperty(string name, object value)
		{
			return OnGetProperty(name).Returns(value);
		}

		public MockMoq<T> SetNull(string name)
		{
			return OnGetProperty(name).Returns(null);
		}

		private MoqSetup<T> CreateSetup(object setup)
		{
			moqSetup = new MoqSetup<T>(this, setup);
			return moqSetup;
		}
	}
}
