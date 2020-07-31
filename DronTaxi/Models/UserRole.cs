using System;
using System.Collections.Generic;
using System.Text;

namespace DronTaxi.Models
{
	class UserRole
	{
		public int ID { get; set; }
		public int UserID { get; set; }
		public int RoleID { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}
}
