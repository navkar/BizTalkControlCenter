using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.Configuration;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

using BCC.Core;
using BCC.Core.Task;

namespace BCC.Agent
{
    /// <summary>
    /// This class is used to send task emails
    /// </summary>
    public class TaskNotifier
    {
        #region Init
        /// <summary>
        /// Task reminder email flag
        /// </summary>
        private bool taskReminderEmailFlag = false;

        /// <summary>
        /// Task cloning flag
        /// </summary>
        private bool taskCloningEnabled = false;

        /// <summary>
        /// Speed code
        /// </summary>
        private string _configSpeedCode = string.Empty;

        /// <summary>
        /// Application Name
        /// </summary>
        private string _applicationName = string.Empty;
        
        /// <summary>
        /// Time interval
        /// </summary>
        private const double TIME_INTERVAL = 60 * 1000;

        /// <summary>
        /// Time interval
        /// </summary>
        private const int USER_LIMIT = 25;

        /// <summary>
        /// Project Report
        /// </summary>
        private StringCollection _projectReportEmailList = null;

        /// <summary>
        /// Scheduled time to run
        /// </summary>
        private DateTime _scheduledTimeToRun = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 9, 0, 0);
        #endregion 

        #region Properties

        public StringCollection ProjectReportEmailCollection
        {
            get
            {
                return _projectReportEmailList;
            }
            set
            {
                _projectReportEmailList = value;
            }
        }

        public DateTime ScheduledTimeToRun
        {
            get
            {
                return _scheduledTimeToRun;
            }
            set
            {
                _scheduledTimeToRun = value;
            }
        }

        public string ApplicationName
        {
            get
            {
                return _applicationName;
            }
            set
            {
                _applicationName = value;
            }
        }

        public string EmailConfigSpeedCode
        {
            get
            {
                return _configSpeedCode;
            }
            set
            {
                _configSpeedCode = value;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Default time interval is 1 hr and executes at 9 AM agent local time.
        /// </summary>
        public TaskNotifier()
        {
            // Default
            this._applicationName = "BizTalkControlCenter";
            this._configSpeedCode = "604";
        }

        #endregion

        #region Methods
        private void LoadEmailConfiguration()
        {
            BCCManageConfigData configData = new BCCManageConfigData();
            configData.Speedcode = EmailConfigSpeedCode;
            configData.Query();

            NameValuePairSet configSet = configData.ConfigurationData;

            foreach (NameValuePair nvPair in configSet)
            {
                if (nvPair.Name.Equals(BCCUIHelper.Constants.TASK_REMINDER_EMAIL_FLAG))
                {
                    Boolean.TryParse(nvPair.Value, out taskReminderEmailFlag);
                }
                else
                    if (nvPair.Name.Equals(BCCUIHelper.Constants.TASK_REMINDER_EMAIL_TIME))
                    {
                        int hour = 9;
                        int min = 0;

                        try
                        {
                            string[] timeComponent = nvPair.Value.Split(':');

                            if (timeComponent != null)
                            {
                                Int32.TryParse(timeComponent[0], out hour);
                                Int32.TryParse(timeComponent[1], out min);
                            }
                        }
                        catch (Exception exception)
                        {
                            System.Diagnostics.Debug.Write(exception.Message, "TaskNotifier");
                        }

                        ScheduledTimeToRun = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, hour, min, 0);
                    }
                    else if (nvPair.Name.Equals(BCCUIHelper.Constants.TASK_CLONING_FLAG))
                    {
                        Boolean.TryParse(nvPair.Value, out taskCloningEnabled);
                    }
            }
        }

        public void DetermineWhenToSendTaskEmail()
        {
            LoadEmailConfiguration();

            if (taskReminderEmailFlag)
            {
                System.TimeSpan timeDiff = ScheduledTimeToRun.Subtract(DateTime.UtcNow);

                if ((timeDiff.TotalMilliseconds < TIME_INTERVAL && timeDiff.TotalMilliseconds > 0))
                   // && !(DateTime.UtcNow.DayOfWeek == DayOfWeek.Saturday || DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday))
                {
                    FindTasksAndNotifyUsers();
                }
            }
        }

        public void PerformTaskCloning()
        {
            LoadEmailConfiguration();

            if (taskCloningEnabled)
            {
                // Invoke BCC cloning sp
                BCCTask.CloneTask();
            }
        }

        public void ProjectSummaryReport(StringCollection recepientList)
        {
            if (recepientList != null && recepientList.Count > 0)
            {
                // Find the list of all the projects 
                // Fill BCCTaskEffort
                // Form HTML and send emails
                // Send email
            }
        }

        private void FindTasksAndNotifyUsers()
        {
            // Get all the users - 25 user limit.
            DataTable userTable = BCCTaskDataAccess.RetrieveAllUsers(ApplicationName, 0, USER_LIMIT);
            string mailMessage = string.Empty;
            string mailSubject = string.Empty;

            // foreach user in the user table.
            foreach (DataRow dr in userTable.Rows)
            {
                BCCTaskEffort effort = new BCCTaskEffort();

                effort.TaskAssignedToUserName = dr["userName"] as string;
                effort.TaskStatus = null; // has to be null to get all the tasks.
               
                DataTable taskTable = null;
                try
                {
                    // Get the list of tasks for each user
                    taskTable = BCCTaskEffort.ReportTaskEfforts(effort);
                }
                catch(Exception exception)
                {
                    System.Diagnostics.Debug.Write(exception.Message, "TaskNotifier");
                }

                // Dont send emails when there are no tasks.
                if (taskTable != null && taskTable.Rows != null && taskTable.Rows.Count > 0)
                {
                    EmailHelper helper = new EmailHelper(EmailConfigSpeedCode);

                    helper.EmailRecipient = dr["email"] as string;
                    WriteToEventLog("Sending email to : " + helper.EmailRecipient);

                    // Form the email message for each task
                    mailMessage = HtmlEmailHelper.FormHTMLContent(taskTable, effort.ReportStartDate, effort.ReportEndDate);
                    mailSubject = "(BCC) - Task Status Report - " + DateTime.UtcNow.Date.ToString("MMM-dd-yyyy");
                    helper.SendMail(mailSubject, mailMessage, false);
                }
                else
                {
                    System.Diagnostics.Debug.Write("No tasks found for " + effort.TaskAssignedToUserName, "TaskNotifier");
                }
            }
        }

        private void WriteToEventLog(string message)
        {
            bool debugFlag = true;
            string category = "TaskNotifier";

            if (debugFlag)
            {
                System.Diagnostics.EventLog.WriteEntry(category, message);
                System.Diagnostics.Debug.Write(message, category);
            }
            else
            {
                System.Diagnostics.Debug.Write(message, category);
            }
        }
        #endregion
    }
}
