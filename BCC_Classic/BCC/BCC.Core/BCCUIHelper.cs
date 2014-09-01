using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Xml;
using BCC.Core;
using System.Collections.Specialized;

/// <summary>
/// Summary description for BCCUIHelper
/// </summary>
namespace BCC.Core
{
    public class BCCUIHelper
    {
        BCCDataAccess dataAccess = new BCCDataAccess();

        public struct Constants
        {
            public const string DEFAULT_THEME = "Day";

            /// <summary>
            /// BizTalk Monitoring - Health Matrix
            /// </summary>
            public const string BMHM = "101";

            /// <summary>
            /// BizTalk Artifacts - Windows Services
            /// </summary>
            public const string BAWS = "201";
            public const string BAO = "203";
            public const string BASP = "205";
            public const string BARL = "206";

            public const string DEFAULT_BIZTALK_APP = "BizTalk Application 1";

            public const string SEND_PORT_APPS = "SEND_PORT_APPS";
            public const string RECEIVE_PORT_APPS = "RECEIVE_PORT_APPS";

            public const string ARTIFACT_SND = "Send Port";
            public const string ARTIFACT_RCV = "Receive Location";
            public const string ARTIFACT_ODX = "Orchestration";

            public const string LOG_EVENT_INFO = "Information";
            public const string LOG_EVENT_WARN = "Warning";
            public const string LOG_EVENT_ERR = "Error";

            public const string ENTRY_PROFILE_KEY = "EntryType";
            public const string EVENT_PROFILE_KEY = "EventType";
            public const string SEVERITY_PARAM_STR = "LOG_SEVERITY";
            public const string MACHINE_PARAM_STR = "MACHINE_NAME";
            public const string SERVICE_PROFILE_KEY = "ServiceList";
            public const string SEARCH_HISTORY_KEY = "SearchHistory";

            public const string SSO_CONNECTION_KEY = "SSOConnectionString";

            public const string DB_SERVER_LIST_KEY = "DatabaseServerWatchList";

            public const string SC102_UPPER_LIMIT_VALUE = "LoadMeterMaxLimit";

            public const string SC301_SERVICE_NAME = "ServiceName";
            public const string SC301_WEB_REQ_TIMEOUT = "WebRequestTimeout";

            public const string IS_WEB_EMAIL = "IsWebEmail";
            public const string SMTP_EMAIL_HOST = "SmtpEmailHost";
            public const string SMTP_EMAIL_PORT = "SmtpEmailPort";
            public const string SMTP_EMAIL_USERNAME = "SmtpEmailUserName";
            public const string SMTP_EMAIL_USERPWD = "SmtpEmailUserPassword";
            public const string SMTP_EMAIL_SUBJECT = "SmtpEmailSubject";
            public const string SMTP_EMAIL_RCPNT = "SmtpEmailRecipient";
            public const string SMTP_EMAIL_TITLE = "SmtpEmailTitle";

            public const string TASK_REMINDER_EMAIL_FLAG = "TaskReminderEmailFlag";
            public const string TASK_REMINDER_EMAIL_TIME = "TaskReminderEmailTime";
            public const string TASK_CLONING_FLAG = "TaskCloningFlag";

            public const string BT_MSG_DISPLAY_LIMIT = "BizTalkMessagesDisplayMaxLimit";
            /// <summary>
            /// Performance Counter Data
            /// </summary>
            public const string DAYS_TO_KEEP_PERFDATA = "KeepPerformanceData";
            /// <summary>
            /// Web User Activity Audit
            /// </summary>
            public const string DAYS_TO_KEEP_USRAVT = "KeepUserActivity";
            /// <summary>
            /// User Notifications
            /// </summary>
            public const string DAYS_TO_KEEP_USRNTF = "KeepUserNotifications";

            public const string SC303_INSTANCE_NOT_FOUND = "Instance unavailable";
            /// <summary>
            /// SMTP enable SSL value
            /// </summary>
            public const string SMTP_EMAIL_SSL = "SmtpEmailSSL";
            

            #region SC-502
            /// <summary>
            /// ConfigOperations - ConfigOperations
            /// </summary>
            public const string S502_CONFIG_OPERATIONS_KEY = "ConfigOperations";

            /// <summary>
            /// SSOConfiguration - SSO Configuration
            /// </summary>
            public const string S502_SSO_CONFIG_KEY = "SSOConfiguration";
            
