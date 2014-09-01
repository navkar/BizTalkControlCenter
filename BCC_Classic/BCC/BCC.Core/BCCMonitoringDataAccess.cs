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
    public class BCCMonitoringEntry
    {
        private string artifactName = string.Empty;
        private ArtifactType artifactType;
        private int pollingInterval = 0;
        private bool isEnabled = false;
        private bool isMarkedForDelete = false;

        public BCCMonitoringEntry()
        {
            // Default constructor;
        }

        public BCCMonitoringEntry(string artifactName, ArtifactType artifactType, int pollingInterval, bool isEnabled, bool isMarkedForDelete)
        {
            this.artifactName = artifactName;
            this.artifactType = artifactType;
            this.pollingInterval = pollingInterval;
            this.isEnabled = isEnabled;
            this.isMarkedForDelete = isMarkedForDelete;
        }

        public string ArtifactName
        {
            get { return this.artifactName; }
            set { this.artifactName = value; }
        }

        public ArtifactType ArtifactType
        {
            get { return this.artifactType; }
            set { this.artifactType = value; }
        }

        public int PollingInterval
        {
            get { return this.pollingInterval; }
            set { this.pollingInterval = value; }
        }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { this.isEnabled = value; }
        }

        public bool IsMarkedForDelete
        {
            get { return this.isMarkedForDelete; }
            set { this.isMarkedForDelete = value; }
        }
    }

    public class BCCMonitoringReportEntry : BCCMonitoringEntry
    {
        private string artifactStatus = string.Empty;
        private DateTime reportedDate = DateTime.Now;

        public BCCMonitoringReportEntry()
        {
            // Default constructor;
        }

        public string ArtifactStatus
        {
            get { return this.artifactStatus; }
            set { this.artifactStatus = value; }
        }

        public DateTime ReportedDate
        {
            get { return this.reportedDate; }
            set { this.reportedDate = value; }
        }
    }


    public class BCCMonitoringDataAccess
    {
        public void CreateMonitoringEntry(ArtifactType artifactType, string artifactName)
        {
            if (artifactName != string.Empty)
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
                {
                    SqlCommand command = new SqlCommand("[dbo].[bcc_MonitoringList_CreateEntry]", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter param = new SqlParameter("@artifactType", Enum.GetName(typeof(ArtifactType), artifactType));
                    command.Parameters.Add(param);

                    param = new SqlParameter("@artifactName", artifactName);
                    command.Parameters.Add(param);

                    param = new SqlParameter("@artifactDesc", "Notify after 10 seconds.");
                    command.Parameters.Add(param);

                    param = new SqlParameter("@pollingInterval", 10);
                    command.Parameters.Add(param);

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }

        public void LogMonitoringData(ArtifactType artifactType, string artifactName, string status)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_MonitoringListData_Insert]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@artifactType", Enum.GetName(typeof(ArtifactType), artifactType));
                command.Parameters.Add(param);

                param = new SqlParameter("@artifactName", artifactName);
                command.Parameters.Add(param);

                param = new SqlParameter("@status", status);
                command.Parameters.Add(param);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void PurgeMonitoringData(int daysToKeep)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_MonitoringListData_Purge]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@daysToKeep", daysToKeep);
                command.Parameters.Add(param);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void RemoveMonitoringEntry(ArtifactType artifactType, string artifactName)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_MonitoringList_Delete]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@artifactType", Enum.GetName(typeof(ArtifactType), artifactType));
                command.Parameters.Add(param);

                param = new SqlParameter("@artifactName", artifactName);
                command.Parameters.Add(param);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void MonitoringEntryMarkedForDeletion(ArtifactType artifactType, string artifactName)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_MonitoringList_MarkedForDelete]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@artifactType", Enum.GetName(typeof(ArtifactType), artifactType));
                command.Parameters.Add(param);

                param = new SqlParameter("@artifactName", artifactName);
                command.Parameters.Add(param);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public List<BCCMonitoringEntry> MonitoringEntryList()
        {
            List<BCCMonitoringEntry> list = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_MonitoringList_List]", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                list = new List<BCCMonitoringEntry>();

                BCCMonitoringEntry entry = null;

                while (reader.Read() )
                {
                    // Create a new list at every instance.
                    entry = new BCCMonitoringEntry();
                    entry.ArtifactType = (ArtifactType) Enum.Parse(typeof(ArtifactType), (string)reader[0]);
                    
                    entry.ArtifactName = (string) reader[1];
                    
                    entry.PollingInterval = (int) reader[2];

                    entry.IsEnabled = (bool)reader[3];

                    entry.IsMarkedForDelete = (bool)reader[4];

                    list.Add(entry);
                }

            }

            return list;
        }

        public List<BCCMonitoringReportEntry> MonitoringDataReport(ArtifactType artifactType, string artifactName, DateTime startDate, DateTime endDate)
        {
            List<BCCMonitoringReportEntry> list = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_MonitoringListData_Report]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@artifactType", Enum.GetName(typeof(ArtifactType), artifactType));
                command.Parameters.Add(param);

                param = new SqlParameter("@artifactName", artifactName);
                command.Parameters.Add(param);

                param = new SqlParameter("@startDate", startDate);
                command.Parameters.Add(param);

                param = new SqlParameter("@endDate", endDate);
                command.Parameters.Add(param);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                list = new List<BCCMonitoringReportEntry>();

                BCCMonitoringReportEntry entry = null;

                while (reader.Read())
                {
                    // Create a new list at every instance.
                    entry = new BCCMonitoringReportEntry();
                    entry.ArtifactType = (ArtifactType)Enum.Parse(typeof(ArtifactType), (string)reader[0]);

                    entry.ArtifactName = (string)reader[1];

                    entry.ArtifactStatus = (string)reader[2];

                    entry.ReportedDate = (DateTime) reader[3];
                   
                    list.Add(entry);
                }

            }

            return list;
        }
    }
}
