using DronTaxi.Models;
using DronTaxi.Modules;
using DronTaxi.Modules.SecurityWorker;
using DronTaxi.Modules.SqlWorker;
using System;
using System.Collections.Generic;
using System.Linq;
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
	/// Логика взаимодействия для PersanalDataPage.xaml
	/// </summary>
	public partial class PersanalDataPage : Page
	{
		private Frame frame;

		public PersanalDataPage(ref Frame frame)
		{
			InitializeComponent();

			this.frame = frame;
			UserSelfLoad();
			
			FieldsDataFill();

			var converter = new BrushConverter();
			PasswordField.Background = (Brush)converter.ConvertFromString("#991199");
		}

		private void UserSelfLoad()
		{
			SqlWorker db = new SqlWorker();
			Session.SessionUser = db.SelectAll<Users>()
				.Where(i => i.Login == Session.SessionUser.Login)
				.FirstOrDefault();
		}

		private void FieldsDataFill()
		{
			LoginField.Content = Session.SessionUser.Login;
			LastNameField.Content = Session.SessionUser.LastName;
			FirstNameField.Content = Session.SessionUser.FirstName;
			MiddleNameField.Content = Session.SessionUser.MiddleName;
			DataBirthField.Content = Session.SessionUser.DataBirth.ToString("dd.MM.yyyy");
			if (Session.SessionUser.Gender != "0")
			{
				GenderField.Content = Session.SessionUser.Gender;
			}
			EmailField.Content = Session.SessionUser.Email;
			if (Session.SessionUser.Phone != "0")
			{
				PhoneField.Content = Session.SessionUser.Phone;
			}
			PasswordField.Password = SecurityWorker.Decrypt(Session.SessionUser.Password);
		}

		private void EditBtn_Click(object sender, RoutedEventArgs e)
		{
			frame.Content = new EditPersanalDataPage(ref frame);
		}
	}
}
