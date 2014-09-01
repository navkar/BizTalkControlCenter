using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Threading;
using System.Collections;

using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace BCC.Core
{
    public class BCCReportScheduler
    {
        System.Threading.Timer dlyTime  = null;
        System.Threading.Timer wklyTime = null;
        System.Threading.Timer mlyTime = null;
        public const string SCHD_DAILY = "DAILY";
        public const string SCHD_WEEKLY = "WEEKLY";
        public const string SCHD_MONTHLY = "MONTHLY";
        public const string SCHD_YEARLY = "YEARLY";
        public const Int32 INTERVAL = 3600000;
        BCCDataAccess dataAccess = null;
        DateTime dtCurrent;
        // Declare variable for db error code and error message
        int error_code = 0;
        String error_msg = null;
        static Hashtable htReptSignals;
        
        // Define system datetime structure
        [StructLayout(LayoutKind.Sequential)]
        public struct sysdatetime
        {
            public short year;
            public short month;
            public short dayOfWeek;
            public short day;
            public short hour;
            public short minute;
            public short second;
            public short milliSeconds;
        }
        // Define TimeZoneInformation structure as it is in registry
        [StructLayout(LayoutKind.Sequential)]
        public struct REG_TZI
        {
            public int bias;
            public int standardBias;
            public int dayLightBias;
            public sysdatetime standardDate;
            public sysdatetime daylightDate;
        }
        public BCCReportScheduler()
        {
            dataAccess = new BCCDataAccess();

            // Get current (Pacific Time Zone)
            // Get UTC time for PST(Pacific Time Zone)
            dtCurrent = DateTime.UtcNow.AddHours(-8);
            // Get TZI for PST from registry
            String pstRegLoc = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones\\Pacific Standard Time";
            REG_TZI pstTzi = readRegTZI(pstRegLoc);
            
            // Create DayLight Start and End Time
            DateTime dlStartDate = new DateTime(DateTime.Now.Year, pstTzi.daylightDate.month, pstTzi.daylightDate.day, pstTzi.daylightDate.hour, pstTzi.daylightDate.minute, pstTzi.daylightDate.second);
            DateTime dlEndDate = new DateTime(DateTime.Now.Year, pstTzi.standardDate.month, pstTzi.standardDate.day, pstTzi.standardDate.hour, pstTzi.standardDate.minute, pstTzi.standardDate.second);
            if (dtCurrent.CompareTo(dlStartDate) > 0 && dtCurrent.CompareTo(dlEndDate) < 0)
            {
               dtCurrent = dtCurrent.AddHours(1);
            }
            
        }
        public REG_TZI readRegTZI(String strTZKeyLoc)
        {
            // Read TimeZoneInformation for a given timezone from registry
            object objTzi = Registry.GetValue(strTZKeyLoc, "TZI", null);
            int length = Marshal.SizeOf(typeof(REG_TZI));
            // Allocate memory to store TimeZoneInformation
            IntPtr tziPtr = Marshal.AllocHGlobal(length);
            // Copy TimeZoneInformation to the allocated memory
            Marshal.Copy((byte[])objTzi, 0, tziPtr, length);
            // Convert TZI in memory to the defined registry TZI structure
            REG_TZI regTzi = (REG_TZI)Marshal.PtrToStructure(tziPtr, typeof(REG_TZI));
            // Free the allocated memory
            Marshal.FreeHGlobal(tziPtr);
            
            return regTzi;
        }
        public void schedule(DateTime dtEventDateTime)
        {
            ArrayList repSchdList = null;// dataAccess.GetHubRepSchdList(null, null, ref error_code, ref error_msg);
            for (int i = 0; i < repSchdList.Count; i++)
            {
                BCCReportScheduleStruct repSchd = (BCCReportScheduleStruct)repSchdList[i];
                switch (repSchd.ScheduleType)
                {
                    case SCHD_DAILY:
                        dailyReportInvoker(repSchd);
                        break;
                    case SCHD_WEEKLY:
                        weeklyReportInvoker(repSchd);
                        break;
                    case SCHD_MONTHLY:
                        monthlyReportInvoker(repSchd);
                        break;
                    /*case SCHD_YEARLY:
                        YearlyReportInvoker(repSchd);
                        break;
                    */
                    default:
                        break;
                }
            }

        }

        private void dailyReportInvoker(BCCReportScheduleStruct dlyRepSchd)
        {
          
            // Set the year, month and day to today
            dlyRepSchd.Year= dtCurrent.Year.ToString();
            dlyRepSchd.Month = dtCurrent.Month.ToString();
            dlyRepSchd.Day = dtCurrent.Day.ToString();

            Int32 runInterval = 0;

            // Create actual report schedule date.
            DateTime dtScheduled = new DateTime(Int32.Parse(dlyRepSchd.Year), Int32.Parse(dlyRepSchd.Month), Int32.Parse(dlyRepSchd.Day), Int32.Parse(dlyRepSchd.HH), Int32.Parse(dlyRepSchd.MI), 0);
                if ( dtScheduled.CompareTo(dtCurrent) > 0)
                {
                    System.TimeSpan timeDiff = dtScheduled.Subtract(dtCurrent);
                    if ((timeDiff.TotalMilliseconds < INTERVAL) || (timeDiff.TotalMilliseconds == 0))
                    {
                        runInterval = Convert.ToInt32(timeDiff.TotalMilliseconds);
                        if (htReptSignals == null)
                        {
                            htReptSignals = new Hashtable();
                        }
                        if (!htReptSignals.Contains(dlyRepSchd.HubRepSchdId.ToString()))
                        {
                            dlyTime = new System.Threading.Timer(dispatchDlyReport, dlyRepSchd, runInterval, System.Threading.Timeout.Infinite);
                            htReptSignals.Add(dlyRepSchd.HubRepSchdId.ToString(), "Y");
                        }
                    }
                }
              
        }

        //private void dispatchDlyReport(object source)
        //{
        //    BCCReportScheduleStruct reportSpec = (BCCReportScheduleStruct)source;
        //    switch (reportSpec.ReportName)
        //    {
        //        case "SERVICE_REPORT":
        //            BCCMailer bccMailer = new BCCMailer();
        //            string subject = "["+ Environment.MachineName + "]: " + "Hub Service Usage Report";
        //            string mailMessage = "";
        //            string mailHeader = "Org CRM".PadRight(7)+" "+"ServiceName".PadRight(50)+ "   "+"DTD".PadRight(10)+" "+"WTD".PadRight(10)+" "+"MTD".PadRight(10)+" "+"YTD".PadRight(10)+" "+"TTD".PadRight(10);
        //            string headerLiner = "".PadRight(112,Char.Parse("_"));
        //            string mailFooter = "DTD: Day ToDay, WTD: Week ToDay, MTD: Month ToDay, YTD: Year ToDay, TTD: Till ToDay";
        //            mailMessage = mailHeader + "\n" + headerLiner;
        //            ArrayList serviceUsageList = dataAccess.GetServiceReport(DateTime.Now, ref error_code, ref error_msg);
        //            foreach (BCCServiceReportStruct servRepEntity in serviceUsageList)
        //            {
        //                mailMessage = mailMessage + "\n" + servRepEntity.Orginator.Trim().PadRight(7) + " " + servRepEntity.ServiceName.PadRight(50) + "   " + servRepEntity.DTD.Trim().PadRight(10) + " " + servRepEntity.WTD.Trim().PadRight(10) + " " + servRepEntity.MTD.Trim().PadRight(10) + " " + servRepEntity.YTD.Trim().PadRight(10) + " " + servRepEntity.TTD.Trim().PadRight(10);
        //            }
        //            mailMessage = mailMessage + "\n\n" +mailFooter;
        //            bccMailer.sendMail(null, null, subject, mailMessage, false, null);
        //            htReptSignals.Remove(reportSpec.HubRepSchdId.ToString());
        //            break;
        //    }
        //}

        private void dispatchDlyReport(object source)
        {
        //    BCCReportScheduleStruct reportSpec = (BCCReportScheduleStruct)source;
        //    switch (reportSpec.ReportName)
        //    {
        //        case "SERVICE_REPORT":
        //            BCCMailer bccMailer = new BCCMailer();
        //            string subject = "[" + Environment.MachineName + "]: " + "Hub Service Usage Report";
        //            string mailMessage = "", hubReportHTML = "";
        //            System.IO.MemoryStream msReport;
        //            System.Net.Mail.MailMessage mailMsg = new System.Net.Mail.MailMessage();

        //            string mailHeader = "Org CRM".PadRight(7) + " " + "ServiceName".PadRight(50) + "   " + "DTD".PadRight(10) + " " + "WTD".PadRight(10) + " " + "MTD".PadRight(10) + " " + "YTD".PadRight(10) + " " + "TTD".PadRight(10);
        //            string headerLiner = "".PadRight(112, Char.Parse("_"));
        //            string mailFooter = "DTD: Day ToDay, WTD: Week ToDay, MTD: Month ToDay, YTD: Year ToDay, TTD: Till ToDay";
        //            mailMessage = mailHeader + "\n" + headerLiner;
        //            ArrayList serviceUsageList = null;// dataAccess.GetServiceReport(DateTime.Now, ref error_code, ref error_msg);

        //            //hubReportHTML holds the HTML of attachment which provides with the graphical interpretation of daily statistics.
        //            hubReportHTML = "<html>\n" +
        //                          "<head>\n" +
        //                            "<script type=\"text/javascript\" src=\"http://www.google.com/jsapi\"></script>\n" +
        //                            "<script type=\"text/javascript\">\n" +
        //                             "google.load(\"visualization\", \"1\", {packages:[\"columnchart\"]});\n" +
        //                              "google.setOnLoadCallback(drawChart);\n" +
        //                              "function drawChart() {\n" +
        //                                "var data = new google.visualization.DataTable();\n" +
        //                                "data.addColumn('string', 'Operation');\n";

        //            foreach (string crmName in Enum.GetNames(typeof(BCCCrms)))
        //            {
        //                hubReportHTML = hubReportHTML + "data.addColumn('number', '" + crmName  + "');\n";
        //            }
 
        //            hubReportHTML = hubReportHTML + "data.addRows(7);\n" +
        //                                "data.setValue(0, 0, 'Create Company Link');\n" +
        //                                "data.setValue(1, 0, 'Update Company Data');\n" +
        //                                "data.setValue(2, 0, 'Delete Company Link');\n" +
        //                                "data.setValue(3, 0, 'Get CRM Company Data');\n" +
        //                                "data.setValue(4, 0, 'Get Company Data');\n" +
        //                                "data.setValue(5, 0, 'Share Pipeline');\n" +
        //                                "data.setValue(6, 0, 'Update Pipeline');\n";

        //            for (int i = 0; i < 7; i++)
        //            {
        //                foreach (int crmid in Enum.GetValues(typeof(BCCCrms)))
        //                {
        //                    Enum.GetName(typeof(BCCCrms), crmid);
        //                    hubReportHTML = hubReportHTML + "data.setValue(" + i + ", " + crmid + ", 0);\n";
        //                }
        //            }

        //            foreach (BCCServiceReportStruct servRepEntity in serviceUsageList)
        //            {
        //                mailMessage = mailMessage + "\n" + servRepEntity.Orginator.Trim().PadRight(7) + " " + servRepEntity.ServiceName.PadRight(50) + "   " + servRepEntity.DTD.Trim().PadRight(10) + " " + servRepEntity.WTD.Trim().PadRight(10) + " " + servRepEntity.MTD.Trim().PadRight(10) + " " + servRepEntity.YTD.Trim().PadRight(10) + " " + servRepEntity.TTD.Trim().PadRight(10);
        //                try
        //                {
        //                    hubReportHTML = hubReportHTML + "data.setValue(" + (int)(BCCOperation)Enum.Parse(typeof(BCCOperation), servRepEntity.ServiceName, true) + "," + (int)(BCCCrms)Enum.Parse(typeof(BCCCrms), servRepEntity.Orginator.Trim(), true) + "," + servRepEntity.DTD.ToString() + ");\n";
        //                }
        //                catch{}
        //            }

        //            hubReportHTML = hubReportHTML + "var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));\n" +
        //                                            "chart.draw(data, {width: 800, height: 320, is3D: true, title: 'HUB Statistics - Daily'});}\n" +
        //                                            "</script></head>\n<body><div id=\"chart_div\"></div></body>\n</HTML>";

        //            mailMessage = mailMessage + "\n\n" + mailFooter;

        //            msReport = new System.IO.MemoryStream(System.Text.ASCIIEncoding.UTF8.GetBytes(hubReportHTML));

        //            mailMsg.Attachments.Add(new System.Net.Mail.Attachment(msReport, "DailyReport.html"));
                    
        //            bccMailer.sendMail(null, null, subject, mailMessage, false, mailMsg.Attachments);
        //            htReptSignals.Remove(reportSpec.HubRepSchdId.ToString());
        //            break;
        //    }
        }
        private void weeklyReportInvoker(BCCReportScheduleStruct wklyRepSchd)
        {

        //    // Set the year, month and day to today
        //    wklyRepSchd.Year = dtCurrent.Year.ToString();
        //    wklyRepSchd.Month = dtCurrent.Month.ToString();

        //    Int32 runInterval = 0;

        //    // Create actual report schedule date.
        //    DateTime dtScheduled = new DateTime(Int32.Parse(wklyRepSchd.Year), Int32.Parse(wklyRepSchd.Month), Int32.Parse(wklyRepSchd.Day), Int32.Parse(wklyRepSchd.HH), Int32.Parse(wklyRepSchd.MI), 0);
        //    if (dtScheduled.DayOfWeek.Equals(dtCurrent.DayOfWeek) && dtScheduled.CompareTo(dtCurrent) > 0)
        //    {
        //        System.TimeSpan timeDiff = dtScheduled.Subtract(dtCurrent);
        //        if ((timeDiff.TotalMilliseconds < INTERVAL) || (timeDiff.TotalMilliseconds == 0))
        //        {
        //            runInterval = Convert.ToInt32(timeDiff.TotalMilliseconds);
        //            wklyTime = new System.Threading.Timer(dispatchWklyReport, wklyRepSchd, runInterval, System.Threading.Timeout.Infinite);
        //        }
        //    }
        }

        private void dispatchWklyReport(object source)
        {
            BCCReportScheduleStruct reportSpec = (BCCReportScheduleStruct)source;
            switch (reportSpec.ReportName)
            {
                default:
                    break;
            }
        }

        private void monthlyReportInvoker(BCCReportScheduleStruct mlyRepSchd)
        {

            // Set the year, month and day to today
            mlyRepSchd.Year = dtCurrent.Year.ToString();
            Int32 runInterval = 0;

            // Create actual report schedule date.
            DateTime dtScheduled = new DateTime(Int32.Parse(mlyRepSchd.Year), Int32.Parse(mlyRepSchd.Month), Int32.Parse(mlyRepSchd.Day), Int32.Parse(mlyRepSchd.HH), Int32.Parse(mlyRepSchd.MI), 0);
            if (dtScheduled.Month.Equals(dtCurrent.Month) && dtScheduled.Day.Equals(dtCurrent.Day) && dtScheduled.CompareTo(dtCurrent) > 0)
            {
                System.TimeSpan timeDiff = dtScheduled.Subtract(dtCurrent);
                if ((timeDiff.TotalMilliseconds < INTERVAL) || (timeDiff.TotalMilliseconds == 0))
                {
                    runInterval = Convert.ToInt32(timeDiff.TotalMilliseconds);
                    mlyTime = new System.Threading.Timer(dispatchMlyReport, mlyRepSchd, runInterval, System.Threading.Timeout.Infinite);
                }
            }

        }

        private void dispatchMlyReport(object source)
        {
            BCCReportScheduleStruct reportSpec = (BCCReportScheduleStruct)source;
            switch (reportSpec.ReportName)
            {
                default:
                    break;
            }
        }

    }
}
