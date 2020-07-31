using DronTaxi.Modules.SqlWorker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DronTaxi.Models
{
	class Roles
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string SysName { get; set; }

		public static int GetRoleDefault()
		{
			SqlWorker db = new SqlWorker();
			return db.SelectAll<Roles>()
				.Where(i => i.SysName == "user")
				.FirstOrDefault().ID;
		}
	}
}
