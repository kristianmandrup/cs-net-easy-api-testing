using FakeItEasy;
using Moq;

namespace Switchr.API.Tests.Common
{
	public class MockFactory
	{
		static public MockFactory Create()
		{
			return new MockFactory();
		}

		public T CreateMock<T>() where T : class
		{
			return A.Fake<T>();
		}

		//public T CreateMock<T>() where T : class
		//{
		//	return new Mock<T>() as T;
		//}
	}
}
