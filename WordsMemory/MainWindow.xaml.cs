using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace RememberTheWords
{	
	public partial class MainWindow : Window
	{
		//TODO BackUP
		//TODO About
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private ContextMenu contextMenu;
		public Dictionary<string, string> settings;
		private Task tast;
		private Thread thread;
		public bool IsEdit { get; set; } = false;
		public string OldWord { get; set; }
		public string OldTranslate { get; set; }
		public MainWindow()
		{
			InitializeComponent();				
			ButtonAddWord.IsEnabled = false;
			ButtonAddWord.Click += BtnAddWord_Click;
			TextBoxTranslate.TextChanged += TbTranslate_TextChanged;
			TextBoxWord.TextChanged += TbTranslate_TextChanged;
			ButtonShow.Click += ButtonShow_Click;
			ButtonList.Click += ButtonList_Click;
			ButtonSettings.Click += ButtonSettings_Click;
			Closing += MainWindow_Closing;
			Closed += MainWindow_Closed;
			ButtonExit.Click += ButtonExit_Click;	

			Top = System.Windows.SystemParameters.WorkArea.Height - Height;
			Left = System.Windows.SystemParameters.WorkArea.Width - Width;			
			Hide();
			//load settings
			RegistryManager registryManager = new RegistryManager();
			settings = registryManager.GetSetings();
			if (settings["autoRun"].ToLower() == "true")
			{
				registryManager.AutoRunSet();
			}			
			//notifyIcon
			notifyIcon = new System.Windows.Forms.NotifyIcon();
			notifyIcon.Visible = true;
			var icon = Properties.Resources.icon1.GetHicon();
			notifyIcon.Icon = System.Drawing.Icon.FromHandle(icon);			
			notifyIcon.MouseClick += NotifyIcon_MouseClick;
			//contextMenu
			contextMenu = new ContextMenu();
			MenuItem item = new MenuItem();
			item.Click += ButtonShow_Click;
			item.Header = "NEXT WORD";
			contextMenu.Items.Add(item);

			item = new MenuItem();
			item.Click += ButtonList_Click;
			item.Header = "LIST";
			contextMenu.Items.Add(item);

			item = new MenuItem();
			item.Click += ButtonSettings_Click;
			item.Header = "SETTINGS";
			contextMenu.Items.Add(item);

			item = new MenuItem();
			item.Click += ButtonExit_Click;
			item.Header = "EXIT";
			contextMenu.Items.Add(item);
			//Start
			tast = Task.Run(() => NextWord());
		}

		private void NextWord()
		{
			thread = Thread.CurrentThread;
			while (true)
			{
				NextWord word = DataModel.NextWord(settings);
				if(word == null)
				{
					MessageBox.Show("No more words");
					return;
				}
				if(word.WaitSeconds != 0)
				{
					Thread.Sleep((int)word.WaitSeconds * 1000);
				}
				Dispatcher.Invoke(() =>
				{
					WordShow(word);
				});
			}
			
		}
		public void WordShow(NextWord word)
		{
			WordShowing wnd = new WordShowing();
			bool flag = false;
			if (settings["ask"] == AskWords.Word.ToString())
			{
				flag = true;
			}
			else if (settings["ask"] == AskWords.Both.ToString())
			{
				Random rnd = new Random();
				flag = rnd.Next(0, 2) == 1;
			}
			if (flag)
			{
				wnd.TextBlockWord.Text = word.WordSet.Word;
				wnd.TextBlockTranslate.Text = word.WordSet.Translate;
			}
			else
			{
				wnd.TextBlockWord.Text = word.WordSet.Translate;
				wnd.TextBlockTranslate.Text = word.WordSet.Word;
			}
			wnd.Top = System.Windows.SystemParameters.WorkArea.Height - wnd.Height;
			wnd.Left = System.Windows.SystemParameters.WorkArea.Width - wnd.Width;
			wnd.ShowDialog();
			DataModel.UpdateWord(word.WordSet);
		}

		private void ButtonList_Click(object sender, RoutedEventArgs e)
		{
			Hide();
			Words wnd = new Words();
			wnd.ParentWindow = this;
			wnd.Top = System.Windows.SystemParameters.WorkArea.Height - wnd.Height;
			wnd.Left = System.Windows.SystemParameters.WorkArea.Width - wnd.Width;
			wnd.ShowDialog();
			Show();
		}

		private void ButtonSettings_Click(object sender, RoutedEventArgs e)
		{
			Hide();
			Settings wnd = new Settings();
			wnd.Top = System.Windows.SystemParameters.WorkArea.Height - wnd.Height;
			wnd.Left = System.Windows.SystemParameters.WorkArea.Width - wnd.Width;
			wnd.ShowDialog(ref settings);
			Show();
		}

		private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{				
				contextMenu.IsOpen = true;
			}
			else if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				if (IsVisible)
				{
					Activate();
				}
				else
				{
					Show();
					WindowState = WindowState.Normal;
				}
			}
		}	
		private void ButtonExit_Click(object sender, RoutedEventArgs e)
		{
			App.Current.Shutdown();
		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			BackUp.BackUpDB();
			notifyIcon?.Dispose();
		}
		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();			
		}

		private void ButtonShow_Click(object sender, RoutedEventArgs e)
		{
			thread.Abort();
			NextWord word = DataModel.NextWord(settings);
			WordShow(word);
			tast = Task.Run(() => NextWord());
		}		

		private void TbTranslate_TextChanged(object sender, TextChangedEventArgs e)
		{
			ButtonAddWord.IsEnabled = !(TextBoxWord.Text.Length < 1 || TextBoxTranslate.Text.Length < 1);			
		}

		private void BtnAddWord_Click(object sender, RoutedEventArgs e)
		{
			thread.Abort();
			NextWord word = new NextWord();
			if (IsEdit)
			{
				word.WordSet = DataModel.Edit(TextBoxWord.Text, TextBoxTranslate.Text, OldWord, OldTranslate);
				word.WaitSeconds = 0;
			}
			else
			{
				word.WordSet = DataModel.Add(TextBoxWord.Text, TextBoxTranslate.Text);
				word.WaitSeconds = 0;				
			}
			WordShow(word);
			TextBoxWord.Text = "";
			TextBoxTranslate.Text = "";			
			tast = Task.Run(() => NextWord());
		}
	}
}
