using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data;

using BCC.Core.WMI.BizTalk;
using System.Collections;

namespace BCC.Core
{
    public class BCCWebAudit
    {
        public static void PurgeWebAuditEvents(int daysToKeep)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[aspnet_WebEvent_PurgeEvents]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@daysToKeep", daysToKeep);
                command.Parameters.Add(param);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

    }
}
