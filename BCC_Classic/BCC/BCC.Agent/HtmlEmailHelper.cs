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
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Timers;

using BCC.Core;
using BCC.Core.WMI.BizTalk;

namespace BCC.Agent
{
    class HtmlEmailHelper
    {
        private const string NBSP = "&nbsp;";

        public static string FormHTMLContent(DataTable taskTable, DateTime fromDate, DateTime toDate)
        {
            string htmlData = string.Empty;
            double totalTaskHours = 0;
            double taskHours = 0;
            StringBuilder tableBuilder = new StringBuilder();

            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append(ReadEmbeddedResource("BCC.Agent.Templates.htmlEnvelope.htm"));

            string emailHeading = string.Format("Task Status Report ({0} to {1})", fromDate.ToString("MMM-dd-yyyy"), toDate.ToString("MMM-dd-yyyy"));
            tableBuilder.Append(FormTaskContent(taskTable, fromDate, toDate, fromDate, DateTime.UtcNow.Date, emailHeading, out taskHours));
            tableBuilder.Append("<br /><br />");
            totalTaskHours += taskHours;

            emailHeading = "Upcoming tasks";
            tableBuilder.Append(FormTaskContent(taskTable, fromDate, toDate, DateTime.UtcNow.Date.AddDays(1), DateTime.UtcNow.Date.AddDays(14), emailHeading, out taskHours));
            tableBuilder.Append("<br /><br />");
            totalTaskHours += taskHours;

            htmlBuilder.Replace("$$TABLE_DATA", tableBuilder.ToString() );
            htmlBuilder.Replace("$$TOTAL_TASK_HOURS", totalTaskHours.ToString() );

            htmlData = htmlBuilder.ToString();

            return htmlData;
        }

        /// <summary>
        /// Format tasks 
        /// </summary>
        /// <param name="taskTable"></param>
        /// <returns></returns>
        public static string FormTaskContent(DataTable taskTable, 
            DateTime fromDate, DateTime toDate, 
            DateTime fromDueDate, DateTime toDueDate,
            string emailHeading, out double taskTotal)
        {
            string tableData = string.Empty;
            taskTotal = 0;

            try
            {
                StringBuilder htmlBuilder = new StringBuilder();
                StringBuilder rowBuilder = null;
                htmlBuilder.Append(ReadEmbeddedResource("BCC.Agent.Templates.tasktemplate.htm"));
                DateTime dueDate = DateTime.Now;
                string tableRow = string.Empty;
                
                //Sort the data please, due date asc
                DataView dataView = new DataView(taskTable);
                dataView.Sort = "taskDueDate ASC";
                dataView.RowFilter = "taskDueDate > '" + fromDueDate + "' AND taskDueDate <= '" + toDueDate + "'";

                foreach (DataRow taskRecord in dataView.ToTable().Rows)
                {
                    rowBuilder = new StringBuilder();
                    rowBuilder.Append(ReadEmbeddedResource("BCC.Agent.Templates.taskRowTemplate.htm"));

                    DateTime.TryParse(Convert.ToString(taskRecord["taskDueDate"]), out dueDate);

                    rowBuilder.Replace("$TASK_NAME", "#" + Convert.ToString(taskRecord["TaskRef"]) + NBSP + Convert.ToString(taskRecord["TaskTitle"]));
                    rowBuilder.Replace("$PROJECT_NAME", Convert.ToString(taskRecord["ProjectName"]));
                    rowBuilder.Replace("$DUE_DATE", dueDate.ToString("MMM-dd-yy"));
                    rowBuilder.Replace("$TASK_STATUS", Convert.ToString(taskRecord["taskStatus"]));
                    
                    string taskHours = Convert.ToString(taskRecord["TotalTaskHours"]);
                    double dTaskHours = 0;
                    Double.TryParse(taskHours, out dTaskHours);
                    
                    rowBuilder.Replace("$HOURS", taskHours);

                    taskTotal += dTaskHours;

                    string priority = Convert.ToString(taskRecord["taskPriority"]);

                    rowBuilder.Replace("$PRIORITY", NBSP + NBSP + priority + NBSP + NBSP);

                    switch (priority)
                    {
                        case "1": rowBuilder.Replace("$COLOR", "Orange");
                            break;
                        case "2": rowBuilder.Replace("$COLOR", "RoyalBlue");
                            break;
                        case "3": rowBuilder.Replace("$COLOR", "Teal");
                            break;
                    }

                    tableRow += rowBuilder.ToString();
                }

                htmlBuilder.Replace("$HEADING", emailHeading);
                htmlBuilder.Replace("$TOTAL_HOURS", taskTotal.ToString());
                htmlBuilder.Replace("$$ROW_DATA", tableRow);
                tableData = htmlBuilder.ToString();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.Write(exception.Message, "HtmlEmailHelper");
            }

            return tableData;
        }

        public static string ReadEmbeddedResource(string fileName)
        {
            string fileData = string.Empty;
            Assembly _assembly;
            StreamReader _textStreamReader;
            try
            {
                _assembly = Assembly.GetExecutingAssembly();
                _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream(fileName));
                fileData = _textStreamReader.ReadToEnd();
            }
            catch(Exception exception)
            {
                System.Diagnostics.Debug.Write(exception.Message, "HtmlEmailHelper");
            }
            
