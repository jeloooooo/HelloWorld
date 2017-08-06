using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HelloWorld.Business
{
	public class Itinerary
	{
		public DataTable GetItineraries()
		{
			SqlConnection con = new SqlConnection(Utility.GetConnectionString());
			con.Open();

			DataTable dtIti = new DataTable();

			SqlCommand cmd = new SqlCommand();
			cmd.Connection = con;
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = "SELECT name, description FROM Itinerary";

			SqlDataAdapter da = new SqlDataAdapter(cmd);
			da.Fill(dtIti);

			con.Close();

			return dtIti;
		}
	}
}