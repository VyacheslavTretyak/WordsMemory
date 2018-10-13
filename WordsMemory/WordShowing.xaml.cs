using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WordsMemory
{
	/// <summary>
	/// Interaction logic for WordShowing.xaml
	/// </summary>
	public partial class WordShowing : Window
	{
		public WordShowing()
		{
			InitializeComponent();
		}

		private void Flipper_IsFlippedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
		{

		}
	}
}
