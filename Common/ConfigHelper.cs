using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Switchr.API.Tests.Common
{
	public class ConfigHelper
	{
		private Dictionary<string, string> _dict;

		private static string folder = @"Fixtures";
		private static string basePath => TestContext.CurrentContext.TestDirectory.Replace(@"bin\Debug", "");

		private static string FixturePath(string filePath)
		{
			return Path.Combine(basePath, folder, filePath);
		}

		public static ConfigHelper Create(string fileName = null)
		{
			var helper = new ConfigHelper();
			helper.LoadDict(fileName);
			return helper;
		}

		public void LoadDict(string fileName = "valid.ini")
		{
			var filePath = FixturePath(fileName);
			_dict = TestDataStore.LoadConfig(filePath);
		}

		public string GetVal(string key)
		{
			return _dict[key];
		}

		public string GetStr(string key)
		{
			return GetVal(key);
		}

		public int GetInt(string key)
		{
			return int.Parse(GetVal(key));
		}

		public int GetTimeSecs(string key)
		{
			var val = GetVal(key);
			DateTime date = DateTime.Parse(val);
			var elapsedTicks = date.Ticks;
			return (int) new TimeSpan(elapsedTicks).TotalSeconds;
		}
	}
}
