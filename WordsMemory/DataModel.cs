namespace WordsMemory
{
	using System;
	using System.Data.Entity;
	using System.Linq;
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