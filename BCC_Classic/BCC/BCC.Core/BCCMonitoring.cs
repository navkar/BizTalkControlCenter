using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using BCC.Core;
using BCC.Core.WMI.BizTalk;

namespace BCC.Core.WMI.BizTalk
{
    public enum ArtifactType
    {
        ReceivePort = 1,
        SendPort = 2,
        HostInstance = 3,
        ServiceInstance = 4,
        EventLog = 5,
        PerfCounters = 6
    }

    public class ArtifactMonitoringEventArgs : EventArgs
    {
        private ArtifactType artifactType = ArtifactType.ReceivePort;
        private string artifactName = string.Empty;
        private string receiveLocationName = string.Empty;
        private string artifactURL = string.Empty;
        private string serverName = string.Empty;
        private string hostName = string.Empty;
        private string artifactStatus = string.Empty;
        private DateTime eventTimestamp = DateTime.Now;

        public ArtifactMonitoringEventArgs(ArtifactType artifactType, string artifactName, string receiveLocationName, string artifactURL, string serverName, string hostName, string artifactStatus)
        {
            this.artifactName = artifactName;
            this.receiveLocationName = receiveLocationName;
            this.artifactURL = artifactURL;
            this.serverName = serverName;
            this.hostName = hostName;
            this.artifactStatus = artifactStatus;
            this.artifactType = artifactType;
        }

        public ArtifactType ArtifactType
        {
            get { return this.artifactType; }
        }

        public string ArtifactName
        {
            get { return this.artifactName; }
        }

        public string ReceiveLocationName
        {
            get { return this.receiveLocationName; }
        }

        public string ArtifactURL
        {
            get { return this.artifactURL; }
        }

        public string ServerName
        {
            get { return this.serverName; }
        }

        public string HostName
        {
            get { return this.hostName; }
        }

        public string ArtifactStatus
        {
            get { return this.artifactStatus; }
        }

        public DateTime EventTimestamp
        {
            get { return this.eventTimestamp; } 
        }
    }

    public delegate void ArtifactMonitoringEventHandler(object sender,
                                                  ArtifactMonitoringEventArgs e);


    public class BCCMonitoring
    {
        public event ArtifactMonitoringEventHandler ArtifactStatusChanged;
        private const string BIZTALK_SCOPE = @"\\.\root\MicrosoftBizTalkServer";
        private const string EVENT_LOG_SCOPE = @"\\.\root\CIMV2";
        private ManagementEventWatcher wmiEvents = null;
        private string artifactName = string.Empty;
        private ArtifactType artifactType = ArtifactType.ReceivePort;
        private int pollIntervalInSecs = 10;
        private string query = string.Empty;

        /// <summary>
        /// Use to monitor various BizTalk Artifacts
        /// </summary>
        /// <param name="artifactType"></param>
        /// <param name="artifactName"></param>
        /// <param name="pollIntervalInSecs"></param>
        public BCCMonitoring(ArtifactType artifactType, string artifactName, int pollIntervalInSecs)
        {
            this.artifactType = artifactType;
            this.artifactName = artifactName;
            this.pollIntervalInSecs = pollIntervalInSecs;
        }

        public string ArtifactName
        {
            get
            {
                return artifactName;
            }
        }

        public int PollIntervalInSecs
        {
            get
            {
                return pollIntervalInSecs;
            }
        }

