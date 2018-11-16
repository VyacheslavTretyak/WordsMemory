using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RememberTheWords
{
	public enum AskWords
	{
		Word,
		Translete,
		Both
	}
	public class RegistryManager
	{
		public string AppName { get; set; }
		private KeyValuePair<string, string>[] defaultParams;
		public RegistryManager()
		{
			AppName = GetType().Namespace;
			defaultParams = new KeyValuePair<string, string>[]{
				new KeyValuePair<string, string>("hours", "12"),
				new KeyValuePair<string, string>("days", "7"),
				new KeyValuePair<string, string>("weeks", "4"),
				new KeyValuePair<string, string>("ask", AskWords.Both.ToString()),
			};
		}
		public void SaveSettings(Dictionary<string, string> parametrs)
		{
			RegistryKey key = Registry.CurrentUser;
			RegistryKey appKey = key.OpenSubKey(AppName, true);
			foreach (var item in parametrs)
			{
				appKey.SetValue(item.Key, item.Value);
			}
			appKey.Close();
			key.Close();
		}
		public Dictionary<string, string> GetSetings()
		{
			RegistryKey key = Registry.CurrentUser;
			RegistryKey appKey = key.OpenSubKey(AppName);
			if (appKey == null)
			{
				appKey = CreateDefaultData(key);
			}
			Dictionary<string, string> pairs = new Dictionary<string, string>();
			foreach (var item in defaultParams)
			{
				pairs[item.Key] = appKey.GetValue(item.Key).ToString();
			}
			appKey.Close();
			key.Close();
			return pairs;
		}
		private RegistryKey CreateDefaultData(RegistryKey appKey)
		{
			RegistryKey key = appKey.CreateSubKey(AppName);
			foreach (var item in defaultParams)
			{
				key.SetValue(item.Key, item.Value);
			}
			return key;
		}
	}
}
