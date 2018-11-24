using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace RememberTheWords
{
    public class BackUp
    {
		private static string directory = "backup";
		private static string fileName = "rememberTheWord";
		private static string format = "yyyy_MM_dd_HH_mm_ss";
		private static void TaskBackUpDB()
		{
			var list = DataModel.GetList();
			string time = DateTime.Now.ToString(format);
			string fullpath = $"{directory}\\{fileName}__{time}.bac";
			string spliter = ";";
			FileInfo fi = new FileInfo(directory);
			if (!fi.Exists)
			{
				Directory.CreateDirectory(fi.FullName);
			}
			
			using (StreamWriter streamWriter = new StreamWriter(fullpath))
			{
				foreach (var row in list)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(row.Id);
					stringBuilder.Append(spliter);
					stringBuilder.Append(row.Word);
					stringBuilder.Append(spliter);
					stringBuilder.Append(row.Translate);
					stringBuilder.Append(spliter);
					stringBuilder.Append(row.CountShow);
					stringBuilder.Append(spliter);
					stringBuilder.Append(row.TimeShow);
					stringBuilder.Append(spliter);
					stringBuilder.Append(row.TimeCreate);
					streamWriter.WriteLine(stringBuilder.ToString());
				}
			}
			DirectoryInfo info = new DirectoryInfo(fi.FullName);
			FileInfo[] files = info.GetFiles();
			while (files.Length > 3) {
				FileInfo latestFile = files[0];
				int index = latestFile.Name.IndexOf("__");
				string strTime = latestFile.Name.Substring(index + 2, 19);
				DateTime latest = DateTime.ParseExact(strTime, format, CultureInfo.InvariantCulture);
				
				foreach (FileInfo file in files)
				{
					index = file.Name.IndexOf("__");
					strTime = file.Name.Substring(index + 2, 19);
					DateTime dateTime = DateTime.ParseExact(strTime, format, CultureInfo.InvariantCulture);
					if (dateTime < latest)
					{
						latestFile = file;
						latest = dateTime;
					}					
				}
				if (File.Exists(latestFile.FullName))
				{
					File.Delete(latestFile.FullName);
				}
				files = info.GetFiles();
			}

		}
		public static void BackUpDB()
		{
			Task.Run(() => TaskBackUpDB());
		}
    }
}