        public void EnableMonitoring()
        {
            switch (artifactType)
            {
                case ArtifactType.ReceivePort:
                    query = String.Format(@"SELECT * FROM __InstanceModificationEvent WITHIN {0} where TargetInstance ISA 'MSBTS_ReceiveLocation' AND TargetInstance.Name = '{1}'", pollIntervalInSecs, artifactName);
                    break;

                case ArtifactType.SendPort:
                    query = String.Format(@"SELECT * FROM __InstanceModificationEvent WITHIN {0} where TargetInstance ISA 'MSBTS_SendPort' AND TargetInstance.Name = '{1}'", pollIntervalInSecs, artifactName);
                    break;

                case ArtifactType.HostInstance:
                    query = String.Format(@"SELECT * FROM __InstanceModificationEvent WITHIN {0} where TargetInstance ISA 'MSBTS_HostInstance' AND TargetInstance.HostName = '{1}'", pollIntervalInSecs, artifactName);
                    break;

                case ArtifactType.ServiceInstance:
                    query = String.Format(@"SELECT * FROM __InstanceCreationEvent WITHIN {0} where TargetInstance ISA 'MSBTS_ServiceInstance' AND TargetInstance.ServiceClass = 4 AND (TargetInstance.ServiceStatus = 4 OR TargetInstance.ServiceStatus = 16 OR TargetInstance.ServiceStatus = 32)", pollIntervalInSecs);
                    break;

                case ArtifactType.EventLog:
                    query = String.Format(@"SELECT * FROM __InstanceCreationEvent where TargetInstance ISA 'Win32_NTLogEvent' AND TargetInstance.LogFile='{0}' AND (TargetInstance.EventType = 1 OR TargetInstance.EventType = 2)", artifactName);
                    break;
            }

            System.Diagnostics.Debug.Write(query, "BCC-Monitoring-WMI-Query");

            WqlEventQuery eventQuery = new WqlEventQuery(query);
            wmiEvents = new ManagementEventWatcher(new ManagementScope(BIZTALK_SCOPE), eventQuery);

            if (artifactType == ArtifactType.EventLog)
            {
                wmiEvents = new ManagementEventWatcher(new ManagementScope(EVENT_LOG_SCOPE), eventQuery);    
            }

            wmiEvents.EventArrived += new EventArrivedEventHandler(WmiEventReceived);

            // Start monitoring!
            wmiEvents.Start();
        }

        public void DisableMonitoring()
        {
            wmiEvents.Stop();
        }

        // This is a generic event handler
        private void WmiEventReceived(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject evt = e.NewEvent;
            PropertyDataCollection col = evt.Properties;
            PropertyDataCollection.PropertyDataEnumerator en = col.GetEnumerator();

            while (en.MoveNext())
            {
                PropertyData data = en.Current;

                // Grab TargetInstance only (could use PreviousInstance to get prior state data) :
                if (data.Value != null && data.Name == "TargetInstance")
                {
                    ManagementBaseObject mbo = (ManagementBaseObject)data.Value;
                    if (mbo.ClassPath.ClassName == "MSBTS_SendPort") // Send port event
                    {
                       BCC.Core.WMI.BizTalk.SendPort.ManagementSystemProperties
                            prop = new SendPort.ManagementSystemProperties(mbo);
                        int iSendPortStatus = Convert.ToInt32(mbo.Properties["Status"].Value.ToString());
                        
                        string portStatus = SendPortStatus(iSendPortStatus);
                        string serverName = prop.SERVER;
                        string sndPortName = mbo.Properties["Name"].Value.ToString();
                        string artifactURL = mbo.Properties["PTAddress"].Value.ToString();

                        OnArtifactStatusChanged(new ArtifactMonitoringEventArgs(ArtifactType.SendPort, sndPortName, "", artifactURL, serverName, "", portStatus));
                    }
                    else if (mbo.ClassPath.ClassName == "MSBTS_ReceiveLocation") // Receive location event
                    {
                        BCC.Core.WMI.BizTalk.ReceiveLocation.ManagementSystemProperties 
                            prop = new ReceiveLocation.ManagementSystemProperties(mbo);
                        bool isDisabled = true;
                        bool.TryParse(mbo.Properties["IsDisabled"].Value.ToString(), out isDisabled);

                        string portStatus = ReceiveLocationStatus(isDisabled);
                        string serverName = prop.SERVER;
                        string rcvLocName = mbo.Properties["Name"].Value.ToString();
                        string rcvPortName = mbo.Properties["ReceivePortName"].Value.ToString();
                        string artifactURL = mbo.Properties["InboundTransportURL"].Value.ToString();
                        string hostName = mbo.Properties["HostName"].Value.ToString();

                        OnArtifactStatusChanged(new ArtifactMonitoringEventArgs(ArtifactType.ReceivePort, rcvPortName, rcvLocName, artifactURL, serverName, hostName, portStatus));
                    }
                    else if (mbo.ClassPath.ClassName == "MSBTS_HostInstance") 
                    {
                        BCC.Core.WMI.BizTalk.HostInstance.ManagementSystemProperties
                            prop = new HostInstance.ManagementSystemProperties(mbo);

                        int serviceState = 0;
                        Int32.TryParse(mbo.Properties["ServiceState"].Value.ToString(), out serviceState);

                        string hiStatus = HostInstanceStatus(serviceState);
                        string serverName = prop.SERVER;
                        string artifactURL = mbo.Properties["RunningServer"].Value.ToString();
                        string hostName = mbo.Properties["HostName"].Value.ToString(); // This is the Host Name
                        string artifactName = mbo.Properties["Name"].Value.ToString(); // Name of the host instance. 

                        OnArtifactStatusChanged(new ArtifactMonitoringEventArgs(ArtifactType.HostInstance, artifactName, "", artifactURL, serverName, hostName, hiStatus));
                    }
                    else if (mbo.ClassPath.ClassName == "MSBTS_ServiceInstance") 
                    {
                        BCC.Core.WMI.BizTalk.ServiceInstance.ManagementSystemProperties
                            prop = new ServiceInstance.ManagementSystemProperties(mbo);

                        int serviceState = 0;
                        Int32.TryParse(mbo.Properties["ServiceStatus"].Value.ToString(), out serviceState);
                        string serviceInstanceStatus = ServiceInstanceStatus(serviceState);
                        string serverName = prop.SERVER;
                        string artifactURL = mbo.Properties["ErrorDescription"].Value.ToString();
                        string hostName = mbo.Properties["HostName"].Value.ToString();
                        string artifactName = mbo.Properties["ServiceName"].Value.ToString();
                        string messageType = "";// mbo.Properties["MessageType"].Value.ToString();

                        OnArtifactStatusChanged(new ArtifactMonitoringEventArgs(ArtifactType.ServiceInstance, artifactName, messageType, artifactURL, serverName, hostName, serviceInstanceStatus));
                    }
                    else if (mbo.ClassPath.ClassName == "Win32_NTLogEvent")
                    {
                        BCC.Core.WMI.BizTalk.ServiceInstance.ManagementSystemProperties
                            prop = new ServiceInstance.ManagementSystemProperties(mbo);

                        string serviceInstanceStatus = Enum.GetName(typeof(WMI.BizTalk.NTLogEvent.EventTypeValues), mbo.Properties["EventType"].Value);
                        string serverName = prop.SERVER;
                        string artifactURL = mbo.Properties["Message"].Value.ToString();

                        string hostName = mbo.Properties["ComputerName"].Value.ToString();
                        string artifactName = mbo.Properties["LogFile"].Value.ToString();
                        string messageType = "";

                        OnArtifactStatusChanged(new ArtifactMonitoringEventArgs(ArtifactType.EventLog, artifactName, messageType, artifactURL, serverName, hostName, serviceInstanceStatus));
                    }
                }
            }
        }

