using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace RememberTheWords
{	
	public class NextWord
	{
		public WordSet WordSet { get; set; }
		public double WaitSeconds { get; set; }
	}
	public class WordSet
	{
		public int Id { get; set; }
		public string Word { get; set; }
		public string Translate { get; set; }
		public DateTime TimeCreate { get; set; }
		public DateTime TimeShow { get; set; }		
		public int CountShow { get; set; }
	}
	public class DataModel : DbContext
	{		
		public DataModel()
			: base("name=WordsMemoryDB")
		{
		}	

		public virtual DbSet<WordSet> WordSets{ get; set; }
		public static List<WordSet> GetList()
		{
			List<WordSet> wordsList = new List<WordSet>();
			using (DataModel db = new DataModel())
			{
				wordsList = db.WordSets.ToList<WordSet>();
			}
			return wordsList;
		}
		public static void DeleteRow(string word, string translate)
		{
			using (DataModel db = new DataModel())
			{
				var row = db.WordSets.FirstOrDefault(a => a.Word == word && a.Translate == translate);
				if(row != null)
				{
					db.WordSets.Remove(row);
					db.SaveChanges();
				}
			}
		}
		public static WordSet Add(string word, string translate)
		{
			WordSet wordSet = new WordSet()
			{
				Word = word,
				Translate = translate,
				TimeCreate = DateTime.Now,
				TimeShow = DateTime.Now,
				CountShow = 0
			};
			using(DataModel db = new DataModel())
			{
				db.WordSets.Add(wordSet);
				db.SaveChanges();
			}
			return wordSet;
		}
		public static void UpdateWord(WordSet wordSet)
		{
			using (DataModel db = new DataModel())
			{
				var word = db.WordSets.FirstOrDefault(a => a.Id == wordSet.Id);
				word.CountShow++;
				word.TimeShow = DateTime.Now;
				db.SaveChanges();
			}
		}
		public static NextWord NextWord(Dictionary<string, string> settings)
		{
			using (DataModel db = new DataModel())
			{
				NextWord nextWord = new NextWord();
				DateTime now = DateTime.Now;
				int to = int.Parse(settings["hours"]);
				int from = 0;
				var sets = db.WordSets.Where(a => a.CountShow <= to);
				WordSet first = null;
				foreach(var a in sets)
				{
					if((now - a.TimeShow).TotalMinutes > 60)
					{
						first = a;
						break;
					}
				}
				var set = first;
				if(set != null)
				{
					nextWord.WordSet = set;
					nextWord.WaitSeconds= 0;
					return nextWord;
				}
				from = int.Parse(settings["hours"]);
				to = int.Parse(settings["days"]) + from;				
				sets = db.WordSets.Where(a => a.CountShow > from && a.CountShow <= to);
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
					nextWord.WordSet = set;
					nextWord.WaitSeconds = 0;
					return nextWord;
				}
				from = to;
				to = int.Parse(settings["weeks"])+ from;
				
				sets = db.WordSets.Where(a => a.CountShow > from && a.CountShow <= to);
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
					nextWord.WordSet = set;
					nextWord.WaitSeconds = 0;
					return nextWord;
				}
				from = to;
				sets = db.WordSets.Where(a => a.CountShow > from);
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
					nextWord.WordSet = set;
					nextWord.WaitSeconds = 0;
					return nextWord;
				}
				to = int.Parse(settings["hours"]);
				set = db.WordSets.Where(a => a.CountShow <= to).OrderBy(a => a.TimeShow).FirstOrDefault();
				if(set != null)
				{
					nextWord.WordSet = set;
					nextWord.WaitSeconds = 60*60-(now - set.TimeShow).TotalSeconds;
					return nextWord;
				}
				from = to;
				to = int.Parse(settings["days"]) + from;				
				set = db.WordSets.Where(a => a.CountShow > from && a.CountShow <= to).OrderBy(a => a.TimeShow).FirstOrDefault();
				if (set != null)
				{
					nextWord.WordSet = set;
					nextWord.WaitSeconds = 60*60*24-(now - set.TimeShow).TotalSeconds;
					return nextWord;
				}
				from = to;
				to = int.Parse(settings["weeks"])+from;				
				set = db.WordSets.Where(a => a.CountShow > from && a.CountShow <= to).OrderBy(a => a.TimeShow).FirstOrDefault();
				if (set != null)
				{
					nextWord.WordSet = set;
					nextWord.WaitSeconds = 60*60*24*7 - (now - set.TimeShow).TotalSeconds;
					return nextWord;
				}
				from = to;
				set = db.WordSets.Where(a => a.CountShow > from).OrderBy(a=>a.TimeShow).FirstOrDefault();
				if (set != null)
				{
					nextWord.WordSet = set;
					nextWord.WaitSeconds = 60 * 60 * 24 * 30 - (now - set.TimeShow).TotalSeconds;
					return nextWord;
				}
				return null;
			}
		}
	}
}