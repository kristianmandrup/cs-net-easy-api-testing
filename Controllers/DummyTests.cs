using System;
using Moq;
using NUnit.Framework;
using StructureMap;
using Switchr.Data.Interfaces;
using Switchr.API.Tests.Common;
using Switchr.Data.Managers;

namespace Switchr.API.Tests.Controllers
{
	class TestMe
	{
		private IUserManager userManager;

		public Data.DataModel.User GetHim()
		{
			return userManager.GetUser(617);
		}
	}

	interface IFoo
	{
		
	}

	class AFoo: IFoo
	{

	}

	class BFoo : IFoo
	{

	}

	class CFoo : IFoo
	{

	}

	[TestFixture]
	class DummyTests
	{
		private Container container;

		[SetUp]
		public void SetUp()
		{
			var fakeRegistry = new FakeRegistry();
			container = new Container(x => x.AddRegistry(fakeRegistry));

			var anyInt = It.IsAny<int>();
			var mockedUserManager = new Mock<IUserManager>();
			mockedUserManager.Setup(m => m.GetUser(anyInt)).Returns<Data.DataModel.User>(null);
			IUserManager userManager = mockedUserManager.Object;
			fakeRegistry.For<IUserManager>().Use(userManager);

			Console.WriteLine("Hello World");
		}

		[Test]
		public void change_default_in_an_existing_container()
		{
			var container = new Container(x => { x.For<IFoo>().Use<AFoo>(); });

			var inst = container.GetInstance<IFoo>();
			Assert.IsInstanceOf<AFoo>(inst);

			// Now, change the container configuration
			container.Configure(x => x.For<IFoo>().Use<BFoo>());

			// The default of IFoo is now different
			var inst2 = container.GetInstance<IFoo>();
			Assert.IsInstanceOf<BFoo>(inst2);

			// or use the Inject method that's just syntactical
			// sugar for replacing the default of one type at a time

			container.Inject<IFoo>(new CFoo());

			var inst3 = container.GetInstance<IFoo>();
			Assert.IsInstanceOf<CFoo>(inst3);
		}

		[Test]
		public void AlwaysTrue()
		{
			Assert.AreEqual(4, 2 + 2);
		}


	}
}
