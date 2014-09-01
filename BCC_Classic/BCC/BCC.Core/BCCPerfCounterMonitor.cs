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

namespace BCC.Core
{
    public class BCCPerfCounterMonitor
    {
        private string category = string.Empty;
        private bool debugFlag = false;
        private BCCPerfCounterEntry pcounter = null;
        private Timer pcounterTimer = null;
        private bool hasError = false;
        private string errorMessage = string.Empty;

        public BCCPerfCounterMonitor(string category, bool debugFlag, BCCPerfCounterEntry pcounter)
        {
            this.debugFlag = debugFlag;
            this.category = category;
            this.pcounter = pcounter;
        }

        public void Start()
        {
            try
            {
                if (pcounter.IsEnabled)
                {
                    pcounterTimer = new Timer();
                    // Converting into milliseconds
                    pcounterTimer.Interval = pcounter.PollingInterval * 1000;
                    pcounterTimer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
                    pcounterTimer.Enabled = true;
                    pcounterTimer.Start();
                    WriteToEventLog("Start()::" + pcounterTimer.Interval);
                }
            }
            catch (Exception ex)
            {
                WriteToEventLog(ex.Message + ex.StackTrace);
            }
        }

        public void Stop()
        {
            try
            {
                if (pcounterTimer != null)
                {
                    pcounterTimer.Stop();
                    pcounterTimer.Enabled = false;
                    pcounterTimer = null;
                }
            }
            catch (Exception ex)
            {
                WriteToEventLog(ex.Message + ex.StackTrace);
            }
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {
                // Get the performance counter value and log it into database.
                float pcounterData = BCCPerformanceCounters.Monitor(pcounter.PerfCategory, pcounter.PerfCounter, pcounter.PerfInstance);
                new BCCPerfCounterDataAccess().LogPerformanceCounterData(pcounter.PerfCategory, pcounter.PerfCounter, pcounter.PerfInstance, pcounterData);
                WriteToEventLog("OnElapsedTime::" + pcounterData);
            }
            catch (Exception ex)
            {
                hasError = true;
                errorMessage = "Message: " + ex.Message + ". Trace:" + ex.StackTrace + ".";
                WriteToEventLog(ex.Message + ex.StackTrace);
                Stop();
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

        public BCCPerfCounterEntry PerfCounterEntry
        {
            get
            {
                return this.pcounter;
            }

            set
            {
                this.pcounter = value;
            }
        }

        public bool HasError
        {
            get { return hasError; }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
        }
    }
}
