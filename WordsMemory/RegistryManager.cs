using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RememberTheWords
{
	public class RegistryManager
	{
		public void GetInfo()
		{
			var appName = GetType().Namespace;
			RegistryKey key = Registry.CurrentUser;
			var res = key.OpenSubKey(appName);

		}
	}
}
