using Moq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Westwind.Utilities;

namespace Switchr.API.Tests.Common
{
	public class DependencyMocker
	{
		public object mockTarget;

		public DependencyMocker(object mockTarget = null)
		{
			this.mockTarget = mockTarget != null ? mockTarget : CreateMocked();
		}

		public static MockMoq<object> CreateMocked()
		{
			return MockMoq<object>.Create();
		}


		public PropertyInfo GetMemberInst(object obj, string propName)
		{
			var type = obj.GetType();
			// , BindingFlags.Instance
			PropertyInfo prop = type.GetProperty(propName);
			if (prop == null)
			{
				throw new Exception($"Invalid property path {propName}");
				// See: https://github.com/RickStrahl/Expando
				//var mockObj = new Expando(null);
				//mockObj[propName] = null;
				//return mockObj.GetType().GetProperty(propName);
			}

			if (prop != null && prop.CanWrite)
			{
				var value = prop.GetValue(obj);
				if (value != null)
				{
					return prop;
				}
			}

			throw new Exception($"Invalid property path {propName}");
		}


		public void SetPropPath(string propPath, object propValue, bool overrideIfSet = true)
		{
			string[] dependencyPathList = propPath.Split('.');
			if (mockTarget == null)
			{
				throw new Exception("dependencyMocker: Missing mockTarget");
			}
			PropertyInfo prop = dependencyPathList.Aggregate(mockTarget, (object acc, string str) =>
			{
				return GetMemberInst(acc, str);
			}, pi => pi) as PropertyInfo;

			// if only public members add this flag to BindingFlags.Instance
			// BindingFlags.Public |
			if (null != prop && prop.CanWrite)
			{
				prop.SetValue(mockTarget, propValue, null);
			}
		}

		static public MockMoq<D> MockClassMethods<D, RT>(string dependencyClass, Dictionary<string, object> members) where D : class
		{
			var dependency = MockMoq<D>.Create();
			// var dependency = new Mock<D>();
			foreach (var pair in members)
			{
				var methodName = pair.Key;
				var returnValue = pair.Value;
				dependency.OnCall(methodName).ReturnsExplicit<RT>(returnValue);
			}
			return dependency;
		}

		public void MockDependencyMethodCall<D>(string dependencyPath, string methodName, object value, bool overrideIfSet = true) where D : class
		{
			var dependency = MockMoq<D>.Create();
			SetPropPath(dependencyPath, dependency, overrideIfSet);
			dependency.OnCall(methodName).Returns(value);
		}

		public void MockDependencyProperty<D>(string dependencyPath, string propName, object value, bool overrideIfSet = true) where D : class
		{
			var dependency = MockMoq<D>.Create();
			SetPropPath(dependencyPath, dependency, overrideIfSet);
			dependency.OnGetProperty(propName).Returns(value);
		}

		public void MockProperty<D>(string propName, object value, bool overrideIfSet = true) where D : class
		{
			SetPropPath(propName, value);
		}

		public void MockDependencyStaticCall<D>(string dependencyPath, string methodName, object value, bool overrideIfSet = true) where D : class
		{
			var dependency = MockMoq<D>.Create();
			SetPropPath(dependencyPath, dependency, overrideIfSet);
			dependency.OnCallStatic(methodName).Returns(value);
		}
	}
}
