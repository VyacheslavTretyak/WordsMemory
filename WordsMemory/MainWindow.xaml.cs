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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordsMemory
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			ButtonAddWord.IsEnabled = false;
			ButtonAddWord.Click += BtnAddWord_Click;
			TextBoxTranslate.TextChanged += TbTranslate_TextChanged;
			TextBoxWord.TextChanged += TbTranslate_TextChanged;
			ButtonLogin.Click += ButtonSettings_Click;
		}

		private void ButtonSettings_Click(object sender, RoutedEventArgs e)
		{
			
			WordShowing showing = new WordShowing();
			showing.ShowDialog();
		}

		private void TbTranslate_TextChanged(object sender, TextChangedEventArgs e)
		{
			ButtonAddWord.IsEnabled = !(TextBoxWord.Text.Length < 1 || TextBoxTranslate.Text.Length < 1);			
		}

		private void BtnAddWord_Click(object sender, RoutedEventArgs e)
		{
			
			DataModel.Add(TextBoxWord.Text, TextBoxTranslate.Text);
			TextBoxWord.Text = "";
			TextBoxTranslate.Text = "";
		}
		
	}
}
