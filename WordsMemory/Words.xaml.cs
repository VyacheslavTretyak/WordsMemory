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

namespace RememberTheWords
{
	public partial class Words : Window
	{
		public List<WordSet> wordsList;
		public Words()
		{
			InitializeComponent();			
			Task.Run(() => GetList());
			ContextMenu contextMenu = new ContextMenu();
			MenuItem item = new MenuItem();
			item.Click += Item_Click;
			item.Header = "delete";
			contextMenu.Items.Add(item);
			DataGridWords.ContextMenu = contextMenu;
		}

		private void Item_Click(object sender, RoutedEventArgs e)
		{
			var item = DataGridWords.SelectedItem;
			//TODO delete item
		}

		private void GetList()
		{
			var list = DataModel.GetList();
			Dispatcher.Invoke(() =>				
			{
				DataGridWords.ItemsSource = list.Select(a => new { a.Word, a.Translate, a.CountShow });
			});				
		}
	}
}
