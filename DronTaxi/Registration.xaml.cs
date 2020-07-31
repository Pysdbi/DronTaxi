using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DronTaxi.Models;
using DronTaxi.Modules.SecurityWorker;
using DronTaxi.Modules.SqlWorker;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace DronTaxi
{
	public partial class Registration
	{
		private MetroWindow ThisMetroWindow;
		public Registration()
		{
			InitializeComponent();
			ThisMetroWindow = this;
		}

		private async void SignUp_Click(object sender, RoutedEventArgs e)
		{
			if (CheckFieldCorrect())
			{
				bool isRegistered = false;
				Users user = GetUser();

				SqlWorker db = new SqlWorker();
					
				Users tempUser = db.SelectAll<Users>()
					.Where(i => i.Login == LoginBox.Text)
					.FirstOrDefault();
				if (tempUser is null)
				{
					db.Insert(user);

					tempUser = null;
					tempUser = db.SelectAll<Users>()
						.Where(i => i.Login == LoginBox.Text)
						.FirstOrDefault();
					if (!(tempUser is null))
					{
						db.Insert(new UserRole
						{
							RoleID = Roles.GetRoleDefault(),
							UserID = tempUser.ID,
							StartDate = DateTime.Now
						});
						db.Insert(new UserPriv
						{
							PrivID = Privs.GetRoleDefault(),
							UserID = tempUser.ID
						});
						isRegistered = true;
					}
				}
				if (!isRegistered)
				{
					await ThisMetroWindow.ShowMessageAsync("Ошибка", "Пользователь уже зарегестрирован.");
					return;
				}
				else if(isRegistered)
				{
					await ThisMetroWindow.ShowMessageAsync("Успешно", "Регистрация прошла успешно.");
					Authorization win = new Authorization();
					win.Show();
					this.Close();
				}
			}
			else 
			{
				await ThisMetroWindow.ShowMessageAsync("Ошибка", "Поля заполнены не верно.\nПроверьте Вводные данные.");
			}
		}

		private Users GetUser()
		{
			SecurityWorker security = new SecurityWorker();
			return new Users()
			{
				ID = 0,
				Login = LoginBox.Text,
				Password = security.Encrypt(PassBox.Password),
				LastName = LastNameBox.Text,
				FirstName = FirstNameBox.Text,
				MiddleName = MiddleNameBox.Text,
				DataBirth = DateTime.Now,
				Gender = "0",
				Email = EmailBox.Text,
				Phone = "0"
			};
		}

		private bool CheckFieldCorrect()
		{
			bool isCorrect = true;

			string pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
				@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
			if(!Regex.IsMatch(EmailBox.Text, pattern, RegexOptions.IgnoreCase))
			{
				EmailBox.Text = "";
				ThisMetroWindow.ShowMessageAsync("Внимание!", "Не правильный формат почты.");
				isCorrect = false;
			}

			if (PassBox.Password.Equals(ConfirmPassBox.Password))
			{
				bool isUpReg = false, isDigit = false;
				foreach(char ch in PassBox.Password)
				{
					if (char.IsDigit(ch))
					{
						isDigit = true;
					}
					if (char.IsUpper(ch))
					{
						isUpReg = true;
					}
				}
				if(PassBox.Password.Length < 6 || !isUpReg || !isDigit)
				{
					ThisMetroWindow.ShowMessageAsync("Внимание!",
						"Пароль должен содержать:\n\t" +
							"Минимум 1 символ в верхнем регистре\n\t" +
							"Минимум 1 цифру\n\t" +
							"Быть длиной не менее 6 символов\n\t");
					isCorrect = false;
				}
			}
			else
			{
				ThisMetroWindow.ShowMessageAsync("Внимание!", "Пароли не совпадают");
				PassBox.Password = "";
				ConfirmPassBox.Password = "";
				isCorrect = false;
			}

			if (LoginBox.Text.Length<=0)
			{
				isCorrect = false;
			}
			if(LastNameBox.Text.Length <= 0 && 
				FirstNameBox.Text.Length <= 0 &&
				 MiddleNameBox.Text.Length <= 0)
			{
				isCorrect = false;
			}

			return isCorrect;
		}
	}
}
