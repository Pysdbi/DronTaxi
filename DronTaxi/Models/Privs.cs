using DronTaxi.Modules.SqlWorker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DronTaxi.Models
{
	class Privs
	{
		public int ID{ get; set; }
		public string Name { get; set; }
		public string Comment { get; set; }

		public static int GetRoleDefault()
		{
			SqlWorker db = new SqlWorker();
			return db.SelectAll<Privs>()
				.Where(i => i.Name == "Профиль")
				.FirstOrDefault().ID;
		}
	}
}