            /// <summary>
            /// HostConfiguration - Host Configuration
            /// </summary>
            public const string S502_HOST_CONFIG_KEY = "HostConfiguration";
            #endregion
            /// <summary>
            /// Used to hold connection strings to various job servers
            /// </summary>
            public const string JOB_CONNECTION_STRING_LIST_KEY = "JobConnectionStringList";

            /// <summary>
            /// Used to filter out SQL server jobs
            /// </summary>
            public const string JOB_LIST_KEY = "JobList";

            /// <summary>
            /// Search Term Key used in DV.aspx
            /// </summary>
            public const string SEARCH_TERM_KEY = "SearchTerm";

            /// <summary>
            /// Configuration Directory Key
            /// </summary>
            public const string FILE_EXTN_LIST = "FileExtnList";

            /// <summary>
            /// Configuration Directory Key
            /// </summary>
            public const string CONFIG_DIRECTORY_LIST = "ConfigDirectoryList";
            
            /// <summary>
            /// Host instance list key
            /// </summary>
            public const string HOST_INSTANCE_KEY = "HostInstanceList";

            /// <summary>
            /// ApplicationList key from Profile.xml
            /// </summary>
            public const string APP_LIST_KEY = "ApplicationList";
            public const string REM_CHK_LIST_KEY = "ReminderCheckList";
            public const string ANNOUNCE_APP_KEY = "Announcement";
            public const string LINE_BK = "<br />";

            public const string CONFIG_PARAM_STR = "CONFIGURATION";
            public const string CONFIG_DIR_PARAM_STR = "CONFIG_DIRECTORY";
            public const string HOST_PARAM_STR = "HOST";
            public const string WEB_SERVICES = "PARTNER_WEB_SERVICES";
            public const string SERVICE_REQ_TEMPLATE = "REQ_TEMPLATE";
            /// <summary>
            /// Service Status - Running
            /// </summary>
            public const string STATUS_RUNNING = "Running";
            public const string STATUS_STARTED = "Started";
            public const string STATUS_STOPPED = "Stopped";
            public const string STATUS_ENLISTED = "Enlisted";
            public const string STATUS_UNENLISTED = "Unenlisted";
            public const string STATUS_ENABLED = "Enabled";
            public const string STATUS_DISABLED = "Disabled";
            public const string STATUS_ACTIVE = "Active";
            public const string STATUS_SUSPENDED = "Suspended";
            public const string STATUS_BOUND = "Bound";
            public const string STATUS_UNKNOWN = "Unknown";

            public const string NOT_ASSIGNED = "Not assigned";
            /// <summary>
            /// Set to lower case for ease of comparisions.
            /// </summary>
            public const string FLAG_TRUE = "true";

            /// <summary>
            /// Set to lower case for ease of comparisions.
            /// </summary>
            public const string FLAG_FALSE = "false";
            /// <summary>
            /// ROLE - BCCAdmin
            /// </summary>
            public const string ROLE_ADMIN = "BCCAdmin";

            /// <summary>
            /// ROLE - BCCArtifact
            /// </summary>
            public const string ROLE_ARTIFACT = "BCCArtifact";

            /// <summary>
            /// ROLE - BCCDeploy
            /// </summary>
            public const string ROLE_DEPLOY = "BCCDeploy";

            /// <summary>
            /// ROLE - BCCMember - Default role.
            /// </summary>
            public const string ROLE_MEMBER = "BCCMember";

            /// <summary>
            /// Access Denied Message.
            /// </summary>
            public const string ACCESS_DENIED = "You are not authorized to view this page. Check with your administrator.";
        }

        
        public BCCUIHelper()
        {
        }

        public static string BizTalkOrchestrationQueryString()
        {
            return "SELECT  o.nvcFullName AS Orchestration, COUNT(*) as Count," +
                                "CASE i.nState " +
                                "WHEN 1 THEN 'Ready To Run' " +
                                "WHEN 2 THEN 'Active' " +
                                "WHEN 4 THEN 'Suspended Resumable' " +
                                "WHEN 8 THEN 'Dehydrated' " +
                                "WHEN 16 THEN 'Completed With Discarded Messages' " +
                                "WHEN 32 THEN 'Suspended Non-Resumable' " +
                                "END as State " +
                                "FROM [BizTalkMsgboxDb]..[Instances] AS i WITH (NOLOCK) " +
                                "JOIN [" + BCCOperator.BizTalkMgmtDb() + "]..[bts_Orchestration] AS o WITH (NOLOCK) ON i.uidServiceID = o.uidGUID " +
                                "GROUP BY o.nvcFullName, i.nState;";

        }

