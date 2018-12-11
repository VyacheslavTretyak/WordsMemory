using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public double WaitSeconds { get; set; }
		public static string spliter = ";";
		public static string formatInWord = "dd.MM.yyyy HH:mm:ss";
		public string ToLine()
		{
			StringBuilder stringBuilder = new StringBuilder();			
			stringBuilder.Append(Word);
			stringBuilder.Append(spliter);
			stringBuilder.Append(Translate);
			stringBuilder.Append(spliter);		
			stringBuilder.Append(CountShow.ToString());
			stringBuilder.Append(spliter);
			stringBuilder.Append(TimeShow.ToString(formatInWord));
			stringBuilder.Append(spliter);
			stringBuilder.Append(TimeCreate.ToString(formatInWord));
			return stringBuilder.ToString();
		}
	}
}
