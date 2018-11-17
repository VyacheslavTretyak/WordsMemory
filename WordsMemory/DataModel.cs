using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RememberTheWords
{	
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
		public static void Add(string word, string translate)
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
		}
	}
}