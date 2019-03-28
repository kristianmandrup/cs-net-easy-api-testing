using System;
using System.Collections.Generic;
using System.IO;

namespace Switchr.API.Tests.Common
{
	static class TestDataStore
	{
		public static Dictionary<String, String> LoadConfig(string settingsfile)
		{
			var dic = new Dictionary<String, String>();

			// ReSharper disable once InvertIf
			if (File.Exists(settingsfile))
			{
				var settingdata = File.ReadAllLines(settingsfile);
				for (var i = 0; i < settingdata.Length; i++)
				{
					var setting = settingdata[i];
					var sidx = setting.IndexOf("=");
					if (sidx >= 0)
					{
						var skey = setting.Substring(0, sidx);
						var svalue = setting.Substring(sidx + 1);						
						if (!dic.ContainsKey(skey))
						{
							dic.Add(skey, svalue);
						}
					}
				}
			}
			else
			{
				throw new Exception($"TestDataStore: Config file not found {settingsfile}");
			}

			return dic;
		}
	}
}
