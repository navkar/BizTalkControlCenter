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
    public class BCCAuditDataAccess
    {
        public static DataTable RetrieveAuditRecords(int startRow, int endRow)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_MessageAudit_Query]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@StartRow", startRow);
                command.Parameters.Add(param);

                param = new SqlParameter("@EndRow", endRow);
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
