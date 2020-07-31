using DronTaxi.Models;
using DronTaxi.Modules.SqlWorker;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DronTaxi.MainAction.Profile;
using DronTaxi.Modules;

namespace DronTaxi.MainAction
{
	public partial class MainWindow
	{
		private MetroWindow ThisWindow;
		private IEnumerable<string> PrivsList;

		public MainWindow(Users User)
		{
			InitializeComponent();
			ThisWindow = this;

			Session.SessionUser = User;

			PrivsListLoad();
			PrivsListViewFill();

			ShowPrivPage("Профиль");
		}

		private void ExitButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Authorization win = new Authorization();
			win.Show();
			this.Close();
		}

		private void PrivsListLoad()
		{
			SqlWorker db = new SqlWorker();
			List<Privs> AvailableFuncList = new List<Privs>();
			List<Roles> AvailableRolesList = new List<Roles>();
			
			List<RolePriv> RolePrivList;
			List<UserRole> UserRolesList = db.SelectAll<UserRole>()
				.Where(i => i.UserID == Session.SessionUser.ID)
				.ToList();

			foreach (UserRole item in UserRolesList)
			{
				AvailableRolesList.Add(db.SelectAll<Roles>()
					.Where(i => i.ID == item.RoleID)
					.First());
			}
			
			RolePrivList = db.SelectAll<RolePriv>().ToList();
			foreach (Roles role in AvailableRolesList)
			{
				foreach (RolePriv rolePriv in RolePrivList.Where(i => i.RoleID == role.ID))
				{
					foreach (Privs priv in db.SelectAll<Privs>().Where(i => i.ID == rolePriv.PrivID))
					{
						AvailableFuncList.Add(priv);
					}
				}
			}
			PrivsList = AvailableFuncList.Select(i => i.Name).Distinct();
		}

		private void PrivsListViewFill()
		{
			PrivsListViewClear();
			
			var converter = new BrushConverter();
			foreach (string priv in PrivsList)
			{
				Border border = new Border();
				border.BorderThickness = new Thickness(0, 0, 0, 2);
				border.Cursor = Cursors.Hand;
				border.Margin = new Thickness(0, 5, 0, 5);
				StackPanel grid = new StackPanel();
				grid.Orientation = Orientation.Horizontal;
				grid.Children.Add(GetImagePriv(priv));
				grid.Children.Add(new Label
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Content = priv.Replace("Управление", "Упр."),
					FontSize = 12,
					Margin = new Thickness(10, 0, 0, 0),
					FontWeight = FontWeights.Medium,
					Foreground = Brushes.White
				});
				border.Child = grid;

				border.MouseEnter += PrivsListItem_MouseEnter;
				border.MouseDown += PrivsListItem_MouseDown;
				border.MouseLeave += PrivsListItem_MouseLeave;

				PrivsListView.Children.Add(border);
			}
		}

		private void PrivsListViewClear()
		{
			PrivsListView.Children.RemoveRange(0, PrivsListView.Children.Count);
		}

		Dictionary<string, string> PrivsIcon = new Dictionary<string, string>()
		{
			{ "Профиль", "profile.png" },
			{ "Управление пользователями", "manag-user.png"},
			{ "Управление ролями", "manag-role.png" }
		};

		private Image GetImagePriv(string Name)
		{
			BitmapImage img = new BitmapImage();
			img.BeginInit();
			string ImageName = PrivsIcon.Where(i => i.Key.Equals(Name)).Select(i => i.Value).First();
			img.UriSource = new Uri($"pack://application:,,,/Resources/icon/{ImageName}",
				UriKind.RelativeOrAbsolute);
			img.EndInit();
			return new Image
			{
				Width = 15,
				Source = img,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center
			};
		}

		private void PrivsListItem_MouseEnter(object sender, MouseEventArgs e)
		{
			//Наведение
			var converter = new BrushConverter();
			((Border)sender).BorderBrush = (Brush)converter.ConvertFromString("#7FFF7F");
		}
		private void PrivsListItem_MouseLeave(object sender, MouseEventArgs e)
		{
			//default		
			var converter = new BrushConverter();
			((Border)sender).BorderBrush = (Brush)converter.ConvertFromString("#991199");
		}

		private void PrivsListItem_MouseDown(object sender, MouseButtonEventArgs e)
		{
			//default
			var converter = new BrushConverter();
			foreach (Border border in PrivsListView.Children)
			{
				((StackPanel)border.Child).Background = (Brush)converter.ConvertFromString("#7FFF7F");
				((StackPanel)border.Child).Background.Opacity = 0;
			}

			//Активный
			((StackPanel)((Border)sender).Child).Background.Opacity = 0.4;

			string CurrentPriv = ((Label)((StackPanel)((Border)sender).Child).Children[1]).Content.ToString();

			ShowPrivPage(CurrentPriv.Replace("Упр.", "Управление"));
		}

		private void ShowPrivPage(string CurrentPrivName)
		{
			switch (CurrentPrivName)
			{
				case "Профиль":
					MainFrame.Content = new Profile.Profile(this.ThisWindow);
					break;
				default:
					MainFrame.Content = null;
					break;
			}
		}
	}
}
