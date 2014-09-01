using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Web.Management;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;

using Microsoft.Win32;
using System.Web;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;

namespace BCC.Core
{
    public class BCCOperator
    {
        BCCDataAccess dataAccess = null;
        private static string connectionString = string.Empty;

        public BCCOperator()
        {
            dataAccess = new BCCDataAccess();
        }

        static BCCOperator()
        {
            connectionString = ConfigurationManager.ConnectionStrings["bizTalk"].ConnectionString;
        }

        public static string BizTalkMgmtDb()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            return connection.Database;
        }

        public static string BizTalkSQLServer()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            return connection.DataSource;
        }

        private string GenerateResourceSpecForMSI(string applicationName, string destinationPath)
        {
            string filePath = string.Empty;

            try
            {
                filePath = destinationPath + "\\resourceSpec.xml";

                StringBuilder buildCmd = new StringBuilder();

                buildCmd.Append("BTSTask ListApp /ApplicationName:\"");
                buildCmd.Append(applicationName + "\""); // end with double-quote
                buildCmd.Append(" /ResourceSpec:\"");
                buildCmd.Append(filePath + "\"");

                StreamWriter sw = File.CreateText(destinationPath + "\\BTSResourceSpec.cmd");
                sw.Write(buildCmd.ToString());
                sw.Flush();
                sw.Close();

                Process execute = Process.Start(destinationPath + "\\BTSResourceSpec.cmd");
                execute.WaitForExit();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return filePath;
        }

        private void ModifyResourceSpecForMSI(string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(filePath);

            // <Resource Type="System.BizTalk:WebDirectory"
            XmlNodeList webNodeList = xmlDoc.SelectNodes("//*[local-name()='Resource'][@Type='System.BizTalk:WebDirectory']");

            foreach (XmlNode webNode in webNodeList)
            {
                if (webNode != null)
                {
                    webNode.ParentNode.RemoveChild(webNode);
                }
            }
            xmlDoc.Save(filePath);
        }

        public int ExportMSIFile(string applicationName,
                                string serverName,
                                string databaseName,
                                string destinationPath,
                                string destinationFileName)
        {
            int returnCode = 0;

            try
            {
                string filePath = GenerateResourceSpecForMSI(applicationName, destinationPath);
                ModifyResourceSpecForMSI(filePath);

                StringBuilder buildCmd = new StringBuilder();

                buildCmd.Append("BTSTask ExportApp /ApplicationName:\"");
                buildCmd.Append(applicationName + "\""); // end with double-quote
                buildCmd.Append(" /Server:");
                buildCmd.Append(serverName);
                buildCmd.Append(" /ResourceSpec:\"");
                buildCmd.Append(filePath + "\""); // double-quote
                buildCmd.Append(" /Database:");
                buildCmd.Append(databaseName);
                buildCmd.Append(" /Package:\""); // end with double-quote
                buildCmd.Append(destinationPath + "\\" + destinationFileName);
                buildCmd.Append("\"");
                buildCmd.Append(@" >> msi.log");

                StreamWriter sw = File.CreateText(destinationPath + "\\BTSExportMSI.cmd");
                sw.Write(buildCmd.ToString());
                sw.Flush();
                sw.Close();

                Process execute = Process.Start(destinationPath + "\\BTSExportMSI.cmd");
                execute.WaitForExit();
            }
            catch
            {
                returnCode = 1;
            }

            return returnCode;
        }

        public int ExportBindingFile(string applicationName, 
                                        string serverName, 
                                        string databaseName, 
                                        string destinationPath,
                                        string destinationFileName)
        {
            int returnCode = 0;

            try
            {

                StringBuilder buildCmd = new StringBuilder();

                buildCmd.Append("BTSTask ExportBindings /ApplicationName:\"");
                buildCmd.Append(applicationName + "\""); // end with double-quote
                buildCmd.Append(" /Server:");
                buildCmd.Append(serverName);
                buildCmd.Append(" /Database:");
                buildCmd.Append(databaseName);
                buildCmd.Append(" /Destination:\""); // end with double-quote
                buildCmd.Append(destinationPath + "\\" + destinationFileName);
                buildCmd.Append("\""); // double-quote

                StreamWriter sw = File.CreateText(destinationPath + "\\BTSExportBindings.cmd");
                sw.Write(buildCmd.ToString());
                sw.Flush();
                sw.Close();

                Process execute = Process.Start(destinationPath + "\\BTSExportBindings.cmd");
                execute.WaitForExit();
            }
            catch
            {
                returnCode = 1;
            }

            return returnCode;
        }

        public int ImportBindingFile(string applicationName,
                                string serverName,
                                string databaseName,
                                string sourcePath,
                                string sourceFileName)
        {
            int returnCode = 0;

            try
            {

                StringBuilder buildCmd = new StringBuilder();

                buildCmd.Append("BTSTask ImportBindings /ApplicationName:\"");
                buildCmd.Append(applicationName + "\""); // end with double-quote
                buildCmd.Append(" /Server:");
                buildCmd.Append(serverName);
                buildCmd.Append(" /Database:");
                buildCmd.Append(databaseName);
                buildCmd.Append(" /Source:\""); // end with double-quote
                buildCmd.Append(sourcePath + "\\" + sourceFileName);
                buildCmd.Append("\""); // double-quote

                StreamWriter sw = File.CreateText(sourcePath + "\\BTSImportBindings.cmd");
                sw.Write(buildCmd.ToString());
                sw.Flush();
                sw.Close();

                Process execute = Process.Start(sourcePath + "\\BTSImportBindings.cmd");
                execute.WaitForExit();
            }
            catch
            {
                returnCode = 1;
            }

            return returnCode;
        }

        public void GenerateBindingFile(string xmlData, string destinationPath)
        {
            try
            {
                StringBuilder 
                    buffer = new StringBuilder();
                    buffer.Append(xmlData);

                StreamWriter sw = File.CreateText(destinationPath);
                    sw.Write(buffer.ToString());
                    sw.Flush();
                    sw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetServiceStatus(StringCollection serviceList)
        {
            // This method creates a DataTable 
            //   DateAdded      datetime
            DataTable dt = new DataTable();
            // define the table's schema
            dt.Columns.Add(new DataColumn("ServiceName", typeof(string)));
            dt.Columns.Add(new DataColumn("ServiceStatus", typeof(string)));
            dt.Columns.Add(new DataColumn("IsEnabled", typeof(string)));

            ServiceController controller = null;
            string status = string.Empty;

            foreach (string serviceName in serviceList)
            {
                controller = new ServiceController();
                controller.MachineName = Environment.MachineName;
                controller.ServiceName = serviceName;
                
                if (BCCServiceHelper.IsInstalled(controller.ServiceName))
                //if (dataAccess.IsServiceEnabled(controller.ServiceName))
                {
                    status = controller.Status.ToString();
                }
                else
                {
                    status = "Not Installed";
                }

                // create the rows
                DataRow dr = dt.NewRow();
                dr["ServiceName"] = serviceName;
                dr["ServiceStatus"] = status;
                dr["IsEnabled"] = status;

                dt.Rows.Add(dr);
            }

            return dt;
        }

        public DataTable RetrieveAllConfigs(FileInfo[] files)
        {
            DataTable dt = new DataTable();

            // define the table's schema
            dt.Columns.Add(new DataColumn("DirectoryName", typeof(string)));
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("CreationTime", typeof(string)));
            dt.Columns.Add(new DataColumn("LastAccessTime", typeof(string)));
            dt.Columns.Add(new DataColumn("FullName", typeof(string)));

            try
            {
                foreach (FileInfo fileInfo in files)
                {
                    // create the rows
                    DataRow dr = dt.NewRow();
                    dr["DirectoryName"] = fileInfo.DirectoryName;
                    dr["Name"] = fileInfo.Name;
                    dr["CreationTime"] = fileInfo.CreationTime.ToString();
                    dr["LastAccessTime"] = fileInfo.LastAccessTime.ToString();
                    dr["FullName"] = fileInfo.FullName;
                    dt.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        public void SetCheckAllCheckBox(GridView gridViewObj, string controlName, bool checkState)
        {
            // Iterate through the Gridview Rows property
            foreach (GridViewRow row in gridViewObj.Rows)
            {
                // Access the CheckBox
                CheckBox cb = (CheckBox)row.FindControl(controlName);
                if (cb != null)
                    cb.Checked = checkState;
            }
        }

        public DataTable GetSchedulerStatus(ArrayList schedulerList)
        {
            // This method creates a DataTable 
            //   DateAdded      datetime
            DataTable dt = new DataTable();
            // define the table's schema
            dt.Columns.Add(new DataColumn("ReportSchedulerId", typeof(string)));
            dt.Columns.Add(new DataColumn("ReportSchedulerName", typeof(string)));
            dt.Columns.Add(new DataColumn("ReportSchedulerType", typeof(string)));
            dt.Columns.Add(new DataColumn("ReportSchedulerStatus", typeof(string)));
            dt.Columns.Add(new DataColumn("ReportYear", typeof(string)));
            dt.Columns.Add(new DataColumn("ReportMonth", typeof(string)));
            dt.Columns.Add(new DataColumn("ReportDay", typeof(string)));
            dt.Columns.Add(new DataColumn("ReportHourMin", typeof(string)));

            foreach (BCCReportScheduleStruct schedulers in schedulerList)
            {
                // create the rows
                DataRow dr = dt.NewRow();
                dr["ReportSchedulerId"] = schedulers.HubRepSchdId;
                dr["ReportSchedulerName"] = schedulers.ReportName;
                dr["ReportSchedulerType"] = schedulers.ScheduleType;
                dr["ReportYear"] = schedulers.Year;
                dr["ReportMonth"] = schedulers.Month;
                dr["ReportDay"] = schedulers.Day;
                dr["ReportHourMin"] = schedulers.HH + ":" + schedulers.MI;
                dr["ReportSchedulerStatus"] = schedulers.Status;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public void PopulateSchedulerlds(GridView gridViewObj, string controlName, ArrayList schedulerList)
        {
            BCCReportScheduleStruct schedulers;
            HiddenField hf;

            foreach (GridViewRow row in gridViewObj.Rows)
            {
                schedulers = (BCCReportScheduleStruct)schedulerList[row.RowIndex];
                hf = (HiddenField)row.FindControl(controlName);
                hf.Value = schedulers.HubRepSchdId;
            }

        }

        public string EnableDisableReportSchedulers(GridView gridViewObj, string repIdControlName, string chkboxControlName, bool isEnabled)
        {
            int errorCode = 0;
            string errorMsg = string.Empty;
            string status = string.Empty;
            string reportSchedulerId = string.Empty;
            HiddenField hf;
            // Iterate through the Gridview Rows property
            foreach (GridViewRow row in gridViewObj.Rows)
            {
                status = row.Cells[8].Text;

                // Get SchedulerReportId
                hf = (HiddenField)row.FindControl(repIdControlName);
                reportSchedulerId = hf.Value;

                // Access the CheckBox
                CheckBox cb = (CheckBox)row.FindControl(chkboxControlName);
                if (cb != null && cb.Checked)
                {
                    try
                    {
                        if (isEnabled && "DISABLED".Equals(status.ToUpper()))
                        {
                            //dataAccess.UpdRepSchdStatus(reportSchedulerId, "E", ref errorCode, ref errorMsg);
                            if (errorCode != 0)
                            {
                                throw new Exception(errorMsg);
                            }
                        }
                        else if (!isEnabled && "ENABLED".Equals(status.ToUpper()))
                        {
                            //dataAccess.UpdRepSchdStatus(reportSchedulerId, "D", ref errorCode, ref errorMsg);
                            if (errorCode != 0)
                            {
                                throw new Exception(errorMsg);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        errorMsg += ex.ToString();
                    }
                }
            }
            return errorMsg;
        }

        public const string BIZTALK_ADMIN_KEY = "Software\\Microsoft\\BizTalk Server\\3.0\\Administration";
    }

    public class ActivityHelper
    {
        private const string TR_BGN = "<tr>";
        private const string TR_END = "</tr>";
        private const string TD_BGN = "<td>";
        private const string TD_END = "</td>";
        private const string APP_OBJECT_NAME = "ACTIVITY_HISTORY";
        private List<ActivityData> array = null;
        private ActivityData activityData = null;


        public ActivityHelper()
        {
            array = new List<ActivityData>();
        }

        public List<ActivityData> ActivityDataList
        {
            get
            {
                return array;
            }
        }

        public void Add(string userName, string userRole, DateTime activityDate, string moduleName, string activity)
        {
            activityData = new ActivityData();

            activityData.UserName = userName;
            activityData.UserRole = userRole;
            activityData.ActivityDate = activityDate;
            activityData.ModuleName = moduleName;
            activityData.Activity = activity;

            array.Add(activityData);

            if (array.Count >= 15)
            {
                array.RemoveRange(0, array.Count);
            }
        }

        public override string ToString()
        {
            string activityTags = "";
            ActivityData activityData = null;

            for (int counter = 0; counter < array.Count; counter++)
            {
                activityData = array[counter];
                activityTags = activityTags + activityData.ToString();
            }

            return activityTags;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ActivityData> ActivityItems(System.Web.HttpContext httpContext)
        {
            ActivityHelper activityList = null;

            if (httpContext.Application[APP_OBJECT_NAME] != null)
            {
                activityList = httpContext.Application[APP_OBJECT_NAME] as ActivityHelper;
            }
            else
            {
                activityList = new ActivityHelper();
            }

            return activityList.array;
        }

        public string ToHTMLString()
        {
            string activityTags = "";

            for (int counter = 0; counter < array.Count; counter++)
            {
                activityTags = activityTags + CreateRow(array[counter]);
            }

            return activityTags;
        }

        private string CreateRow(ActivityData activityData)
        {
            return TR_BGN +
                   TD_BGN + activityData.UserName + TD_END +
                   TD_BGN + activityData.ActivityDate + TD_END +
                   TD_BGN + activityData.ModuleName + TD_END +
                   TD_BGN + activityData.Activity + TD_END +
                   TR_END;

        }

        public static string LastKnownActivity(System.Web.HttpContext httpContext)
        {
            ActivityHelper activityList = null;
            List<ActivityData> list = null;
            String userInfo = String.Empty;

            if (httpContext.Application[APP_OBJECT_NAME] != null)
            {
                activityList = httpContext.Application[APP_OBJECT_NAME] as ActivityHelper;

                if (activityList != null)
                {
                    list = activityList.array;

                    if (list.Count > 0)
                    {
                        ActivityData data = list[list.Count - 1];
                        userInfo = data.UserName + " *" + data.Activity + "* " + data.ModuleName + " at " + data.ActivityDate;
                    }
                }
            }

            return userInfo;
        }
        
        /// <summary>
        /// This method is called by all the modules which require User Activity trace.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="moduleName"></param>
        /// <param name="activity"></param>
        /// <param name="pageCode"></param>
        public void RaiseAuditEvent(object source, string moduleName, string activity, int pageCode)
        {
            ActivityData activityData = new ActivityData();
            activityData.Activity = activity;
            activityData.ActivityDate = System.DateTime.Now.ToLocalTime();
            activityData.ModuleName = moduleName;

            try
            {
                if (source != null)
                {
                    Page currentPage = source as Page;
                    activityData.UserName = currentPage.User.Identity.Name;
                    UpdateApplicationObject(currentPage, moduleName, activity);
                }
            }
            catch
            {

            }

            string message = string.Format("{0} {1} on {2}.", 
                activityData.UserName, 
                activityData.Activity, String.Format("{0:MMM dd, yyyy hh:mm:ss tt}", activityData.ActivityDate) ); 
            
            BCCWebAuditEvent bccEvent = new BCCWebAuditEvent(message, source, WebEventCodes.WebExtendedBase + pageCode, activityData);
            bccEvent.Raise();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="moduleName"></param>
        /// <param name="activity"></param>
        private void UpdateApplicationObject(Page sourcePage, string moduleName, string activity)
        {
            ActivityHelper activityList = null;
        
            try
            {
                if (sourcePage.Application[APP_OBJECT_NAME] != null)
                {
                    activityList = sourcePage.Application[APP_OBJECT_NAME] as ActivityHelper;
                }
                else
                {
                    activityList = new ActivityHelper();
                    sourcePage.Application[APP_OBJECT_NAME] = activityList;
                }

                activityList.Add(sourcePage.User.Identity.Name, "", System.DateTime.Now, moduleName, activity);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.Write(exception.Message,"BCC Operator - UpdateApplicationObject - Exception");
            }
        }
    }

    public class ActivityData
    {
        private string userName = string.Empty;
        private string userRole = string.Empty;
        private DateTime activityTime;
        private string moduleName = string.Empty;
        private string activity = string.Empty;

        public string UserRole
        {
            get
            {
                return userRole;
            }
            set
            {
                userRole = value;
            }
        }

        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
            }
        }

        public DateTime ActivityDate
        {
            get
            {
                return activityTime;
            }
            set
            {
                activityTime = value;
            }
        }


        public string ModuleName
        {
            get
            {
                return moduleName;
            }
            set
            {
                moduleName = value;
            }
        }

        public string Activity
        {
            get
            {
                return activity;
            }
            set
            {
                activity = value;
            }
        }

        public override string ToString()
        {
            return UserName + "#" + UserRole + "#" + ActivityDate.ToString() + "#" + ModuleName + "#" + Activity;
        }
    }    
}
