using DronTaxi.Models;
using DronTaxi.Modules;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DronTaxi.MainAction.Profile
{
	/// <summary>
	/// Логика взаимодействия для Profile.xaml
	/// </summary>
	public partial class Profile : Page
	{
		private MetroWindow MainWindow;

		public Profile(MetroWindow window)
		{
			InitializeComponent();

			MainWindow = window;

			var converter = new BrushConverter();
			((Grid)MenuBar.Children[0]).Background = (Brush)converter.ConvertFromString("#991199");
			ShowItemMenuPage("Личные данные");
		}

		private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var converter = new BrushConverter();
			foreach (Grid item in MenuBar.Children)
			{
				item.Background = (Brush)converter.ConvertFromString("#b812b8");
			}
			((Grid)sender).Background = (Brush)converter.ConvertFromString("#991199");

			string CurrentItemMenu = ((Label)((Grid)sender).Children[0]).Content.ToString();
			ShowItemMenuPage(CurrentItemMenu);
		}

		private void ShowItemMenuPage(string Name)
		{
			switch (Name)
			{
				case "Личные данные":
					SubFrame.Content = new PersanalDataPage(ref SubFrame);
					break;
				case "Роли":
					SubFrame.Content = new RolesPage();
					break;
			}
		}
	}
}
