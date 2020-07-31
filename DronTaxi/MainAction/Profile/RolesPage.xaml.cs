using DronTaxi.Models;
using DronTaxi.Modules;
using DronTaxi.Modules.SqlWorker;
using System;
using System.Collections;
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

namespace DronTaxi.MainAction.Profile
{
	/// <summary>
	/// Логика взаимодействия для RolesPage.xaml
	/// </summary>
	public partial class RolesPage : Page
	{
		private List<Roles> AvailableRolesList;
		private List<UserRole> UserRolesList;
		private List<RolePriv> RolePrivList;

		public RolesPage()
		{
			InitializeComponent();

			AvailableRolesListLoad();
			AvailableRolesListView.ItemsSource = AvailableRolesList;

			MoreInfoContainer.Visibility = Visibility.Hidden;
		}

		private void AvailableRolesListLoad()
		{
			AvailableRolesList = new List<Roles>();

			SqlWorker db = new SqlWorker();
			UserRolesList = db.SelectAll<UserRole>()
				.Where(i => i.UserID == Session.SessionUser.ID)
				.ToList();

			foreach (UserRole item in UserRolesList)
			{
				AvailableRolesList.Add(db.SelectAll<Roles>()
					.Where(i => i.ID == item.RoleID)
					.First());
			}
		}

		private List<Privs> AvailableFuncListLoad(int RoleId)
		{
			List<Privs> AvailableFuncList = new List<Privs>();

			SqlWorker db = new SqlWorker();
			RolePrivList = db.SelectAll<RolePriv>().ToList();

			foreach (RolePriv rolePriv in RolePrivList.Where(i => i.RoleID == RoleId))
			{
				foreach (Privs priv in db.SelectAll<Privs>().Where(i => i.ID == rolePriv.PrivID))
				{
					AvailableFuncList.Add(priv);
				}
			}
			return AvailableFuncList;
		}

		private void AvailableRolesListView_CurrentCellChanged(object sender, EventArgs e)
		{

			Roles role = (Roles)AvailableRolesListView.CurrentItem;
			if (role != null)
			{
				MoreInfoContainer.Visibility = Visibility.Visible;
				TemplateRoleName.Text = $"Роль: {role.Name}";
				UserRole userRole = UserRolesList.Where(i => i.RoleID == role.ID).First();
				RoleSysName.Content = role.SysName;
				RoleName.Content = role.Name;
				RoleStartDate.Content = userRole.StartDate.ToString("dd.MM.yyyy");
				if (userRole.EndDate != null)
				{
					RoleEndDate.Content = ((DateTime)userRole.EndDate).ToString("dd.MM.yyyy");
				}
				AvailableFuncListView.ItemsSource = AvailableFuncListLoad(role.ID);
			}
		}

		private void Border_MouseEnter(object sender, MouseEventArgs e)
		{
			((Border)sender).BorderThickness = new Thickness(1);
		}

		private void Border_MouseLeave(object sender, MouseEventArgs e)
		{
			((Border)sender).BorderThickness = new Thickness(0);
		}
	}
}

