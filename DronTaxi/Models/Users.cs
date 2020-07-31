using System;
using System.Collections.Generic;
using System.Text;

namespace DronTaxi.Models
{
	public class Users
	{
		public int ID { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		public string LastName { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public DateTime DataBirth { get; set; }
		public string Gender { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
	}
}
