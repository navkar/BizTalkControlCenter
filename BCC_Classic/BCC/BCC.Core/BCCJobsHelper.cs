using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;


namespace BCC.Core
{
    public class BCCJobsHelper
    {
        private string connectionString = string.Empty;
        // "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=msdb;Data Source=" + serverName;

        /// <summary>
        /// specify a connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        public BCCJobsHelper(string connectionString)
        {
            this.connectionString = connectionString;
            System.Diagnostics.Debug.Write("Connection String:" + connectionString, "BCCJobsHelper");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataTable EnumerateAllSQLJobs(StringCollection jobCollection)
        {
            DataTable dt = new DataTable();
            SqlConnection sqlConnection = null;

            try
            {
                // define the table's schema
                dt.Columns.Add(new DataColumn("JobName", typeof(string)));
                dt.Columns.Add(new DataColumn("JobExecutionStatus", typeof(string)));
                dt.Columns.Add(new DataColumn("IsJobEnabled", typeof(string)));
                dt.Columns.Add(new DataColumn("LastRunDate", typeof(string)));
                dt.Columns.Add(new DataColumn("NextRunDate", typeof(string)));
                dt.Columns.Add(new DataColumn("JobServer", typeof(string)));
                dt.Columns.Add(new DataColumn("JobResult", typeof(string)));
                dt.Columns.Add(new DataColumn("JobServerConnectionString", typeof(string)));

                sqlConnection = new SqlConnection(this.connectionString);
                ServerConnection connection = new ServerConnection(sqlConnection);

                Server server = new Server(connection);
                JobServer js = server.JobServer;

                DataRow dr = null;

                if (jobCollection != null)
                {
                    foreach (Job job in js.Jobs)
                    {
                        if (jobCollection.Contains(job.Name))
                        {
                            dr = dt.NewRow();
                            dr["JobName"] = job.Name.ToString();
                            dr["JobExecutionStatus"] = job.CurrentRunStatus.ToString();
                            dr["IsJobEnabled"] = job.IsEnabled;
                            dr["LastRunDate"] = job.LastRunDate.ToString();
                            dr["NextRunDate"] = job.NextRunDate.ToString();
                            dr["JobServer"] = job.OriginatingServer;
                            dr["JobResult"] = job.LastRunOutcome.ToString();
                            dr["JobServerConnectionString"] = this.connectionString;
                            dt.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                {
                    sqlConnection.Close();
                }
            }

            return dt;
        }

        ///// <summary>
        ///// Toggle Job - Start/Stop
        ///// </summary>
        ///// <param name="jobName"></param>
        ///// <param name="flag"></param>
        //public void StartJob(string jobName, bool flag)
        //{
        //    SqlConnection sqlConnection = null;

        //    try
        //    {
        //        sqlConnection = new SqlConnection(this.connectionString);
        //        ServerConnection connection = new ServerConnection(sqlConnection);
        //        Server server = new Server(connection);

        //        JobServer js = server.JobServer;

        //        if (js.Jobs.Count > 0)
        //        {
        //            Job job = js.Jobs[jobName];

        //            if (flag && job.IsEnabled)
        //            {
        //                System.Diagnostics.Debug.Write("job " + jobName + " started.", "BCCJobsHelper");
        //                job.Start();
        //            }
        //            else if (job.IsEnabled && !flag)
        //            {
        //                System.Diagnostics.Debug.Write("job " + jobName + " stopped.", "BCCJobsHelper");
        //                job.Stop();
        //            }

        //            job.Alter();
        //            job.Refresh();
        //        }
        //        else
        //        {
        //            throw new Exception("Unable to find job(s), check your connection string.");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //    finally
        //    {
        //        if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
        //        {
        //            sqlConnection.Close();
        //        }
        //    }
        //}


        /// <summary>
        /// Use this to start a Job()
        /// </summary>
        public void EnableJob(string jobName, bool flag)
        {
            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection = new SqlConnection(this.connectionString);
                ServerConnection connection = new ServerConnection(sqlConnection);

                Server server = new Server(connection);
                JobServer js = server.JobServer;

                Job job = js.Jobs[jobName];

                if (!job.IsEnabled && flag)
                {
                    System.Diagnostics.Debug.Write("job " + jobName + " enabled.", "BCCJobsHelper");
                    job.IsEnabled = true;
                }
                else if (job.IsEnabled && !flag)
                {
                    System.Diagnostics.Debug.Write("job " + jobName + " disabled.", "BCCJobsHelper");
                    job.IsEnabled = false;
                }

                job.Alter();
                job.Refresh();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                {
                    sqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public bool StartJob(string jobName, bool flag)
        {
            int status = 0;
            SqlCommand dbCmd = null;

            using (SqlConnection dbConn = new SqlConnection(connectionString))
            {
                if (flag)
                {
                    dbCmd = new SqlCommand("exec msdb.dbo.sp_start_job @job_name = N'" + jobName + "' ;", dbConn);
                }
                else
                {
                    dbCmd = new SqlCommand("exec msdb.dbo.sp_stop_job @job_name = N'" + jobName + "' ;", dbConn);
                }

                dbConn.Open();

                SqlDataReader dr = dbCmd.ExecuteReader();

                if (dr.Read())
                {
                    Int32.TryParse((string)dr[0], out status);
                }
                
                dr.Close();

                dbConn.Close();
            }

            // 1 == Failure, 0 == Success
            if (status == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public bool IsJobRunning(string jobName)
        {
            int status = 0;

            using (SqlConnection dbConn = new SqlConnection(connectionString))
            {
                SqlCommand dbCmd = new SqlCommand("exec msdb.dbo.sp_help_job @job_name = N'" + jobName + "' ;", dbConn);
                dbConn.Open();

                SqlDataReader dr = dbCmd.ExecuteReader();

                if (dr.Read())
                {
                    status = Convert.ToInt32(dr["current_execution_status"]);
                }
                dr.Close();

                dbConn.Close();
            }

            if (status == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public bool DidJobSucceed(string jobName)
        {
            int status = 0;

            using (SqlConnection dbConn = new SqlConnection(connectionString))
            {
                SqlCommand dbCmd = new SqlCommand("exec msdb.dbo.sp_help_job @job_name = N'" + jobName + "' ;", dbConn);
                dbConn.Open();

                SqlDataReader dr = dbCmd.ExecuteReader();

                if (dr.Read())
                {
                    status = Convert.ToInt32(dr["last_run_outcome"]);
                }
                dr.Close();
                dbConn.Close();
            }

            if (status == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
