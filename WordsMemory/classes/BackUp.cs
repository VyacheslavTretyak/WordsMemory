//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.IO;
//using System.Globalization;

//namespace RememberTheWords
//{
//	public class BackUp
//	{
//		private string directory = "backup";
//		private string fileName = "rememberTheWord";
//		private string format = "yyyy_MM_dd_HH_mm_ss";
//		private string spliter = ";";
//		private DataManager dataManager;
//		public BackUp(DataManager dataManager)
//		{
//			this.dataManager = dataManager;
//		}
//		private void TaskBackUpDB()
//		{			
//			var list = dataManager.GetList();
//			string time = DateTime.Now.ToString(format);
//			string fullpath = $"{directory}\\{fileName}__{time}.bac";
//			FileInfo fi = new FileInfo(directory);
//			if (!fi.Exists)
//			{
//				Directory.CreateDirectory(fi.FullName);
//			}

//			using (StreamWriter streamWriter = new StreamWriter(fullpath))
//			{
//				foreach (var row in list)
//				{
//					StringBuilder stringBuilder = new StringBuilder();
//					stringBuilder.Append(row.Id);
//					stringBuilder.Append(spliter);
//					stringBuilder.Append(row.Word);
//					stringBuilder.Append(spliter);
//					stringBuilder.Append(row.Translate);
//					stringBuilder.Append(spliter);
//					stringBuilder.Append(row.CountShow);
//					stringBuilder.Append(spliter);
//					stringBuilder.Append(row.TimeShow);
//					stringBuilder.Append(spliter);
//					stringBuilder.Append(row.TimeCreate);
//					streamWriter.WriteLine(stringBuilder.ToString());
//				}
//			}
//			DirectoryInfo info = new DirectoryInfo(fi.FullName);
//			FileInfo[] files = info.GetFiles();
//			while (files.Length > 3)
//			{
//				FileInfo latestFile = files[0];
//				int index = latestFile.Name.IndexOf("__");
//				string strTime = latestFile.Name.Substring(index + 2, 19);
//				DateTime latest = DateTime.ParseExact(strTime, format, CultureInfo.InvariantCulture);

//				foreach (FileInfo file in files)
//				{
//					index = file.Name.IndexOf("__");
//					strTime = file.Name.Substring(index + 2, 19);
//					DateTime dateTime = DateTime.ParseExact(strTime, format, CultureInfo.InvariantCulture);
//					if (dateTime < latest)
//					{
//						latestFile = file;
//						latest = dateTime;
//					}
//				}
//				if (File.Exists(latestFile.FullName))
//				{
//					File.Delete(latestFile.FullName);
//				}
//				files = info.GetFiles();
//			}

//		}
//		public void BackUpDB()
//		{
//			Task.Run(() => TaskBackUpDB());
//		}
//		public void RollBack()
//		{
//			System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
//			DirectoryInfo info = new DirectoryInfo(directory);
//			fileDialog.InitialDirectory = info.FullName;
//			if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
//			{
//				Task.Run(() => TaskRollBackDB(fileDialog.FileName));
//			}
//		}

//		private void TaskRollBackDB(string path)
//		{
//			using (StreamReader streamReader = new StreamReader(path))
//			{
				
//					int count = dataManager.GetList().Count();
//					while (count > 0)
//					{
//						var row = db.WordSets.FirstOrDefault();
//						if (row != null)
//						{
//							db.WordSets.Remove(row);
//							db.SaveChanges();
//						}
//						count = db.WordSets.Count();
//					}

//					while (!streamReader.EndOfStream)
//					{
//						string[] data = streamReader.ReadLine().Split(spliter[0]);

//						db.WordSets.ToList().Clear();
//						WordSet wordSet = new WordSet();
//						wordSet.Word = data[1];
//						wordSet.Translate = data[2];
//						wordSet.CountShow = int.Parse(data[3]);
//						wordSet.TimeShow = DateTime.Parse(data[4]);
//						wordSet.TimeCreate = DateTime.Parse(data[5]);

//						db.WordSets.Add(wordSet);
//					}
//					db.SaveChanges();
//				}
//			}
//		}
//	}
//}
