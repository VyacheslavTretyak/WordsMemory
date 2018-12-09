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
		public string ToLine()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Id.ToString());
			stringBuilder.Append(";");
			stringBuilder.Append(Word);
			stringBuilder.Append(";");
			stringBuilder.Append(Translate);
			stringBuilder.Append(";");
			stringBuilder.Append(TimeCreate.ToString());
			stringBuilder.Append(";");
			stringBuilder.Append(TimeShow.ToString());
			stringBuilder.Append(";");
			stringBuilder.Append(CountShow.ToString());
			stringBuilder.Append(";");
			return stringBuilder.ToString();
		}
	}
}
