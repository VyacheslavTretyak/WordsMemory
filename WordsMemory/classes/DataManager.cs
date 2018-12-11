using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RememberTheWords
{

	public class DataManager
	{
		private int maxCountSaveFiles = 10;
		private string directory = "data";
		private string fileName = "words";
		private string formatInFile = "yyyy_MM_dd_HH_mm_ss";
		private string spliter = WordSet.spliter;
		private string fullpath;
		private List<WordSet> words;
		private static DataManager instance = null;
		public static DataManager GetInstance()
		{
			if(instance == null)
			{
				instance = new DataManager();
			}
			return instance;
		}
		private DataManager()
		{
			fullpath = $"{directory}\\{fileName}.wrd";
			LoadList();
		}
		public List<WordSet> GetWordsList()
		{
			return words;
		}
		public void RollBack()
		{
			System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
			DirectoryInfo info = new DirectoryInfo(directory);
			fileDialog.InitialDirectory = info.FullName;
			if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				RollBackTask(fileDialog.FileName);
			}
		}
		private void RollBackTask(string fileName)
		{
			using (StreamReader streamReader = new StreamReader(fileName))
			{
				words.Clear();
				while (!streamReader.EndOfStream)
				{
					string[] data = streamReader.ReadLine().Split(spliter[0]);					
					WordSet wordSet = new WordSet();
					wordSet.Word = data[0];
					wordSet.Translate = data[1];
					wordSet.CountShow = int.Parse(data[2]);
					wordSet.TimeShow = DateTime.Parse(data[3]);
					wordSet.TimeCreate = DateTime.Parse(data[4]);					
					words.Add(wordSet);
				}
				SaveChanges();
			}
		}
		public void SaveChanges()
		{
			Task.Run(() => SaveChangesTask());
		}
		private void SaveChangesTask()
		{			
			string time = DateTime.Now.ToString(formatInFile);
			string fullpath = $"{directory}\\{fileName}__{time}.wrd";
			FileInfo fi = new FileInfo(directory);			
			using (StreamWriter streamWriter = new StreamWriter(fullpath))
			{
				foreach (var row in words)
				{					
					streamWriter.WriteLine(row.ToLine());
				}
			}
			DirectoryInfo info = new DirectoryInfo(fi.FullName);
			FileInfo[] files = info.GetFiles();
			while (files.Length > maxCountSaveFiles)
			{
				FileInfo latestFile = files[0];
				int index = latestFile.Name.IndexOf("__");
				string strTime = latestFile.Name.Substring(index + 2, 19);
				DateTime latest = DateTime.ParseExact(strTime, formatInFile, CultureInfo.InvariantCulture);

				foreach (FileInfo file in files)
				{
					index = file.Name.IndexOf("__");
					strTime = file.Name.Substring(index + 2, 19);
					DateTime dateTime = DateTime.ParseExact(strTime, formatInFile, CultureInfo.InvariantCulture);
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
		private void LoadList()
		{
			Task.Run(() =>
			{
				words = new List<WordSet>();
				FileInfo fi = new FileInfo(directory);
				if (!fi.Exists)
				{
					Directory.CreateDirectory(fi.FullName);
				}
				DirectoryInfo info = new DirectoryInfo(fi.FullName);
				FileInfo[] files = info.GetFiles();
				if(files.Length == 0)
				{
					throw new Exception("File not found!");
				}
				FileInfo newestFile = files[0];
				int index = newestFile.Name.IndexOf("__");
				string strTime = newestFile.Name.Substring(index + 2, 19);
				DateTime newest = DateTime.ParseExact(strTime, formatInFile, CultureInfo.InvariantCulture);
				foreach (FileInfo file in files)
				{
					index = file.Name.IndexOf("__");
					strTime = file.Name.Substring(index + 2, 19);
					DateTime dateTime = DateTime.ParseExact(strTime, formatInFile, CultureInfo.InvariantCulture);
					if (dateTime > newest)
					{
						newestFile = file;
						newest = dateTime;
					}
				}						
				using (StreamReader sr = new StreamReader(newestFile.FullName))
				{
					while (!sr.EndOfStream)
					{
						string[] line = sr.ReadLine().Split(spliter.ToCharArray());
						WordSet word = new WordSet()
						{							
							Word = line[0],
							Translate = line[1],
							CountShow = int.Parse(line[2]),
							TimeShow = DateTime.ParseExact(line[3], WordSet.formatInWord, System.Globalization.CultureInfo.InvariantCulture),
							TimeCreate = DateTime.ParseExact(line[4], WordSet.formatInWord, System.Globalization.CultureInfo.InvariantCulture),
							
						};
						words.Add(word);
					}
				}
			});
		}
		public void CountReset(string word, string translate)
		{
			var row = words.FirstOrDefault(a => a.Word == word && a.Translate == translate);
			if (row != null)
			{
				row.CountShow = 0;
				SaveChanges();
			}

		}
		public WordSet GetWord(string word, string translate)
		{
			var wordSet = words.FirstOrDefault(a => a.Word == word && a.Translate == translate);
			if (wordSet != null)
			{
				wordSet.WaitSeconds = 0;
			}
			return wordSet;
		}
		public void DeleteRow(string word, string translate)
		{
			var row = words.FirstOrDefault(a => a.Word == word && a.Translate == translate);
			if (row != null)
			{
				words.Remove(row);
				SaveChanges();
			}

		}
		public WordSet Add(string word, string translate)
		{
			WordSet wordSet = new WordSet()
			{
				Word = word,
				Translate = translate,
				TimeCreate = DateTime.Now,
				TimeShow = DateTime.Now,
				CountShow = 0
			};
			words.Add(wordSet);
			SaveChanges();
			return wordSet;
		}
		public WordSet Edit(string word, string translate, string oldWord, string oldTranslate)
		{
			WordSet wordSet = words.FirstOrDefault(a => a.Word == oldWord && a.Translate == oldTranslate);
			if (wordSet != null)
			{
				wordSet.Word = word;
				wordSet.Translate = translate;
			}
			return wordSet;
		}
		public void UpdateWord(WordSet wordSet)
		{
			var word = words.FirstOrDefault(a => a.Word == wordSet.Word);
			word.CountShow++;
			word.TimeShow = DateTime.Now;
			SaveChanges();
		}
		public WordSet NextWord(Dictionary<string, string> settings)
		{
			WordSet nextWord = null;
			DateTime now = DateTime.Now;
			int to = int.Parse(settings["hours"]);
			int from = 0;
			var sets = words.Where(a => a.CountShow <= to);
			WordSet first = null;
			foreach (var a in sets)
			{
				if ((now - a.TimeShow).TotalMinutes > 60)
				{
					first = a;
					break;
				}
			}
			var set = first;
			if (set != null)
			{
				nextWord = set;
				nextWord.WaitSeconds = 0;
				return nextWord;
			}
			from = int.Parse(settings["hours"]);
			to = int.Parse(settings["days"]) + from;
			sets = words.Where(a => a.CountShow > from && a.CountShow <= to);
			first = null;
			foreach (var a in sets)
			{
				if ((now - a.TimeShow).TotalHours > 24)
				{
					first = a;
					break;
				}
			}
			set = first;
			if (set != null)
			{
				nextWord = set;
				nextWord.WaitSeconds = 0;
				return nextWord;
			}
			from = to;
			to = int.Parse(settings["weeks"]) + from;

			sets = words.Where(a => a.CountShow > from && a.CountShow <= to);
			first = null;
			foreach (var a in sets)
			{
				if ((now - a.TimeShow).TotalDays > 7)
				{
					first = a;
					break;
				}
			}
			set = first;
			if (set != null)
			{
				nextWord = set;
				nextWord.WaitSeconds = 0;
				return nextWord;
			}
			from = to;
			sets = words.Where(a => a.CountShow > from);
			first = null;
			foreach (var a in sets)
			{
				if ((now - a.TimeShow).TotalDays > 30)
				{
					first = a;
					break;
				}
			}
			set = first;
			if (set != null)
			{
				nextWord = set;
				nextWord.WaitSeconds = 0;
				return nextWord;
			}
			to = int.Parse(settings["hours"]);
			set = words.Where(a => a.CountShow <= to).OrderBy(a => a.TimeShow).FirstOrDefault();
			if (set != null)
			{
				nextWord = set;
				nextWord.WaitSeconds = 60 * 60 - (now - set.TimeShow).TotalSeconds;
				return nextWord;
			}
			from = to;
			to = int.Parse(settings["days"]) + from;
			set = words.Where(a => a.CountShow > from && a.CountShow <= to).OrderBy(a => a.TimeShow).FirstOrDefault();
			if (set != null)
			{
				nextWord = set;
				nextWord.WaitSeconds = 60 * 60 * 24 - (now - set.TimeShow).TotalSeconds;
				return nextWord;
			}
			from = to;
			to = int.Parse(settings["weeks"]) + from;
			set = words.Where(a => a.CountShow > from && a.CountShow <= to).OrderBy(a => a.TimeShow).FirstOrDefault();
			if (set != null)
			{
				nextWord = set;
				nextWord.WaitSeconds = 60 * 60 * 24 * 7 - (now - set.TimeShow).TotalSeconds;
				return nextWord;
			}
			from = to;
			set = words.Where(a => a.CountShow > from).OrderBy(a => a.TimeShow).FirstOrDefault();
			if (set != null)
			{
				nextWord = set;
				nextWord.WaitSeconds = 60 * 60 * 24 * 30 - (now - set.TimeShow).TotalSeconds;
				return nextWord;
			}
			return null;
		}
	}
}


