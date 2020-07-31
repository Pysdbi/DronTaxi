using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Linq;
using DronTaxi.Modules.SecurityWorker;
using DronTaxi.Models;
using DronTaxi.Modules.SqlWorker;
using DronTaxi.MainAction;
using MahApps.Metro.Controls.Dialogs;

namespace DronTaxi
{
	public partial class Authorization
	{
		private MetroWindow ThisWindow;
		public Authorization()
		{
			InitializeComponent();

			ThisWindow = this;

			if (!File.Exists("config.txt"))
			{
				File.Create("config.txt");
				SetValueConfig(false, "", "");
			}
			else if (GetValueConfig() != "false") 
			{
				string[] data = GetValueConfig().Split(':');
				LoginBox.Text = data[0];
				PassBox.Password = data[1];
				RememberMe.IsChecked = true;
			}
		}

		private async void SignIn_Click(object sender, RoutedEventArgs e)
		{

			if (LoginBox.Text.Length > 0 && PassBox.Password.Length > 0) 
			{
				SqlWorker db = new SqlWorker();
			
				Users OriginUser = db.SelectAll<Users>().Where(i => i.Login == LoginBox.Text).FirstOrDefault();
				Users user = new Users() { Login = LoginBox.Text, Password = PassBox.Password };

				if (SecurityWorker.CompareUsers(user, OriginUser))
				{
					MainWindow win = new MainWindow(OriginUser);
					win.Show();
					this.Close();
				}
				else
				{
					await ThisWindow.ShowMessageAsync("Ошибка", "Неверный логин или пароль.");
					LoginBox.Text = "";
				}
			}

			if(RememberMe.IsChecked == true)
			{
				SetValueConfig(true, LoginBox.Text, PassBox.Password);
			}
			else
			{
				SetValueConfig(false, "", "");
			}
		}

		private void SignUp_Click(object sender, RoutedEventArgs e)
		{
			Registration win = new Registration();
			win.Show();
			this.Close();
		}

		private string GetValueConfig()
		{
			using (FileStream fs = File.OpenRead("config.txt"))
			{
				byte[] bytes = new byte[fs.Length];
				fs.Read(bytes, 0, bytes.Length);
				string res = Encoding.UTF8.GetString(bytes);
				if (res.Contains("(True)"))
				{
					return res.Substring(res.IndexOf(")")+1);
				}
				else
				{
					return "false";
				}
			}
		}
		private void SetValueConfig(bool IsRememberPassword, string Login, string Password)
		{
			ClearValueConfig();
			using (FileStream fs = File.OpenWrite("config.txt"))
			{
				StringBuilder builder = new StringBuilder();
				builder.Append($"({IsRememberPassword}){Login}:{Password}");
				fs.Write(Encoding.UTF8.GetBytes(builder.ToString()), 0, builder.Length);
			}
		}

		private void ClearValueConfig()
		{
			if (File.Exists("config.txt"))
			{
				File.WriteAllText("config.txt", "");
			}
		}
	}
}
