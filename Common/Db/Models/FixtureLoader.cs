using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using NUnit.Framework;
using Switchr.API.Tests.Common.Db.Models;
using Env = Switchr.API.Tests.Common.Db.Models.Env;

namespace Switchr.API.Tests.Common
{
	public class FixtureLoader
	{
		private Envs envs;
		private static string folder = @"Fixtures";
		private static string basePath => TestContext.CurrentContext.TestDirectory.Replace(@"bin\Debug", "");

		private static string FixturePath(string filePath)
		{
			return Path.Combine(basePath, folder, filePath);
		}


		public FixtureLoader LoadJson(string filePath)
		{
			var fullFilePath = FixturePath(filePath);
			using (StreamReader r = new StreamReader(fullFilePath))
			{
				var json = r.ReadToEnd();
				envs = Deserialize<Envs>(json);
			}

			return this;
		}

		public Env GetEnv(string name)
		{
			return envs.environments.Find(env => env.Name == name);
		}		

		public T Deserialize<T>(string json) where T: class
		{
			return JsonConvert.DeserializeObject<T>(json);
		}
		
		public List<T> DeserializeList<T>(string jsonList)
		{
			return Deserialize<List<T>>(jsonList);
		}
	}
}
