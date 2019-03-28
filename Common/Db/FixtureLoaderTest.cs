using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Switchr.Data.Interfaces;
using Switchr.Logic.Handover;
using Switchr.Logic.Services.Cds.interfaces;
using Switchr.Logic.Services.Ninja.Interfaces;
using Switchr.Logic.Subscriptions;
using Switchr.Logic.Users;

namespace Switchr.API.Tests.Common.Db
{
	[TestFixture]
	class FixtureLoaderTest
	{


		[OneTimeSetUp]
		public static void Initialize()
		{
		}

		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void DeserializeGameList()
		{
			string json = @"['Starcraft','Halo','Legend of Zelda']";
			List<string> videogames = new FixtureLoader().DeserializeList<string>(json);
			Console.WriteLine(string.Join(", ", videogames.ToArray()));
			Assert.AreEqual(videogames[0], "Starcraft");
		}

		[Test]
		public void DeserializeFixtureData()
		{
			var fixtures = new FixtureLoader().LoadJson("data.json");
			var devDeta = fixtures.GetEnv("dev").data;
			var users = devDeta.users;
			Assert.AreEqual(3, users.Count);
		}

		[Test]
		public void LoadUser()
		{
			
		}
	}
}

