using System;
using StructureMap;
using Switchr.Data.DataModel;
using System.Linq;
using TeliaDK.Core.Cache;
using AutoMapper;
using NSubstitute;
using Switchr.Logic.Configuration;
using Switchr.Data.Interfaces;
using TeliaDK.UpgradeManager.ClassLibrary.Clients;
using Telia.Core.Clients.BPM.Models;
using System.Configuration;
using ConnectionManager;

namespace Switchr.API.Tests.Common
{
	public class FakeRegistry : Registry
	{

		private readonly MockFactory _mockFactory = MockFactory.Create();

		public T CreateMock<T>() where T : class
		{
			return _mockFactory.CreateMock<T>();
		}

		public void Use<T>(T mock = null) where T : class
		{
			mock = mock ?? CreateMock<T>();
			For<T>().Use(mock);
		}

		public void Add<T>(T value) where T : class
		{
			For<T>().Add(value);
		}

		public void AddContext(SwitchrEntities context)
		{
			For<SwitchrEntities>().Add(context);
		}

		public FakeRegistry()
		{
			var vasManager = Substitute.For<IValueAddedServiceManager>();
			var benfitManager = Substitute.For<IBenefitManager>();
			var containerSocManager = Substitute.For<IContainerSocManager>();
			var upgradeManagerClient = Substitute.For<IUpgradeManagerClient>();
			var subscriptionManager = Substitute.For<ISubscriptionManager>();
			var focusSubscriptionOfferingManager = Substitute.For<IFocusSubscriptionOfferingManager>();

			var profile = new SwitchrLogicMappingProfile(vasManager, benfitManager, focusSubscriptionOfferingManager, containerSocManager, upgradeManagerClient, subscriptionManager);
			var config = new MapperConfiguration(cfg => cfg.AddProfile(profile));
			var mapper = config.CreateMapper();

			For<IMapper>().Add(mapper);
			For<ICache>().Use<InMemoryCache>();

			var assemblies = new[] { "Switchr", "TeliaDK", "Telia" };
			Scan(scanner =>
			{
				scanner.AssembliesFromApplicationBaseDirectory(assembly =>
					assemblies.Any(name =>
					{
						if (assembly.FullName.StartsWith(name))
						{
							// Debug.WriteLine($"Assembly {assembly.FullName}");
							return assembly.FullName.StartsWith(name);
						}
						else return false;
					})
				);
				scanner.WithDefaultConventions();
			});

			For<BpmConfiguration>().Singleton().Use(() => new BpmConfiguration()
			{
				Username = ConfigurationManager.AppSettings["BPM.UserName"],
				Password = ConfigurationManager.AppSettings["BPM.Password"],
				BaseUrl = Connections.BpmHostUrl(ConnectionSource.Default)
			});

			Console.WriteLine("Done fake registry");
			//Use<ISubscriptionAccessRightsLogic>();
			//Use<BanAccessRightsLogic>();
			//Use<IUserProfileLogic>();
			//Use<IAccountLogic>();
			//Use<IPasswordLogic>();
		}
	}
}