            return fileData;
        }


        public static string FormatContent(ArtifactMonitoringEventArgs e, string tableHeaderTitle)
        {
            StringBuilder htmlBody = new StringBuilder(
            "<html xmlns=\"http://www.w3.org/1999/xhtml\">"
            + "<head><title></title>"
            + "<style type=\"text/css\">"
            + ".itemStyle { color:#FDD017;background-color:#525252;width:35%;}"
            + "</style>"
            + "</head>"
            + "<body style=\"font-family: Verdana; font-size: x-small; background-color: #C0C0C0;\">"
            + "<table cellspacing=\"0\" cellpadding=\"4\" border=\"1\" style=\"font-family:Verdana;font-size:x-small;color:#333333;width:100%;border-collapse:collapse;\" bgcolor=\"#FFFFCC\">"
            + "<thead><tr class=\"itemStyle\"><th colspan=\"2\">" + tableHeaderTitle + "</th></tr></thead>");

            // BizTalk Artifact type
            htmlBody.Append("<tr><td class=\"itemStyle\">BizTalk Artifact Type</td><td>" + Enum.GetName(typeof(ArtifactType), e.ArtifactType) + "</td></tr>");

            if (e.ArtifactType != ArtifactType.EventLog)
            {
                if (e.ArtifactName != string.Empty)
                {
                    htmlBody.Append("<tr><td class=\"itemStyle\">BizTalk Artifact Name</td><td style='word-break:break-all;'>" + e.ArtifactName + "</td></tr>");
                }

                if (e.ArtifactStatus != string.Empty)
                {
                    string markup = string.Empty;

                    if ((e.ArtifactStatus.Equals("Bound")
                        || e.ArtifactStatus.Equals("Enlisted")
                        || e.ArtifactStatus.Equals("Unenlisted")
                        || e.ArtifactStatus.Equals("Disabled")
                        || e.ArtifactStatus.Equals("Stopped")
                        || e.ArtifactStatus.Contains("Suspended")
                        || e.ArtifactStatus.Equals("Unknown")))
                    {
                        markup = "<b><font color=\"Red\">" + e.ArtifactStatus + "</font></b>";
                    }
                    else
                    {
                        markup = "<b><font color=\"Green\">" + e.ArtifactStatus + "</font></b>";
                    }

                    htmlBody.Append("<tr><td class=\"itemStyle\">BizTalk Artifact Status</td><td>" + markup + "</td></tr>");
                }

                if (e.ArtifactURL != string.Empty)
                {
                    htmlBody.Append("<tr><td class=\"itemStyle\">BizTalk Artifact URL</td><td>" + e.ArtifactURL + "</td></tr>");
                }

                if (e.HostName != string.Empty)
                {
                    htmlBody.Append("<tr><td class=\"itemStyle\">BizTalk Host Name</td><td>" + e.HostName + "</td></tr>");
                }

                if (e.ServerName != string.Empty)
                {
                    htmlBody.Append("<tr><td class=\"itemStyle\">BizTalk Server Name</td><td>" + e.ServerName + "</td></tr>");
                }

                if (e.ReceiveLocationName != string.Empty)
                {
                    htmlBody.Append("<tr><td class=\"itemStyle\">BizTalk Receive location name</td><td>" + e.ReceiveLocationName + "</td></tr>");
                }
            }
            else
            {
                if (e.ArtifactName != string.Empty)
                {
                    htmlBody.Append("<tr><td class=\"itemStyle\">NT Eventlog logfile</td><td>" + e.ArtifactName + "</td></tr>");
                }

                if (e.ArtifactStatus != string.Empty)
                {
                    string markup = string.Empty;

                    if (e.ArtifactStatus.Equals("Error"))
                    {
                        markup = "<b><font color=\"Red\">" + e.ArtifactStatus + "</font></b>";
                    }
                    else
                    {
                        markup = "<b><font color=\"Orange\">" + e.ArtifactStatus + "</font></b>";
                    }

                    htmlBody.Append("<tr><td class=\"itemStyle\">NT Event Type</td><td>" + markup + "</td></tr>");
                }

                if (e.ArtifactURL != string.Empty)
                {
                    htmlBody.Append("<tr><td class=\"itemStyle\">Event Message</td><td>" + e.ArtifactURL + "</td></tr>");
                }

                if (e.HostName != string.Empty)
                {
                    htmlBody.Append("<tr><td class=\"itemStyle\">Computer Name</td><td>" + e.HostName + "</td></tr>");
                }

                if (e.ServerName != string.Empty)
                {
                    htmlBody.Append("<tr><td class=\"itemStyle\">Server Name</td><td>" + e.ServerName + "</td></tr>");
                }
            }

            htmlBody.Append("<tr><td class=\"itemStyle\">Event timestamp</td><td>" + e.EventTimestamp.ToString("MMM-dd-yyyy hh:mm:ss tt") + "</td></tr>");

            htmlBody.Append("</table><ul><li>To enable/disable alert notifications, use speedcode '103'.</li><li>To manage BCC Alert service, use speedcode '604'.</li></ul></body></html>");

            return htmlBody.ToString();
        }

    }
}
