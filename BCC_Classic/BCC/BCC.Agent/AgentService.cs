using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.Configuration;
using System.ServiceProcess;
using System.Text;
using System.Timers;

using BCC.Core;
using BCC.Core.WMI.BizTalk;

namespace BCC.Agent
{
    public partial class AgentService : ServiceBase
    {
        private Timer timer = null;
        private static List<BCCMonitoring> masterMonitoringList = null;
        private static List<BCCPerfCounterMonitor> masterPerfCounterList = null;
        private string category = "BizTalkControlCenterAgent";
        private bool debugFlag = false;
        private bool isLocalEmailFlag = false;
        private double timerInterval = 60 * 1 * 1000;
        private const string BCC_AGENT_CONFIG_SPEEDCODE = "604";
        
        /// <summary>
        /// Creates a new instance of BCCMonitoring master list.
        /// </summary>
        public AgentService()
        {
            InitializeComponent();
            SetupTimer();
        }

        /// <summary>
        /// 
        /// </summary>
        private bool IsLocalEmailFlag
        {
            get 
            { 
                return isLocalEmailFlag; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetupTimer()
        {
            timer = new Timer();
            masterMonitoringList = new List<BCCMonitoring>();
            masterPerfCounterList = new List<BCCPerfCounterMonitor>();
            bool.TryParse(ConfigurationManager.AppSettings["Verbose"], out debugFlag);
            bool.TryParse(ConfigurationManager.AppSettings["IsSmtpSettingsLocal"], out isLocalEmailFlag);
            double.TryParse(ConfigurationManager.AppSettings["TimerInterval"], out timerInterval);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            //Initial call 
            OnElapsedTime(null, null);
            
            // Call the report scheduler every one minute
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = timerInterval;
            this.timer.Start();
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            ActivateBizTalkArtifactMonitoring();
            ActivatePerformanceCounters();
            PurgeControlCenterData();
            GenerateTaskEmails();

            WriteToEventLog("OnElapsedTime() execution complete.");
        }

        protected override void OnStop()
        {
            this.timer.Stop();
            PerformSelfCleaningActivity();
            DeactivatePerformanceCounters();

            WriteToEventLog("OnStop() execution complete.");
        }

        protected override void OnPause()
        {
            this.timer.Stop();
        }

        protected override void OnContinue()
        {
            this.timer.Start();
        }

        private void GenerateTaskEmails()
        {
            TaskNotifier taskNotifier = new TaskNotifier();
            taskNotifier.EmailConfigSpeedCode = BCC_AGENT_CONFIG_SPEEDCODE;
            taskNotifier.DetermineWhenToSendTaskEmail();
            taskNotifier.PerformTaskCloning();
        }

        private void ActivatePerformanceCounters()
        {
            try
            {
                BCCPerfCounterMonitor perfCounterMonitor = null;
                BCCPerfCounterDataAccess da = new BCCPerfCounterDataAccess();

                foreach (BCCPerfCounterEntry entry in da.PerformanceCounterEntryList())
                {
                    perfCounterMonitor = masterPerfCounterList.Find(item => item.PerfCounterEntry.ToString() == entry.ToString());

                    if (perfCounterMonitor != null)
                    {
                        if (!entry.IsEnabled || (entry.IsEnabled && entry.IsMarkedForDelete)) // Entry is disabled.
                        {
                            perfCounterMonitor.Stop();
                            masterPerfCounterList.Remove(perfCounterMonitor);

                            WriteToEventLog("Perf counter removed for " + entry.ToString());
                        }
                    }
                    else
                    {
                        if (entry.IsEnabled)
                        {
                            perfCounterMonitor = new BCCPerfCounterMonitor(category, debugFlag, entry);
                            perfCounterMonitor.Start();
                            masterPerfCounterList.Add(perfCounterMonitor);

                            WriteToEventLog("Perf counter enabled for " + entry.ToString());
                        }
                    }

                    if (entry.IsMarkedForDelete)
                    {
                        da.RemovePerformanceCounterEntry(entry);
                        WriteToEventLog("Perf counter deleted for " + entry.ToString());
                    }

                }
            }
            catch (Exception exception)
            {
                WriteToEventLog(exception.Message + exception.StackTrace);
            }
        }

        private void ActivateBizTalkArtifactMonitoring()
        {
            try
            {
                // Read monitoring list
                BCCMonitoringDataAccess da = new BCCMonitoringDataAccess();
                // Get the monitoring list from the database
                List<BCCMonitoringEntry> list = da.MonitoringEntryList();

                // Reference monitor instance
                BCCMonitoring monitor = null;
                // Setup monitoring and enable events
                foreach (BCCMonitoringEntry entry in list)
                {
                    // Get the monitor instance from masterMonitoringList.
                    monitor = masterMonitoringList.Find(item => item.ArtifactName == entry.ArtifactName);

                    if (monitor != null)
                    {
                        if (!entry.IsEnabled || (entry.IsEnabled && entry.IsMarkedForDelete)) // Entry is disabled.
                        {
                            monitor.DisableMonitoring();
                            // remove the monitor from the master monitoring list.
                            masterMonitoringList.Remove(monitor);

                            WriteToEventLog("Monitoring disabled for " + monitor.ArtifactName);
                        }

                    }
                    else // Master monitoring list DOES NOT contain and hence add it. 
                    {
                        if (entry.IsEnabled)
                        {
                            try
                            {
                                // Create a new monitor instance from the monitoring entry
                                monitor = new BCCMonitoring(entry.ArtifactType, entry.ArtifactName, entry.PollingInterval);
                                // Add the monitor to master monitor list.
                                monitor.ArtifactStatusChanged += new ArtifactMonitoringEventHandler(monitor_ArtifactStatusChanged);
                                monitor.EnableMonitoring();
                                masterMonitoringList.Add(monitor);

                                WriteToEventLog("Monitoring enabled for " + entry.ArtifactType + "::" + monitor.ArtifactName);
                            }
                            catch (Exception ex)
                            {
                                WriteToEventLog(ex.Message + ex.StackTrace);
                            }
                        }
                    }
                    
                    if (entry.IsMarkedForDelete)
                    {
                        da.RemoveMonitoringEntry(entry.ArtifactType, entry.ArtifactName);

                        WriteToEventLog("Monitoring deleted for " + monitor.ArtifactName);
                    }
                }
            }
            catch (Exception exception)
            {
                WriteToEventLog(exception.Message + exception.StackTrace);
            }
        }

        /// <summary>
        /// Removes all the performance counter monitoring events - Spring cleaning
        /// </summary>
        public void DeactivatePerformanceCounters()
        {
            try
            {
                // Reference monitor instance
                BCCPerfCounterMonitor monitor = null;

                for (int count = 0; count < masterPerfCounterList.Count; count++)
                {
                    monitor = masterPerfCounterList[count];
                    monitor.Stop();
                }

                // Clean up and create a new instance
                masterPerfCounterList = new List<BCCPerfCounterMonitor>();
            }
            catch (Exception exception)
            {
                WriteToEventLog(exception.Message + exception.StackTrace);
            }
        }

        /// <summary>
        /// Removes all the monitoring events - Spring cleaning
        /// </summary>
        public void PerformSelfCleaningActivity()
        {
            try
            {
                // Reference monitor instance
                BCCMonitoring monitor = null;

                for (int count = 0; count < masterMonitoringList.Count; count++)
                {
                    monitor = masterMonitoringList[count];
                    monitor.DisableMonitoring();
                }

                // Clean up and create a new instance
                masterMonitoringList = new List<BCCMonitoring>();
            }
            catch (Exception exception)
            {
                WriteToEventLog(exception.Message + exception.StackTrace);
            }
        }

        private void PurgeControlCenterData()
        {
            try
            {
                int mdaDaysToKeep = 30;
                int pcdaDaysToKeep = 7;
                int webAuditDaysToKeep = 30;

                BCCManageConfigData configData = new BCCManageConfigData();
                configData.Speedcode = BCC_AGENT_CONFIG_SPEEDCODE;
                configData.Query();

                NameValuePairSet configSet = configData.ConfigurationData;

                foreach (NameValuePair nvPair in configSet)
                {
                    if (nvPair.Name.Equals(BCCUIHelper.Constants.DAYS_TO_KEEP_PERFDATA))
                    {
                        Int32.TryParse(nvPair.Value, out pcdaDaysToKeep);
                    }
                    else
                        if (nvPair.Name.Equals(BCCUIHelper.Constants.DAYS_TO_KEEP_USRAVT))
                        {
                            Int32.TryParse(nvPair.Value, out webAuditDaysToKeep);
                        }
                        else
                            if (nvPair.Name.Equals(BCCUIHelper.Constants.DAYS_TO_KEEP_USRNTF))
                            {
                                Int32.TryParse(nvPair.Value, out mdaDaysToKeep);
                            }
                }

                BCCMonitoringDataAccess mda = new BCCMonitoringDataAccess();
                mda.PurgeMonitoringData(mdaDaysToKeep);

                BCCPerfCounterDataAccess pcda = new BCCPerfCounterDataAccess();
                pcda.PurgePerformanceCounterData(pcdaDaysToKeep);

                BCCWebAudit.PurgeWebAuditEvents(webAuditDaysToKeep);
            }
            catch (Exception exception)
            {
                WriteToEventLog(exception.Message + exception.StackTrace);
            }
        }

        /// <summary>
        /// Event handler for WMI events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void monitor_ArtifactStatusChanged(object sender, ArtifactMonitoringEventArgs e)
        {
            try
            {
                int smtp_email_port = 25;
                bool smtp_email_ssl = false;
                string smtp_email_host = string.Empty;
                string smtp_email_username = string.Empty;
                string smtp_email_userpwd = string.Empty;
                string smtp_email_subject = string.Empty;
                string smtp_email_rcpnt = string.Empty;
                string smtp_email_title = "BizTalk Control Center (BCC) - Alert Notification";
                string mailSubject = "BCC Agent notification [" + Environment.MachineName + "]";

                string mailMessage = HtmlEmailHelper.FormatContent(e, smtp_email_title);

                if (!IsLocalEmailFlag)
                {
                    try
                    {
                        // Read monitoring list
                        BCCMonitoringDataAccess da = new BCCMonitoringDataAccess();

                        // Receive Port
                        if (e.ArtifactType == ArtifactType.ReceivePort)
                        {
                            da.LogMonitoringData(e.ArtifactType, e.ReceiveLocationName, e.ArtifactStatus);
                        }
                        // Host Instance
                        else if (e.ArtifactType == ArtifactType.HostInstance)
                        {
                            da.LogMonitoringData(e.ArtifactType, e.HostName, e.ArtifactStatus);
                        }
                        // Service Instance
                        else if (e.ArtifactType == ArtifactType.ServiceInstance)
                        {
                            da.LogMonitoringData(e.ArtifactType, e.ServerName, e.ArtifactStatus);
                        }
                        // Send Port
                        else 
                        {
                            da.LogMonitoringData(e.ArtifactType, e.ArtifactName, e.ArtifactStatus);
                        }
                    }
                    catch (Exception exception)
                    {
                        WriteToEventLog(exception.Message + exception.StackTrace);
                    }

                    // Send email 
                    EmailHelper helper = new EmailHelper(BCC_AGENT_CONFIG_SPEEDCODE);

                    mailMessage = HtmlEmailHelper.FormatContent(e, helper.EmailTitle);
                    mailSubject = helper.EmailSubject.Replace("$machineName", Environment.MachineName);
                    
                    helper.SendMail(mailSubject, mailMessage, false);

                    WriteToEventLog("Mail message:" + mailMessage);
                }
                else // Use local SMTP settings
                {
                    SmtpSection smtpSection = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;

                    // Get the from email of the config file  
                    smtp_email_username = smtpSection.From;
                    // Get the to address from the app settings
                    smtp_email_rcpnt = ConfigurationManager.AppSettings["EmailRecipients"].ToString();
                    // Get the host name from the smtp section
                    smtp_email_host = smtpSection.Network.Host;
                    
                    EmailHelper helper = new EmailHelper(smtp_email_port, smtp_email_ssl, smtp_email_host, smtp_email_username, smtp_email_userpwd, smtp_email_rcpnt, smtp_email_title, false);
                    helper.SendMail(mailSubject, mailMessage, true);
                }
            }
            catch (Exception exception)
            {
                WriteToEventLog(exception.Message + exception.StackTrace);
            }
        }

        private void WriteToEventLog(string message)
        {
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
    }
}
