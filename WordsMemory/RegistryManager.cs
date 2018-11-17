using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
				new KeyValuePair<string, string>("autoRun", "true"),
			};
		}
		public void SaveSettings(Dictionary<string, string> parametrs)
		{
			RegistryKey curUserkey = Registry.CurrentUser;
			RegistryKey appKey = curUserkey.OpenSubKey(AppName, true);
			foreach (var item in parametrs)
			{
				appKey.SetValue(item.Key, item.Value);
			}
			appKey.Close();
			if (parametrs["autoRun"].ToLower() == "true")
			{
				AutoRunSet();
			}
			else
			{
				AutoRunUnset();
			}
			curUserkey.Close();
		}
		public void AutoRunSet()
		{
			RegistryKey curUserkey = Registry.CurrentUser;
			RegistryKey autoRunKey = curUserkey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
			var location = System.Reflection.Assembly.GetEntryAssembly().Location;
			autoRunKey.SetValue(AppName, location);
			autoRunKey.Close();
			curUserkey.Close();
		}
		public void AutoRunUnset()
		{
			RegistryKey curUserkey = Registry.CurrentUser;
			RegistryKey autoRunKey = curUserkey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
			autoRunKey.DeleteValue(AppName);
			autoRunKey.Close();
			curUserkey.Close();
		}
		public Dictionary<string, string> GetSetings()
		{
			RegistryKey curUserKey = Registry.CurrentUser;
			RegistryKey appKey = curUserKey.OpenSubKey(AppName,true);
			if (appKey == null)
			{			
				appKey = curUserKey.CreateSubKey(AppName, true);
			}
			Dictionary<string, string> pairs = new Dictionary<string, string>();
			foreach (var item in defaultParams)
			{
				var value = appKey.GetValue(item.Key);
				if(value == null)
				{
					appKey.SetValue(item.Key, item.Value);
				}
				else
				{
					pairs[item.Key] = value.ToString();
				}				
			}
			appKey.Close();
			curUserKey.Close();
			return pairs;
		}		
	}
}
