using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RememberTheWords
{

	public class DataManager
	{
		private string dataFile = "data/words.txt";
		public string format = "yyyy_MM_dd_HH_mm_ss";
		public List<WordSet> WordSets { get; set; }
		private void SaveChanges()
		{
			using(StreamWriter sw = new StreamWriter($"{dataFile}.tmp"))
			{
				foreach(var word in WordSets)
				{
					sw.WriteLine(word.ToLine());
				}
			}
		}
		public List<WordSet> GetList()
		{
			List<WordSet> WordSets = new List<WordSet>();
			using (StreamReader sr = new StreamReader(dataFile))
			{
				while (!sr.EndOfStream)
				{
					string[] line = sr.ReadLine().Split(';');
					WordSet word = new WordSet()
					{
						Id = int.Parse(line[0]),
						Word = line[1],
						Translate = line[2],
						CountShow = int.Parse(line[3]),
						TimeShow = DateTime.Parse(line[4], System.Globalization.CultureInfo.InvariantCulture),
						TimeCreate = DateTime.Parse(line[5], System.Globalization.CultureInfo.InvariantCulture)
					};
					WordSets.Add(word);
				}
			}
			return WordSets;
		}
		public void CountReset(string word, string translate)
		{ 
			var row = WordSets.FirstOrDefault(a => a.Word == word && a.Translate == translate);
			if (row != null)
			{
				row.CountShow = 0;
				SaveChanges();
			}
			
		}
		public WordSet GetWord(string word, string translate)
		{
			var wordSet = WordSets.FirstOrDefault(a => a.Word == word && a.Translate == translate);
			if (wordSet != null)
			{
				wordSet.WaitSeconds = 0;
			}
			return wordSet;
		}
		public  void DeleteRow(string word, string translate)
		{
			var row = WordSets.FirstOrDefault(a => a.Word == word && a.Translate == translate);
			if (row != null)
			{
				WordSets.Remove(row);
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
			WordSets.Add(wordSet);
			SaveChanges();
			return wordSet;
		}
		public WordSet Edit(string word, string translate, string oldWord, string oldTranslate)
		{
			WordSet wordSet = WordSets.FirstOrDefault(a => a.Word == oldWord && a.Translate == oldTranslate);
			if (wordSet != null)
			{
				wordSet.Word = word;
				wordSet.Translate = translate;
				SaveChanges();			
			}
			return wordSet;
		}

		public void UpdateWord(WordSet wordSet)
		{
			var word = WordSets.FirstOrDefault(a => a.Id == wordSet.Id);
			word.CountShow++;
			word.TimeShow = DateTime.Now;
			SaveChanges();			
		}
		public WordSet NextWord(Dictionary<string, string> settings)
		{			
			WordSet nextWord = new WordSet();
			DateTime now = DateTime.Now;
			int to = int.Parse(settings["hours"]);
				int from = 0;
				var sets = WordSets.Where(a => a.CountShow <= to);
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
				sets = WordSets.Where(a => a.CountShow > from && a.CountShow <= to);
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

				sets = WordSets.Where(a => a.CountShow > from && a.CountShow <= to);
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
				sets = WordSets.Where(a => a.CountShow > from);
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
				set = WordSets.Where(a => a.CountShow <= to).OrderBy(a => a.TimeShow).FirstOrDefault();
				if (set != null)
				{
					nextWord = set;
					nextWord.WaitSeconds = 60 * 60 - (now - set.TimeShow).TotalSeconds;
					return nextWord;
				}
				from = to;
				to = int.Parse(settings["days"]) + from;
				set = WordSets.Where(a => a.CountShow > from && a.CountShow <= to).OrderBy(a => a.TimeShow).FirstOrDefault();
				if (set != null)
				{
					nextWord = set;
					nextWord.WaitSeconds = 60 * 60 * 24 - (now - set.TimeShow).TotalSeconds;
					return nextWord;
				}
				from = to;
				to = int.Parse(settings["weeks"]) + from;
				set = WordSets.Where(a => a.CountShow > from && a.CountShow <= to).OrderBy(a => a.TimeShow).FirstOrDefault();
				if (set != null)
				{
					nextWord = set;
					nextWord.WaitSeconds = 60 * 60 * 24 * 7 - (now - set.TimeShow).TotalSeconds;
					return nextWord;
				}
				from = to;
				set = WordSets.Where(a => a.CountShow > from).OrderBy(a => a.TimeShow).FirstOrDefault();
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
}

