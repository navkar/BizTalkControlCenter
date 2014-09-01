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
    public class BCCPerfCounterEntry
    {
        private string perfCategory = string.Empty;
        private string perfInstance = string.Empty;
        private string perfCounter = string.Empty;
        private int pollingInterval = 0;
        private bool isEnabled = false;
        private bool isMarkedForDelete = false;

        public BCCPerfCounterEntry()
        {
            // Default constructor;
        }

        public BCCPerfCounterEntry(string perfCategory, string perfInstance, string perfCounter)
        {
            this.perfCategory = perfCategory;
            this.perfCounter = perfCounter;
            this.perfInstance = perfInstance;
        }

        public BCCPerfCounterEntry(string perfCategory, string perfInstance, string perfCounter, int pollingInterval, bool isEnabled, bool isMarkedForDelete)
        {
            this.perfCategory = perfCategory;
            this.perfCounter = perfCounter;
            this.perfInstance = perfInstance;
            this.pollingInterval = pollingInterval;
            this.isEnabled = isEnabled;
            this.isMarkedForDelete = isMarkedForDelete;
        }

        public string PerfCategory
        {
            get { return this.perfCategory; }
            set { this.perfCategory = value; }
        }

        public string PerfCounter
        {
            get { return this.perfCounter; }
            set { this.perfCounter = value; }
        }

        public string PerfInstance
        {
            get { return this.perfInstance; }
            set { this.perfInstance = value; }
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

        /// <summary>
        /// This shall return the concatenated value of Category, Performance Counter and instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.perfCategory + "_" + this.perfCounter + "_" + this.perfInstance;
        }
    }

    public class BCCPerfCounterReportEntry : BCCPerfCounterEntry
    {
        private float performanceCounterValue = 0f;
        private DateTime reportedDate = DateTime.Now;

        public BCCPerfCounterReportEntry()
        {
            // Default constructor;
        }

        public float PerformanceCounterValue
        {
            get { return this.performanceCounterValue; }
            set { this.performanceCounterValue = value; }
        }

        public DateTime ReportedDate
        {
            get { return this.reportedDate; }
            set { this.reportedDate = value; }
        }
    }

    public class BCCPerfCounterDataAccess
    {
        public void CreatePerformanceCounterEntry(BCCPerfCounterEntry entry)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_PerfCounterList_CreateEntry]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@perfCategory", entry.PerfCategory);
                command.Parameters.Add(param);

                param = new SqlParameter("@perfInstance", entry.PerfInstance);
                command.Parameters.Add(param);

                param = new SqlParameter("@perfCounterName", entry.PerfCounter);
                command.Parameters.Add(param);

                param = new SqlParameter("@pollingInterval", 10); // 10 seconds is the new default.
                command.Parameters.Add(param);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void RemovePerformanceCounterEntry(BCCPerfCounterEntry entry)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_PerfCounterList_Delete]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@perfCategory", entry.PerfCategory);
                command.Parameters.Add(param);

                param = new SqlParameter("@perfInstance", entry.PerfInstance);
                command.Parameters.Add(param);

                param = new SqlParameter("@perfCounterName", entry.PerfCounter);
                command.Parameters.Add(param);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void PerformanceCounterEntryMarkedForDeletion(BCCPerfCounterEntry entry)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_PerfCounterList_MarkedForDelete]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@perfCategory", entry.PerfCategory);
                command.Parameters.Add(param);

                param = new SqlParameter("@perfInstance", entry.PerfInstance);
                command.Parameters.Add(param);

                param = new SqlParameter("@perfCounterName", entry.PerfCounter);
                command.Parameters.Add(param);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public List<BCCPerfCounterEntry> PerformanceCounterEntryList()
        {
            List<BCCPerfCounterEntry> list = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_PerfCounterList_List]", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                list = new List<BCCPerfCounterEntry>();

                BCCPerfCounterEntry entry = null;

                while (reader.Read() )
                {
                    // Create a new list at every instance.
                    entry = new BCCPerfCounterEntry();
                    entry.PerfCategory = (string)reader[0];
                    
                    entry.PerfCounter = (string) reader[1];

                    entry.PerfInstance = (string)reader[2];

                    entry.PollingInterval = (int) reader[3];

                    entry.IsEnabled = (bool)reader[4];

                    entry.IsMarkedForDelete = (bool)reader[5];

                    list.Add(entry);
                }
            }

            return list;
        }

        public void LogPerformanceCounterData(string categoryName, string counterName, string instanceName, float performanceCounterData)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_PerfCounterData_LogData]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@categoryName", categoryName);
                command.Parameters.Add(param);

                param = new SqlParameter("@counterName", counterName);
                command.Parameters.Add(param);

                param = new SqlParameter("@instanceName", instanceName);
                command.Parameters.Add(param);

                param = new SqlParameter("@perfCounterValue", performanceCounterData);
                command.Parameters.Add(param);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void PurgePerformanceCounterData(int daysToKeep)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_PerfCounterData_Purge]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@daysToKeep", daysToKeep);
                command.Parameters.Add(param);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        
        public List<BCCPerfCounterReportEntry> PerformanceCounterDataReport(string categoryName, string counterName, string instanceName, int rowcount)
        {
            List<BCCPerfCounterReportEntry> list = null;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("[dbo].[bcc_PerfCounterData_Report]", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@categoryName", categoryName);
                command.Parameters.Add(param);

                param = new SqlParameter("@counterName", counterName);
                command.Parameters.Add(param);

                param = new SqlParameter("@instanceName", instanceName);
                command.Parameters.Add(param);

                param = new SqlParameter("@rowcount", rowcount);
                command.Parameters.Add(param);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                list = new List<BCCPerfCounterReportEntry>();

                BCCPerfCounterReportEntry entry = null;

                while (reader.Read())
                {
                    // Create a new list at every instance.
                    entry = new BCCPerfCounterReportEntry();
                    entry.PerfCategory = categoryName;

                    entry.PerfCounter = counterName;

                    entry.PerfInstance = instanceName;

                    entry.ReportedDate = (DateTime) reader[0];

                    entry.PerformanceCounterValue = (float) reader[1];
                   
                    list.Add(entry);
                }

            }

            return list;
        }
    }
}
