using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BCC.Core;
using BCC.Core.WMI.BizTalk;
using System.Xml;
using System.Collections;
using System.Data;
using System.IO;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //BCCMonitoringDataAccess da = new BCCMonitoringDataAccess();

                //List<BCCMonitoringEntry> list = da.MonitoringEntryList();

                //foreach (BCCMonitoringEntry entry in list)
                //{
                //    Console.WriteLine("Entry: "+ entry.ArtifactName);

                //}

                //Console.Write("ggg" + System.DateTime.Now.ToString("MMM-dd-yyyy hh:mm:ss tt"));

                //Console.ReadLine();

                //BCCMonitoring x = new BCCMonitoring(ArtifactType.SendPort, "SynergyCRMDynamics", 10);

                // BCCMonitoring x = new BCCMonitoring(ArtifactType.HostInstance, "IHS_Receive", 10);

               //BCCMonitoring x = new BCCMonitoring(ArtifactType.ServiceInstance, "BizTalk Application 1", 10);

                //BCCMonitoring x = new BCCMonitoring(ArtifactType.EventLog, "Application", 10);

                //x.EnableMonitoring();


                //x.ArtifactStatusChanged += new ArtifactMonitoringEventHandler(x_PortStatusChanged);

                //Console.Write("Waiting for events...");
                //Console.ReadLine();
                //x.DisableMonitoring();
                //Console.Write("Monitoring disabled...");
                //Console.ReadLine();

                NameValuePairSet set = new NameValuePairSet();

                NameValuePair pair = new NameValuePair();
                pair.Name = "IsWebEmail";
                pair.DisplayName = "Web Email";
                pair.Value = "True";
                pair.ToolTip = "For web-based email, set it to True, else False.";

                set.Add(pair);

                pair = new NameValuePair();
                pair.Name = "SmtpEmailHost";
                pair.DisplayName = "Email Host";
                pair.Value = "smtp.gmail.com";
                pair.ToolTip = "Email server name";

                set.Add(pair);

                pair = new NameValuePair();
                pair.Name = "SmtpEmailPort";
                pair.DisplayName = "Email Port";
                pair.Value = "587";
                pair.ToolTip = "Email port number";

                set.Add(pair);

                pair = new NameValuePair();
                pair.Name = "SmtpEmailUserName";
                pair.DisplayName = "From Email ID";
                pair.Value = "biztalkcontrolcenter@gmail.com";
                pair.ToolTip = "From email address or email server username";

                set.Add(pair);

                pair = new NameValuePair();
                pair.Name = "SmtpEmailUserPassword";
                pair.DisplayName = "Password";
                pair.Value = "P@ssw0rd$";
                pair.ToolTip = "password";
                set.Add(pair);

                pair = new NameValuePair();
                pair.Name = "SmtpEmailRecipient";
                pair.DisplayName = "Recipient Email ID";
                pair.Value = "youremailid@yahoo.com";
                pair.ToolTip = "destination email address (use comma for mulitple email addresses)";
                set.Add(pair);

                pair = new NameValuePair();
                pair.Name = "SmtpEmailSubject";
                pair.DisplayName = "Subject";
                pair.Value = "BizTalk Control Center (BCC) Alert on [$machineName]";
                pair.ToolTip = "Email subject";
                set.Add(pair);

                pair = new NameValuePair();
                pair.Name = "SmtpEmailTitle";
                pair.DisplayName = "Title";
                pair.Value = "BizTalk Control Center (BCC) - Alert Notification";
                pair.ToolTip = "Email Title";
                set.Add(pair);

                pair = new NameValuePair();
                pair.Name = "SmtpEmailSSL";
                pair.DisplayName = "SSL enabled";
                pair.Value = "True";
                pair.ToolTip = "Specify 'False', if SSL is not enabled.";
                set.Add(pair);

                pair = new NameValuePair();
                pair.Name = "KeepPerformanceData";
                pair.DisplayName = "Keep Performance Data";
                pair.Value = "7";
                pair.ToolTip = "Default value is 7 days";
                set.Add(pair);

                pair = new NameValuePair();
                pair.Name = "KeepUserNotifications";
                pair.DisplayName = "Keep User Notifications";
                pair.Value = "30";
                pair.ToolTip = "Default value is 30 days";
                set.Add(pair);

                pair = new NameValuePair();
                pair.Name = "KeepUserActivity";
                pair.DisplayName = "Keep User Activity";
                pair.Value = "30";
                pair.ToolTip = "Default value is 30 days";
                set.Add(pair);

                pair = new NameValuePair();
                pair.Name = "TaskReminderEmailFlag";
                pair.DisplayName = "Tasks Email";
                pair.Value = "True";
                pair.ToolTip = "Default value is 'True'";
                set.Add(pair);

                pair = new NameValuePair();
                pair.Name = "TaskReminderEmailTime";
                pair.DisplayName = "Tasks Email Time (UTC)";
                pair.Value = "09:00";
                pair.ToolTip = "Specify time in 24-hour format (hh:mm), default value is '09:00'";
                set.Add(pair);

                pair = new NameValuePair();
                pair.Name = "TaskCloningFlag";
                pair.DisplayName = "Task Cloning";
                pair.Value = "True";
                pair.ToolTip = "Task cloning will check for recurring tasks and create new tasks";
                set.Add(pair);

                BCC.Core.BCCManageConfigData cd = new BCCManageConfigData();

                cd.Speedcode = "604";
                cd.ConfigurationData = set;
                cd.Update();

                cd = new BCCManageConfigData();
                cd.Speedcode = "604";
                cd.Query();

                set = cd.ConfigurationData;

                Console.WriteLine("Count: " + set.Count);

                //BCCPerformanceCounters x = new BCCPerformanceCounters();

                //DataTable dt = x.EnumeratePerformanceCounters();

                //int count = dt.Rows.Count;

                //BCCPerfCounterDataAccess x = new BCCPerfCounterDataAccess();

                //BCCPerfCounterEntry entry = new BCCPerfCounterEntry("Cat", "Inst", "Counter");


                //x.CreatePerformanceCounterEntry(entry);

                //BCCMonitoringDataAccess da = new BCCMonitoringDataAccess();
                //da.LogMonitoringData(ArtifactType.ReceivePort, "DropCRMRequestLocation", "Hello");

                //string[] daysOfWeek = new string[7];

                //for (int count = 0; count < 7; count++)
                //{
                //    daysOfWeek[count] = DateTime.Today.Subtract(new TimeSpan(6 - count, 0, 0, 0)).DayOfWeek.ToString();
                //}

                //BCCPerfCounterDataAccess da = new BCCPerfCounterDataAccess();

                ////da.LogPerformanceCounterData(".NET Data Provider for SqlServer", "NumberOfInactiveConnectionPools", "windowsservice_0[1432]", 33);

                //List<BCCPerfCounterEntry> list = da.PerformanceCounterEntryList();

                //string data = "This is my data";

                //UnicodeEncoding unicode = new UnicodeEncoding();
                //byte[] byteData = unicode.GetBytes(data);


                //string hexString = BCCHexUtil.ToString(byteData);

                //Console.WriteLine("hexString: " + hexString);

                //if (BCCHexUtil.IsHexString(hexString))
                //{
                //   Console.WriteLine("Yes. This is a hex string.");
                //   byte[] normalData =  BCCHexUtil.GetBytes(hexString);

                //   string normal = unicode.GetString(normalData);
                //   Console.WriteLine("Back to normal::" + normal);
                //}

                //BCCTransformationHelper helper = new BCCTransformationHelper();

                //XmlDocument xDoc = new XmlDocument();
                //xDoc.Load(@"C:\BCC_Classic\BCC\tasks.xml");

                //string data = helper.Transform(xDoc.OuterXml, @"C:\BCC_Classic\BCC\BCC.web\xslt\TaskEmailView.xslt");

                //File.WriteAllText(@"C:\BCC_Classic\BCC\Sample.html", data);
                //System.Timers.Timer t = new System.Timers.Timer();

                //t.Interval = 60 * 1000;
                //t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
                //t.AutoReset = true;
                //t.Enabled = true;
                //Console.WriteLine("waiting for timer event...." + DateTime.Now.ToString());
            }
            catch (Exception e)
            {
                String data = e.Message + e.StackTrace;
                Console.WriteLine(data);
            }

            Console.WriteLine("Press any key...");
            Console.Read();
        }

        static void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DateTime _scheduledTimeToRun = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 00, 0);

            System.TimeSpan timeDiff = _scheduledTimeToRun.Subtract(DateTime.Now);

            // This code will not run after the elapse of the time. ever.
            if (timeDiff.TotalMilliseconds < 60 * 1000 ) 
            {
                Console.WriteLine("Tick: " + timeDiff.TotalMilliseconds.ToString() + DateTime.Now.ToString());
            }
            else
            {
                Console.WriteLine("No Tick: " + timeDiff.TotalMilliseconds.ToString() + DateTime.Now.ToString());
            }
        }

        static void x_PortStatusChanged(object sender, ArtifactMonitoringEventArgs e)
        {
            Console.WriteLine("ArtifactName:" + e.ArtifactName);
            Console.WriteLine("ArtifactURL:" + e.ArtifactURL);
            Console.WriteLine("ArtifactStatus:" + e.ArtifactStatus);
            Console.WriteLine("ReceiveLocationName:" + e.ReceiveLocationName);
            Console.WriteLine("HostName:" + e.HostName);
            Console.WriteLine("ServerName:" + e.ServerName);
            
        }
    }
}
