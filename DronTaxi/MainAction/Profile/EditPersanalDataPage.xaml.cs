using ControlzEx.Standard;
using DronTaxi.Models;
using DronTaxi.Modules;
using DronTaxi.Modules.SecurityWorker;
using DronTaxi.Modules.SqlWorker;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
	/// Логика взаимодействия для EditPersanalDataPage.xaml
	/// </summary>
	public partial class EditPersanalDataPage : Page
	{
		private MetroWindow ThisMetroWindow;
		private Frame frame;
		private bool isNewUserPhoto = false;

		public EditPersanalDataPage(ref Frame frame)
		{
			InitializeComponent();

			ThisMetroWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)
				as MetroWindow;

			this.frame = frame;
			FieldsDataFill();
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			if (CheckFieldCorrect())
			{
				SqlWorker db = new SqlWorker();
				db.Update(GetUser(), Session.SessionUser.ID);
				if (isNewUserPhoto)
				{
					Image img = new Image();
					img.Source = UserPhoto.ImageSource;
					db.Insert(new UserPhoto()
					{
						UserID = Session.SessionUser.ID,
						Photo = ImageSourceToBytes((BitmapImage)UserPhoto.ImageSource)
					});
				}
				Session.SessionUser = GetUser();
				frame.Content = new PersanalDataPage(ref frame);
				ThisMetroWindow.ShowMessageAsync("Успешно", "Изменения выполнены");
			}
		}

		private Users GetUser()
		{
			SecurityWorker security = new SecurityWorker();
			string tempGender = "0";
			if(MaleCheckBox.IsChecked == true || FemaleCheckBox.IsChecked == true)
			{
				if(MaleCheckBox.IsChecked == true)
				{
					tempGender = "М";
				}
				else if (FemaleCheckBox.IsChecked == true)
				{
					tempGender = "Ж";
				}
			}
			return new Users()
			{
				ID = Session.SessionUser.ID,
				Login = LoginField.Text,
				Password = security.Encrypt(PasswordField.Password),
				LastName = LastNameField.Text,
				FirstName = FirstNameField.Text,
				MiddleName = MiddleNameField.Text,
				DataBirth = DataBirthField.SelectedDate.Value,
				Gender = tempGender,
				Email = EmailField.Text,
				Phone = PhoneField.Text == "" ? "" : PhoneField.Text
			};
		}

		private byte[] ImageSourceToBytes(BitmapImage imageC)
		{
			MemoryStream memStream = new MemoryStream();
			JpegBitmapEncoder encoder = new JpegBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(imageC));
			encoder.Save(memStream);
			return memStream.ToArray();
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			frame.Content = new PersanalDataPage(ref frame);
		}

		private void FieldsDataFill()
		{
			LoginField.Text = Session.SessionUser.Login;
			LastNameField.Text = Session.SessionUser.LastName;
			FirstNameField.Text = Session.SessionUser.FirstName;
			MiddleNameField.Text = Session.SessionUser.MiddleName;
			DataBirthField.SelectedDate = Session.SessionUser.DataBirth;
			if(Session.SessionUser.Gender != "0")
			{
				switch (Session.SessionUser.Gender)
				{
					case "М":
						MaleCheckBox.IsChecked = true;
						break;
					case "Ж":
						FemaleCheckBox.IsChecked = true;
						break;
				}
			}
			EmailField.Text = Session.SessionUser.Email;
			if(Session.SessionUser.Phone != "0")
			{
				PhoneField.Text = Session.SessionUser.Phone;
			}
			PasswordField.Password = SecurityWorker.Decrypt(Session.SessionUser.Password);
			ConfirmPasswordField.Password = SecurityWorker.Decrypt(Session.SessionUser.Password);
		}

		private void UpdatePhoto_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.FileName = "";
			dlg.DefaultExt = ".png";
			dlg.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
						 "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
						 "Portable Network Graphic (*.png)|*.png";
			bool? result = dlg.ShowDialog();
			if (result == true)
			{
				UserPhoto.ImageSource = new BitmapImage(new Uri(dlg.FileName));
				isNewUserPhoto = true;
			}
		}

		private void MaleCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if (FemaleCheckBox.IsChecked == true) 
			{
				FemaleCheckBox.IsChecked = false;
			}
		}

		private void FemaleCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if (MaleCheckBox.IsChecked == true)
			{
				MaleCheckBox.IsChecked = false;
			}
		}

		private bool CheckFieldCorrect()
		{
			bool isCorrect = true;

			string pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
				@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
			if (!Regex.IsMatch(EmailField.Text, pattern, RegexOptions.IgnoreCase))
			{
				EmailField.Text = "";
				ThisMetroWindow.ShowMessageAsync("Внимание!", "Не правильный формат почты");
				isCorrect = false;
			}

			if (PhoneField.Text.Length > 0)
			{
				if (!Regex.IsMatch(PhoneField.Text, @"\d{1} \d{3}-\d{3}-\d{4}"))
				{
					PhoneField.Text = "";
					ThisMetroWindow.ShowMessageAsync("Внимание!", "Не правильный формат телефона");
					isCorrect = false;
				}
			}

			if (PasswordField.Password.Equals(ConfirmPasswordField.Password))
			{
				bool isUpReg = false, isDigit = false;
				foreach (char ch in PasswordField.Password)
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
				if (PasswordField.Password.Length < 6 || !isUpReg || !isDigit)
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
				PasswordField.Password = "";
				ConfirmPasswordField.Password = "";
				isCorrect = false;
			}

			if (LoginField.Text.Length <= 0)
			{
				ThisMetroWindow.ShowMessageAsync("Внимание!", "Поле Логин не заполнено");
				isCorrect = false;
			}
			if (LastNameField.Text.Length <= 0 &&
				FirstNameField.Text.Length <= 0 &&
				 MiddleNameField.Text.Length <= 0)
			{
				ThisMetroWindow.ShowMessageAsync("Внимание!", "Поля ФИО заполнены не правильно");
				isCorrect = false;
			}

			return isCorrect;
		}
	}
}
