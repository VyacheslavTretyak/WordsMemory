using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RememberTheWords
{
    public class BackUp
    {
		private static string directory = "backup";
		private static string fileName = "rememberTheWord";
		private static void TaskBackUpDB()
		{
			var list = DataModel.GetList();
			string time = DateTime.Now.ToString("yyyy_MM__dd_HH_mm_ss");
			string fullpath = $"{directory}\\{fileName}_{time}.bac";
			string spliter = ";";
			FileInfo fi = new FileInfo(directory);
			if (!fi.Exists)
			{
				System.IO.Directory.CreateDirectory(fi.FullName);
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
		}
		public static void BackUpDB()
		{
			Task.Run(() => TaskBackUpDB());
		}
    }
}
