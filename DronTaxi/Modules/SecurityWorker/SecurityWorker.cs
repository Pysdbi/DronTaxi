using DronTaxi.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Text;

namespace DronTaxi.Modules.SecurityWorker
{
	public class SecurityWorker
	{
		private const string salt = "ceb20772e0c9d240";
		private int startInd { get; set; }
		private Random rand;

		public SecurityWorker()
		{
			rand = new Random();
		}

		public string Encrypt(string data)
		{ 
			startInd = rand.Next(1, data.Length - 2);
			return SetSalt(data, startInd);
		}

		private string SetSalt(string data, int ind)
		{
			StringBuilder builder = new StringBuilder();

			int temp = data.Length - ind - 1;
			builder.Append(data.Substring(0, temp));
			builder.Append(salt);
			builder.Append(data.Substring(temp, data.Length - temp));
			return builder.ToString();
		}

		public static string Decrypt(string data)
		{
			return data.Replace(salt, "");
		}

		public static bool CompareUsers(Users user1, Users user2)
		{
			if(user1 is null || user2 is null)
			{
				return false;
			}
			if (user1.Login.Equals(user2.Login))
			{
				if (user1.Password.Equals(Decrypt(user2.Password)))
				{
					return true;
				}
			}
			return false;
		}
	}
}