        protected void OnArtifactStatusChanged(ArtifactMonitoringEventArgs e)
        {
            if (ArtifactStatusChanged != null)
            {
                ArtifactStatusChanged(this, e);
            }
        }

        private string ServiceInstanceStatus(int serviceInstanceStatus)
        {
            string serviceStatus = string.Empty;

            switch (serviceInstanceStatus)
            {
                case 4: serviceStatus = "Suspended (Resumable)"; break;
                case 16: serviceStatus = "Completed with Discarded messages (Zombie)"; break;
                case 32: serviceStatus = "Suspended (Non Resumable)"; break;
                default: serviceStatus = "null"; break;
            }

            return serviceStatus;
        }

        private string HostInstanceStatus(int hostInstanceStatus)
        {
            string hostStatus = string.Empty;

            switch (hostInstanceStatus)
            {
                case 1: hostStatus = "Stopped"; break;
                case 2: hostStatus =  "Start pending"; break;
                case 3: hostStatus =  "Stop pending"; break;
                case 4: hostStatus =  "Running"; break;
                case 5: hostStatus =  "Continue pending"; break;
                case 6: hostStatus =  "Pause pending"; break;
                case 7: hostStatus =  "Pause"; break;
                case 8: hostStatus =  "Unknown"; break;
                default: hostStatus =  "null"; break;    
            }

            return hostStatus;
        }

        private string SendPortStatus(int PortStatus)
        {
            string strPortStatus = string.Empty;

            switch (PortStatus)
            {
                case 1:
                    strPortStatus = "Unenlisted";
                    break;
                case 2:
                    strPortStatus = "Stopped";
                    break;
                case 3:
                    strPortStatus = "Started";
                    break;
            }

            return strPortStatus;
        }

        private string ReceiveLocationStatus(bool IsDisabled)
        {
            string strLocationStatus = "Disabled";
            
            if (!IsDisabled)
            {
                strLocationStatus = "Enabled";
            }

            return strLocationStatus;
        }

    }
}
