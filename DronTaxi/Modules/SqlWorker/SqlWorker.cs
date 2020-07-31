using ControlzEx.Standard;
using DronTaxi.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media;

namespace DronTaxi.Modules.SqlWorker
{
	public class SqlWorker
	{
		private SqlConnection conn { get; set; }

		public SqlWorker()
		{
			SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
			builder.DataSource = "localhost";
			builder.InitialCatalog = "DronTaxi";
			builder.IntegratedSecurity = true;
			conn = new SqlConnection(builder.ConnectionString);
		}


		public IEnumerable<T> SelectAll<T>(string compare = null)
		{
			string ClassName = typeof(T).ToString();
			string Table = ClassName.Substring(ClassName.LastIndexOf('.') + 1);
			List<T> list = new List<T>();

			try
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand(GetSQLcmd<T>("SELECT", Table, where: compare), conn);
				SqlDataReader reader = cmd.ExecuteReader();
				T obj = default(T);
				while (reader.Read())
				{
					obj = Activator.CreateInstance<T>();
					foreach (PropertyInfo prop in obj.GetType().GetProperties())
					{
						if (!object.Equals(reader[prop.Name], DBNull.Value))
						{
							prop.SetValue(obj, Convert.ChangeType(reader[prop.Name], prop.PropertyType));
						}
						//Debug.Write($"{prop.GetValue(obj)}\t");
					}
					list.Add(obj);
					//Debug.WriteLine("");
				}
				return list;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"SelectAll: {ex.Message}");
			}
			finally
			{
				conn.Close();
			}
			return null;
		} 

		public void Insert<T>(T Item)
		{
			string ClassName = typeof(T).ToString();
			string Table = ClassName.Substring(ClassName.LastIndexOf('.') + 1);
			try
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand(GetSQLcmd("INSERT", Table, item:Item), conn);
				cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Insert: {ex.Message}");
			}
			finally
			{
				conn.Close();
			}
		}

		public void Update<T>(T Item, int Id=-1)
		{
			string ClassName = typeof(T).ToString();
			string Table = ClassName.Substring(ClassName.LastIndexOf('.') + 1);
			try
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand(GetSQLcmd("UPDATE", Table, item: Item, Id:Id), conn);
				cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				Debug.Write($"UPDATE: {ex.Message}");
			}
			finally
			{
				conn.Close();
			}
		}


		private string GetSQLcmd<T>(string TypeCMD, string Table, T item = default, int Id = -1, string where = "")
		{
			string query = null;

			switch (TypeCMD)
			{
				case "SELECT":
					query = $"SELECT * FROM {Table};";
					break;
				case "INSERT":
					switch (Table)
					{
						case "Users":
							query = GetInsertCmd(item as Users);
							break;
						case "UserPriv":
							query = GetInsertCmd(item as UserPriv);
							break;
						case "UserRole":
							query = GetInsertCmd(item as UserRole);
							break;
						case "UserPhoto":
							query = GetInsertCmd(item as UserPhoto);
							break;
					}
					break;
				case "UPDATE":
					switch (Table)
					{
						case "Users":
							query = GetUpdateCmd(item as Users, Id);
							break;
					}
					break;
				default:
					break;
			}
			//Debug.WriteLine(query);
			return query;
		}

		private string GetInsertCmd<T>(T Item)
		{
			T obj = Activator.CreateInstance<T>();

			string table = typeof(T).ToString().Substring(typeof(T).ToString().LastIndexOf('.') + 1);

			StringBuilder builderProp = new StringBuilder();
			StringBuilder builderVal = new StringBuilder();
			foreach (PropertyInfo prop in obj.GetType().GetProperties())
			{
				if (!object.Equals(prop.GetValue(Item), null) && !prop.Name.Equals("ID"))
				{
					builderProp.Append($"[{prop.Name}], ");
					if (!(prop.GetValue(Item) is string) && !(prop.GetValue(Item) is DateTime) && !(prop.GetValue(Item) is byte[]))
					{
						builderVal.Append($"{prop.GetValue(Item)}, ");
					}
					else
					{
						if (prop.GetValue(Item) is byte[])
						{
							builderVal.Append($"'{BitConverter.ToString(prop.GetValue(Item) as Byte[]) }', ");
						}
						else
						{
							builderVal.Append($"'{prop.GetValue(Item)}', ");
						}
					}
				}
			}
			builderProp.Remove(builderProp.Length - 2, 2);
			builderVal.Remove(builderVal.Length - 2, 2);

			return $"INSERT INTO {table} ({builderProp}) VALUES ({builderVal});";
		}

		private string GetUpdateCmd<T>(T Item, int Id=-1)
		{
			T obj = Activator.CreateInstance<T>();

			string table = typeof(T).ToString().Substring(typeof(T).ToString().LastIndexOf('.') + 1);

			StringBuilder builder = new StringBuilder();

			foreach (PropertyInfo prop in obj.GetType().GetProperties())
			{
				if (!object.Equals(prop.GetValue(Item), null) && !prop.Name.Equals("ID"))
				{
					builder.Append($"[{prop.Name}]=");
					if (!(prop.GetValue(Item) is string) && !(prop.GetValue(Item) is DateTime))
					{
						builder.Append($"{prop.GetValue(Item)}, ");
					}
					else
					{
						builder.Append($"'{prop.GetValue(Item)}', ");
					}
				}
			}
			builder.Remove(builder.Length - 2, 2);
			return $"UPDATE {table} SET {builder} WHERE [ID]={Id};";
		}
	}
}
