using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data;

namespace BCC.Core
{
    public class BCCTaskDataAccess
    {
        public static DataTable RetrieveAllUsers(string applicationName, int pageIndex, int pageSize)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[aspnet_Membership_GetAllUsers]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@ApplicationName", applicationName);
                command.Parameters.Add(param);

                param = new SqlParameter("@PageIndex", pageIndex);
                command.Parameters.Add(param);

                param = new SqlParameter("@PageSize", pageSize);
                command.Parameters.Add(param);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                dt = new DataTable();
                dt.Load(reader);
            }

            return dt;
        }
    }
}