        public static void MergeRows(GridView gridView)
        {
            for (int rowIndex = gridView.Rows.Count - 2; rowIndex >= 0; rowIndex--)
            {
                GridViewRow row = gridView.Rows[rowIndex];
                GridViewRow previousRow = gridView.Rows[rowIndex + 1];

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    if (row.Cells[i].Text == previousRow.Cells[i].Text)
                    {
                        row.Cells[i].RowSpan = previousRow.Cells[i].RowSpan < 2 ? 2 :
                                               previousRow.Cells[i].RowSpan + 1;
                        previousRow.Cells[i].Visible = false;
                    }
                }
            }
        }
        
        public void PopulateDropDown(DropDownList dlist, StringCollection entryList)
        {
            dlist.Items.Clear();
            dlist.Items.Insert(0, new ListItem("Select", "select"));
            if (entryList.Count > 0)
            {
                foreach (String entryItem in entryList)
                {
                    dlist.Items.Add(new ListItem(entryItem, entryItem));
                }
            }

        }

        /// <summary>
        /// Retrives Parameter values from 'filter' tag.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paramName">PARAMETER_NAME in Filters.xml</param>
        /// <returns></returns>
        public static ArrayList RetrieveParamValue(ArrayList filterList, string paramName)
        {
            ArrayList applicationNames = null;

            foreach (BCCFilter bccFilter in filterList)
            {
                if (bccFilter.name == paramName)
                {
                    if (applicationNames == null)
                    {
                        applicationNames = new ArrayList();
                    }

                    applicationNames.Add(bccFilter.value);
                }
            }

            return applicationNames;
        }

        public static DataTable RetrieveFilterData(string filterPath)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Category", typeof(string)));
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Value", typeof(string)));

            DataSet ds = new DataSet();
            ds.ReadXml(filterPath);

            XmlDataDocument xmlDataDoc = new XmlDataDocument(ds);
            string strXPathQuery = "/root/filter";
            string category = string.Empty;
            string itemName = string.Empty;
            string itemValue = string.Empty;

            DataRow dr = null;

            foreach (XmlNode nodeDetail in xmlDataDoc.SelectNodes(strXPathQuery))
            {
                category = nodeDetail.ChildNodes[0].InnerText.ToString();
                itemName = nodeDetail.ChildNodes[1].InnerText.ToString();
                itemValue = nodeDetail.ChildNodes[2].InnerText.ToString();

                dr = dt.NewRow();

                dr["Category"] = category;
                dr["Name"] = itemName;
                dr["Value"] = itemValue;

                dt.Rows.Add(dr);
            }

            return dt;
        }
        
        public void SetMenuItem(Menu menu, string itemText)
        {
            if (menu != null)
            {
                if (menu.SelectedItem == null)
                {
                    foreach (MenuItem item in menu.Items)
                    {
                        if (itemText.Equals(item.Text))
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
        }

    }

    public class SelectionBar
    {
        private ArrayList array = null;
        private int noOfItems = 1;
        private string pageFileName = string.Empty;

        public SelectionBar(int noOfItems, string pageFileName)
        {
            array = new ArrayList();
            this.noOfItems = noOfItems;
            this.pageFileName = pageFileName;
        }

        /// <summary>
        /// Gets the count of the current items in the array.
        /// </summary>
        public int Count
        {
            get
            {
                if ( array != null)
                {
                    return array.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public void Add(string data)
        {
            if (!array.Contains(data))
            {
                array.Add(data);
            }

            if (array.Count > this.noOfItems)
            {
                array.RemoveAt(0); //
            }
        }

        public string ToSearchTerms()
        {
            string searchTerms = "";

            foreach (string data in array)
            {
                searchTerms = wrapAnchor(data) + ", " + searchTerms;
            }

            return searchTerms.Substring(0, searchTerms.Length - 1);
        }

        private string wrapAnchor(string data)
        {
            return "<a style='border-style:outset;color:White;background-color:Gray;font-weight:bold;text-decoration:none;' href='" + this.pageFileName + "?searchTerm=" + data + "'>" + data + "</a>";
        }
    }
}