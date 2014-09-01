using Microsoft.BizTalk.ExplorerOM;
using Microsoft.BizTalk.Operations;
using Microsoft.BizTalk.Message;
using Microsoft.BizTalk.Message.Interop;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Data.SqlClient;

namespace BCC.Core
{
    /// <summary>
    /// Updated by Naveen on 9:14 AM 5/13/2010
    /// </summary>
    public class BCCDataAccess
    {
        private BtsCatalogExplorer bceExplorer;
        private string bizTalkMgmtDbConnectionString = string.Empty;

        public BCCDataAccess()
        {
            bizTalkMgmtDbConnectionString = ConfigurationManager.ConnectionStrings["bizTalk"].ConnectionString;
        }

        public void StartService(string pserviceName)
        {
            ServiceController controller = null;

            try
            {
                controller = new ServiceController();
                controller.MachineName = Environment.MachineName;
                controller.ServiceName = pserviceName;
                               
                if ((controller.Status.Equals(ServiceControllerStatus.Stopped)) 
                    || (controller.Status.Equals(ServiceControllerStatus.StopPending)))
                {
                    controller.Start();
                    controller.WaitForStatus(ServiceControllerStatus.Running);
                }
                controller.Refresh();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void StopService(string pserviceName)
        {
            ServiceController controller = null;

            try
            {
                controller = new ServiceController();
                controller.MachineName = Environment.MachineName;
                controller.ServiceName = pserviceName;

                // Stop the service
                if (controller.Status.Equals(ServiceControllerStatus.Running))
                {
                    if (controller.CanStop)
                    {
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.StopPending);
                    }
                }
                controller.Refresh();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void ResumeMessage(string instanceID)
        {
            BizTalkOperations btOperations = new BizTalkOperations();
            btOperations.ResumeInstance(new Guid(instanceID));
        }
        public void TerminateMessage(string instanceID)
        {
            BizTalkOperations btOperations = new BizTalkOperations();
            btOperations.TerminateInstance(new Guid(instanceID));
        }

        public DataTable DatabaseCheck(StringCollection connectionStringList)
        {
            DataTable dt = new DataTable();
            DataRow dr = null;

            // define the table's schema
            dt.Columns.Add(new DataColumn("ServerName", typeof(string)));
            dt.Columns.Add(new DataColumn("DBName", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));

            SqlConnection sqlConnection = null;
            
            foreach (string connectionString in connectionStringList)
            {
                try
                {
                    sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();

                    dr = dt.NewRow();
                    dr[0] = sqlConnection.DataSource;
                    dr[1] = sqlConnection.Database;

                    if (sqlConnection.State == ConnectionState.Open)
                    {
                        dr[2] = "Enabled";
                    }
                    
                    dt.Rows.Add(dr);
                }
                catch
                {
                    dr = dt.NewRow();
                    dr[0] = sqlConnection.DataSource;
                    dr[1] = sqlConnection.Database;

                    if (sqlConnection.State != ConnectionState.Open)
                    {
                        dr[2] = "Unknown";
                    }

                    dt.Rows.Add(dr);
                }
                finally
                {
                    if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                    {
                        sqlConnection.Close();
                    }
                }
            }

            return dt;
        }

        public ArrayList GetParameterList(string inParamFlag)
        {
            return null;
        }

        public ArrayList GetLogMachineNames()
        {
            return new ArrayList();
        }

        public string GetBiztalkMessage(string messageId)
        {
            string messageBody = string.Empty;
            // Converts the Collection for Connection String
            // NameValueCollection collection = ConvertStringToNameValueCollection(ConfigurationManager.AppSettings["BCC.BIZTALK_CONNECTION"]);
            BizTalkOperations operations = new BizTalkOperations();
            IEnumerable messages = operations.GetMessages();
            try
            {
                foreach (BizTalkMessage message in messages)
                {
                    if (messageId.Equals(message.MessageID.ToString()))
                    {
                        using (StreamReader streamReader = new StreamReader(message.BodyPart.Data))
                        {
                            messageBody = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            if (messageBody != null && messageBody.Length > 0)
            {
                return messageBody;
            }
            else
            {
                return "No message found for " + messageId;
            }
        }

        public BizTalkMessage RetrieveBiztalkMessage(string messageId)
        {
            // Converts the Collection for Connection String
            // NameValueCollection collection = ConvertStringToNameValueCollection(ConfigurationManager.AppSettings["BCC.BIZTALK_CONNECTION"]);
            BizTalkOperations operations = new BizTalkOperations();
            IEnumerable messages = operations.GetMessages();
            try
            {
                foreach (BizTalkMessage message in messages)
                {
                    if (messageId.Equals(message.MessageID.ToString()))
                    {
                        return message;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return null;
        }

        private string ConvertBooleanToEnglish(string flag)
        {
            string result = string.Empty;

            switch (flag)
            {
                case "True": result = "Yes"; break;
                case "False": result = "No"; break;
            }

            return result;
        }

        private string ConvertHostStatus(string serviceStatus)
        {
            string result = string.Empty;

            switch (serviceStatus)
            {
                case "1": result =  "Stopped"; break;
                case "2": result =  "Start pending"; break;
                case "3": result =  "Stop pending"; break;
                case "4": result =  "Running"; break;
                case "5": result =  "Continue pending"; break;
                case "6": result =  "Pause pending"; break;
                case "7": result =  "Paused"; break;
                case "8": result =  "Unknown"; break;
            }

            return result;
        }

        public DataTable RetrieveAllHostsAndInstance(StringCollection hostInstanceList, bool isFilterEnabled)
        {
            DataTable dt = new DataTable();
            // define the table's schema
            dt.Columns.Add(new DataColumn("MachineName", typeof(string)));
            dt.Columns.Add(new DataColumn("HostName", typeof(string)));
            dt.Columns.Add(new DataColumn("HostType", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));
            dt.Columns.Add(new DataColumn("IsDisabled", typeof(string)));

            try
            {
                DataRow dr = null;
                string hostName = string.Empty;
                string status = string.Empty;

                ManagementClass objHostSettingClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_HostInstance", null);

                foreach (ManagementObject objShare in objHostSettingClass.GetInstances())
                {
                   hostName = objShare.Properties["HostName"].Value.ToString();

                   dr = dt.NewRow();

                   dr["MachineName"] = objShare.Properties["RunningServer"].Value.ToString();
                   dr["HostName"] = hostName;

                   status = objShare.Properties["ServiceState"].Value.ToString();
                   dr["Status"] = ConvertHostStatus(status);

                   status = objShare.Properties["IsDisabled"].Value.ToString();
                   dr["IsDisabled"] = ConvertBooleanToEnglish(status);

                   status = objShare.Properties["HostType"].Value.ToString();
                   dr["HostType"] = ("1".Equals(status) ? "In-Process" : "Isolated");

                   // Adds only what is present in the filter. 
                   if (hostInstanceList != null && hostInstanceList.Contains(hostName) && isFilterEnabled)
                   {
                       dt.Rows.Add(dr);
                   }

                   // Ignores the filter.
                   if (!isFilterEnabled)
                   {
                       dt.Rows.Add(dr);
                   }
                   
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        private NameValueCollection ConvertStringToNameValueCollection(string stringText, char delimiter)
        {
            NameValueCollection nameValue = new NameValueCollection();
            string[] aryStrings = stringText.Split(delimiter);
            string[] nameAndValue;

            foreach (string s in aryStrings)
            {
                nameAndValue = s.Split('=');
                nameValue.Add(nameAndValue[0], nameAndValue[1]);
            }
            return nameValue;
        }

        private NameValueCollection ConvertStringToNameValueCollection(string stringText)
        {
            NameValueCollection nameValue = new NameValueCollection();
            string[] aryStrings = stringText.Split(';');
            string[] nameAndValue;

            foreach (string s in aryStrings)
            {
                nameAndValue = s.Split('=');
                nameValue.Add(nameAndValue[0], nameAndValue[1]);
            }
            return nameValue;
        }

        public DataTable RetrieveAllMessages(int maxLimit)
        {
            DataTable dt = new DataTable();
            BizTalkOperations btOperations = new BizTalkOperations();
            int counter = 0;

            dt.Columns.Add(new DataColumn("MessageID", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));
            dt.Columns.Add(new DataColumn("InstanceID", typeof(string)));
            dt.Columns.Add(new DataColumn("CreationTime", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("Application", typeof(string)));
            dt.Columns.Add(new DataColumn("HostName", typeof(string)));

            if (btOperations != null)
            {
                // Loop and get the service instances
                foreach (MessageBoxServiceInstance serviceInstance in btOperations.GetServiceInstances())
                {
                    foreach (BizTalkMessage msg in serviceInstance.Messages)
                    {
                        if (counter < maxLimit)
                        {
                            DataRow dr = dt.NewRow();
                            dr[0] = msg.MessageID;
                            dr[1] = msg.InstanceStatus;
                            dr[2] = msg.InstanceID.ToString();
                            dr[3] = msg.CreationTime.ToString();
                            dr[4] = serviceInstance.Application;
                            dr[5] = msg.HostName;
                            dt.Rows.Add(dr);
                            counter = counter + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return dt;
        }

        public DataTable RetrieveAllMessages(string messageStatus, StringCollection applicationList, int maxLimit)
        {
            DataTable dt = new DataTable();
            BizTalkOperations btOperations = new BizTalkOperations();
            int messageCounter = 0;

            //// define the table's schema
            dt.Columns.Add(new DataColumn("MessageID", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));
            dt.Columns.Add(new DataColumn("InstanceID", typeof(string)));
            dt.Columns.Add(new DataColumn("CreationTime", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("Application", typeof(string)));
            dt.Columns.Add(new DataColumn("HostName", typeof(string)));

            if (btOperations != null)
            {
                // Loop and get the service instances
                foreach (MessageBoxServiceInstance serviceInstance in btOperations.GetServiceInstances())
                {
                    if (serviceInstance.Application != null && (applicationList.Contains(serviceInstance.Application)))
                    {
                        foreach (BizTalkMessage msg in serviceInstance.Messages)
                        {
                            if (messageCounter < maxLimit)
                            {
                                DataRow dr = dt.NewRow();
                                dr[0] = msg.MessageID;
                                dr[1] = msg.InstanceStatus;
                                dr[2] = msg.InstanceID.ToString();
                                dr[3] = msg.CreationTime.ToString();
                                dr[4] = serviceInstance.Application;
                                dr[5] = msg.HostName;
                                dt.Rows.Add(dr);
                                messageCounter++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return dt;
        }

        public BtsOrchestration GetOrchestrationByName(string orchestrationName)
        {
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            BtsAssemblyCollection btsAssemblyCollection = bceExplorer.Assemblies;

            BizTalkOperations btOperations = new BizTalkOperations();
            try
            {
                foreach (BtsAssembly btsAssembly in btsAssemblyCollection)
                {
                    foreach (BtsOrchestration btsOrchestration in btsAssembly.Orchestrations)
                    {
                        // Show only HUB related orchestrations.
                        if (orchestrationName.Equals(btsOrchestration.FullName))
                        {
                            return btsOrchestration;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return null;
        }

        public BtsOrchestration GetOrchestrationByName(string orchestrationName, string applicationName)
        {
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            BtsAssemblyCollection btsAssemblyCollection = bceExplorer.Assemblies;

            BizTalkOperations btOperations = new BizTalkOperations();
            try
            {
                foreach (BtsAssembly btsAssembly in btsAssemblyCollection)
                {
                    foreach (BtsOrchestration btsOrchestration in btsAssembly.Orchestrations)
                    {
                        // Show only HUB related orchestrations.
                        if ((applicationName.ToUpper()).Equals(btsOrchestration.BtsAssembly.Application.Name.ToUpper())
                            && orchestrationName.Equals(btsOrchestration.FullName))
                        {
                            return btsOrchestration;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return null;
        }

        private void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(message, System.Reflection.Assembly.GetExecutingAssembly().FullName);
        }

        public StringCollection RetrieveAllApplications(StringCollection applicationList)
        {
            StringCollection appCollection = null;
            bceExplorer = new BtsCatalogExplorer();
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            ApplicationCollection btsAppCollection = bceExplorer.Applications;

            try
            {
                appCollection = new StringCollection();

                foreach (Application application in btsAppCollection)
                {
                    if (applicationList.Contains(application.Name))
                    {
                        appCollection.Add(application.Name);
                    }
                }
            }
            catch (Exception e)
            {
                Debug(e.Message);
            }

            return appCollection;
        }

        public DataTable RetrieveAllAdapters()
        {
            DataTable dt = new DataTable();
            DataRow dr = null;
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;

            // define the table's schema
            dt.Columns.Add(new DataColumn("ProtocolName", typeof(string)));
            dt.Columns.Add(new DataColumn("DefaultSendHandler", typeof(string)));
            dt.Columns.Add(new DataColumn("Capabilities", typeof(string)));

            ProtocolTypeCollection protocolTypeCollection = bceExplorer.ProtocolTypes;
            SendHandler defaultHandler = null;
            
            foreach (ProtocolType protocolType in protocolTypeCollection)
            {
                dr = dt.NewRow();
                dr[0] = protocolType.Name;
                defaultHandler = protocolType.DefaultSendHandler;
                
                if (defaultHandler != null)
                {
                    dr[1] = protocolType.DefaultSendHandler.Name;
                }
                else
                {
                    dr[1] = "none";
                }

                Capabilities ability = protocolType.Capabilities;
                dr[2] = ability.ToString();
               
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public StringCollection RetrieveAllApplications()
        {
            StringCollection appCollection = null;
            bceExplorer = new BtsCatalogExplorer();
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            ApplicationCollection btsAppCollection = bceExplorer.Applications;

            try
            {
                appCollection = new StringCollection();

                foreach (Application application in btsAppCollection)
                {
                    appCollection.Add(application.Name);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e.Message + e.StackTrace, "Retrieve-All-Applications");
            }

            return appCollection;
        }

        public DataTable EnumerateAllArtifacts()
        {
            DataTable dt = new DataTable();
            DataRow dr = null;
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            BtsAssemblyCollection btsAssemblyCollection = bceExplorer.Assemblies;
            BizTalkOperations btOperations = new BizTalkOperations();

            // define the table's schema
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));
            dt.Columns.Add(new DataColumn("Data", typeof(string)));

            try
            {
                // Enumerate all orchestrations
                foreach (BtsAssembly btsAssembly in btsAssemblyCollection)
                {
                        dr = dt.NewRow();
                        dr["Name"] = btsAssembly.Name;
                        dr["Status"] = "";
                        dr["Data"] = "Assembly:" + btsAssembly.DisplayName + ":" + btsAssembly.Version + ":" + btsAssembly.PublicKeyToken;
                        dt.Rows.Add(dr);

                    foreach (BtsOrchestration btsOrchestration in btsAssembly.Orchestrations)
                    {
                        // create the rows
                        dr = dt.NewRow();
                        dr["Name"] = btsOrchestration.FullName;
                        dr["Status"] = btsOrchestration.Status.ToString();
                        dr["Data"] = "Orchestration:" + btsOrchestration.BtsAssembly.Name + ":v" + btsOrchestration.BtsAssembly.Version + ":" + btsOrchestration.BtsAssembly.PublicKeyToken;
                        dt.Rows.Add(dr);

                        // Enumerate ports and operations
                        foreach (OrchestrationPort port in btsOrchestration.Ports)
                        {
                            dr = dt.NewRow();
                            dr["Name"] = port.Name;
                            dr["Status"] = "";
                            dr["Data"] = "Port Type:" + port.PortType.FullName + ":" + port.PortType.AssemblyQualifiedName;
                            dt.Rows.Add(dr);

                            foreach (PortTypeOperation operation in port.PortType.Operations)
                            {
                                dr = dt.NewRow();
                                dr["Name"] = operation.Name;
                                dr["Status"] = "";
                                dr["Data"] = "PortTypeOperation:" + operation.Type;
                                dt.Rows.Add(dr);
                            }
                        }

                        // Enumerate used roles
                        foreach (Role role in btsOrchestration.UsedRoles)
                        {
                            dr = dt.NewRow();
                            dr["Name"] = role.Name;
                            dr["Status"] = "";
                            dr["Data"] = "ServiceLinkType:" + role.ServiceLinkType;
                            dt.Rows.Add(dr);

                            foreach (EnlistedParty enlistedparty in role.EnlistedParties)
                            {
                                dr = dt.NewRow();
                                dr["Name"] = enlistedparty.Party.Name;
                                dr["Status"] = "";
                                dr["Data"] = "Data:" + enlistedparty.Party.CustomData;
                                dt.Rows.Add(dr);
                            }
                        }
                    }
                }

                foreach (SendPort sp in bceExplorer.SendPorts)
                {
                    dr = dt.NewRow();
                    dr["Name"] = sp.Name;
                    dr["Status"] = sp.Status.ToString();
                    
                    if (sp.PrimaryTransport != null)
                    {
                       dr["Data"] = "Send Port:" + sp.PrimaryTransport.Address + ":" + sp.PrimaryTransport.TransportType.Name;
                    }
                    dt.Rows.Add(dr);
                }

                foreach (ReceivePort rp in bceExplorer.ReceivePorts)
                {
                    foreach (ReceiveLocation rl in rp.ReceiveLocations)
                    {
                        // create the rows
                        dr = dt.NewRow();
                        dr["Name"] = rp.Name;
                        dr["Status"] = (rl.Enable ? "Enabled" : "Disabled");
                        dr["Data"] = "Receive Location:" + rl.Name;
                        dt.Rows.Add(dr);
                    }
                }

                // Host instances
                ManagementClass objHostSettingClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_HostInstance", null);
                foreach (ManagementObject objShare in objHostSettingClass.GetInstances())
                {
                    dr = dt.NewRow();
                    dr["Name"] = objShare.Properties["RunningServer"].Value.ToString();
                    string status = objShare.Properties["ServiceState"].Value.ToString();
                    dr["Status"] = ("4".Equals(status) ? "Running" : "Stopped");
                    dr["Data"] = "Host:" + objShare.Properties["HostName"].Value.ToString();   
                    dt.Rows.Add(dr);
                }

                if (btOperations != null)
                {
                    // Loop and get the service instances
                    foreach (MessageBoxServiceInstance serviceInstance in btOperations.GetServiceInstances())
                    {
                        if (serviceInstance.Application != null )
                        {
                            foreach (BizTalkMessage msg in serviceInstance.Messages)
                            {
                                dr = dt.NewRow();
                                dr["Name"] = serviceInstance.Application.ToUpper();
                                dr["Status"] = msg.InstanceStatus;
                                dr["Data"] = "Message:" + msg.MessageID + ":" + msg.InstanceID.ToString() + ":" + msg.CreationTime.ToString();
                                dt.Rows.Add(dr);
                            }
                        }
                    }
                }

                foreach (Transform transform in bceExplorer.Transforms)
                {
                    dr = dt.NewRow();
                    dr["Name"] = transform.FullName;
                    dr["Status"] = "";
                    dr["Data"] = "Map:" + transform.SourceSchema.FullName + ":" + transform.TargetSchema.FullName;
                    dt.Rows.Add(dr);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return dt;
        }

        public DataTable RetrieveArtifactHandlers(string artifactType)
        {
            ArrayList filter = new ArrayList();
            DataTable dt = new DataTable();
            DataRow dr = null;
            bceExplorer = new BtsCatalogExplorer();
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            int rowCount = 0;
            // define the table's schema
            dt.Columns.Add(new DataColumn("Id", typeof(string)));
            dt.Columns.Add(new DataColumn("HandlerName", typeof(string)));

            if (artifactType == BCCUIHelper.Constants.ARTIFACT_SND)
            {
                foreach(SendHandler sendHandler in bceExplorer.SendHandlers)
                {
                    if (filter.Contains(sendHandler.Name))
                    {
                        continue;
                    }
                    else
                    {
                        // create the rows
                        dr = dt.NewRow();
                        dr["Id"] = rowCount;
                        dr["HandlerName"] = sendHandler.Name;
                        dt.Rows.Add(dr);
                        rowCount = rowCount + 1;
                        filter.Add(sendHandler.Name);
                    }
                }
            }
            else if (artifactType == BCCUIHelper.Constants.ARTIFACT_RCV)
            {
                foreach (ReceiveHandler rcvHandler in bceExplorer.ReceiveHandlers)
                {
                    if (filter.Contains(rcvHandler.Name))
                    {
                        continue;
                    }
                    else
                    {
                        // create the rows
                        dr = dt.NewRow();
                        dr["Id"] = rowCount;
                        dr["HandlerName"] = rcvHandler.Name;
                        dt.Rows.Add(dr);
                        rowCount = rowCount + 1;
                        filter.Add(rcvHandler.Name);
                    }
                }
            }
            else if (artifactType == BCCUIHelper.Constants.ARTIFACT_ODX)
            {
                foreach (Host host in bceExplorer.Hosts)
                {
                    if (filter.Contains(host.Name))
                    {
                        continue;
                    }
                    else
                    {
                        // create the rows
                        dr = dt.NewRow();
                        dr["Id"] = rowCount;
                        dr["HandlerName"] = host.Name;
                        dt.Rows.Add(dr);
                        rowCount = rowCount + 1;
                        filter.Add(host.Name);
                    }
                }
            }

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="oCount">Orchestration count</param>
        /// <param name="rCount">Receive location count</param>
        /// <param name="sCount">Send Port count</param>
        /// <param name="oaHosts">Orchestration unique hosts</param>
        /// <param name="raHosts">Receive port hosts</param>
        /// <param name="saHosts">Send port hosts</param>
        /// <returns></returns>
        public DataTable EnumerateAllArtifacts(string applicationName, 
            out int oCount, out int rCount, out int sCount,
            out int oaHosts, out int raHosts, out int saHosts)
        {
            DataTable dt = new DataTable();
            DataRow dr = null;
            bceExplorer = new BtsCatalogExplorer();
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            oaHosts = 0; raHosts = 0; saHosts = 0;
            oCount = 0; rCount = 0; sCount = 0;
            int rowCounter = 0;
            // define the table's schema
            dt.Columns.Add(new DataColumn("ArtifactType", typeof(string)));
            dt.Columns.Add(new DataColumn("ArtifactName", typeof(string)));
            dt.Columns.Add(new DataColumn("ArtifactHost", typeof(string)));
            dt.Columns.Add(new DataColumn("ID", typeof(string)));

            try
            {
                ArrayList namedArray = null;
                string hostName = string.Empty;

                foreach (Application application in bceExplorer.Applications)
                {
                    if (application.Name.Equals(applicationName))
                    {
                        namedArray = new ArrayList();

                        foreach (BtsOrchestration btsOrchestration in application.Orchestrations)
                        {
                            // create the rows
                            dr = dt.NewRow();
                            dr["ArtifactType"] = BCCUIHelper.Constants.ARTIFACT_ODX;
                            dr["ArtifactName"] = btsOrchestration.FullName;

                            if (btsOrchestration.Host != null)
                            {
                                hostName = btsOrchestration.Host.Name;
                            }
                            else
                            {
                                hostName = "(unbound)";
                            }

                            dr["ArtifactHost"] = hostName;
                            dr["ID"] = rowCounter.ToString();
                            
                            if (!namedArray.Contains(hostName))
                            {
                                namedArray.Add(hostName);
                            }
                                                        
                            dt.Rows.Add(dr);
                            oCount = oCount + 1;
                            rowCounter = rowCounter + 1;
                        }

                        oaHosts = namedArray.Count;
                        namedArray = new ArrayList();

                        foreach (SendPort sp in application.SendPorts)
                        {
                            dr = dt.NewRow();
                            dr["ArtifactType"] = BCCUIHelper.Constants.ARTIFACT_SND;
                            dr["ArtifactName"] = sp.Name;

                            if (sp.PrimaryTransport != null && sp.PrimaryTransport.SendHandler != null)
                            {
                                hostName = sp.PrimaryTransport.SendHandler.Name;
                                dr["ArtifactHost"] = hostName;

                                if (!namedArray.Contains(hostName))
                                {
                                    namedArray.Add(hostName);
                                }

                                sCount = sCount + 1;
                            }
                            else
                            {
                                dr["ArtifactHost"] = "&lt;empty&gt;";
                            }

                            dr["ID"] = rowCounter.ToString();
                            dt.Rows.Add(dr);
                            rowCounter = rowCounter + 1;
                        }

                        saHosts = namedArray.Count;
                        namedArray = new ArrayList();

                        foreach (ReceivePort rp in application.ReceivePorts)
                        { 
                           foreach (ReceiveLocation rl in rp.ReceiveLocations)
                            {
                                // create the rows
                                dr = dt.NewRow();
                                dr["ArtifactType"] = BCCUIHelper.Constants.ARTIFACT_RCV;
                                dr["ArtifactName"] = rl.Name;

                                if (rl.ReceiveHandler.Host != null)
                                {
                                    hostName = rl.ReceiveHandler.Host.Name;
                                }
                                else
                                {
                                    hostName = "&lt;empty&gt;";
                                }

                                dr["ArtifactHost"] = hostName;

                                if (!namedArray.Contains(hostName))
                                {
                                    namedArray.Add(hostName);
                                }
                                
                                dr["ID"] = rowCounter.ToString();
                                dt.Rows.Add(dr);
                                rCount = rCount + 1;
                                rowCounter = rowCounter + 1;
                            }
                        }

                        raHosts = namedArray.Count;
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return dt;
        }

        public string RetrieveDefaultBizTalkApplication()
        {
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;

            string defaultAppName = string.Empty;

            ApplicationCollection appCollection = bceExplorer.Applications;

            foreach (Application app in appCollection)
            {
                if (app.IsDefaultApplication)
                {
                    defaultAppName = app.Name;
                    break;
                }
            }

            return defaultAppName;
        }

        public DataTable RetrieveAllOrchestrations()
        {
            DataTable dt = new DataTable();
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            BtsAssemblyCollection btsAssemblyCollection = bceExplorer.Assemblies;

            // define the table's schema
            dt.Columns.Add(new DataColumn("HDName", typeof(string)));
            dt.Columns.Add(new DataColumn("Application", typeof(string)));
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));
            dt.Columns.Add(new DataColumn("AssemblyName", typeof(string)));
            dt.Columns.Add(new DataColumn("Version", typeof(string)));
            dt.Columns.Add(new DataColumn("Token", typeof(string)));

            try
            {
                foreach (BtsAssembly btsAssembly in btsAssemblyCollection)
                {
                    Host host = null;

                    foreach (BtsOrchestration btsOrchestration in btsAssembly.Orchestrations)
                    {
                        // Create the rows
                        DataRow dr = dt.NewRow();
                        dr["HDName"] = btsOrchestration.FullName;
                        dr["Application"] = btsOrchestration.Application.Name;
                        dr["Name"] = btsOrchestration.FullName;

                        host = btsOrchestration.Host;

                        if (host != null)
                        {
                            dr["Status"] = btsOrchestration.Status.ToString();
                        }
                        else
                        {
                            dr["Status"] = btsOrchestration.Status.ToString() + " (unbound)";
                        }
                        
                        dr["AssemblyName"] = btsOrchestration.BtsAssembly.Name;
                        dr["Version"] = btsOrchestration.BtsAssembly.Version;
                        dr["Token"] = btsOrchestration.BtsAssembly.PublicKeyToken;
                        
                        dt.Rows.Add(dr);

                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        private bool AreAllPortsBound(BtsOrchestration oInstance)
        {
            bool portBoundFlag = true;

            foreach(OrchestrationPort port in oInstance.Ports)
            {
                if (port.ReceivePort == null && port.SendPort == null && port.SendPortGroup == null)
                {
                    portBoundFlag = false;
                    break;
                }
            }

            return portBoundFlag;
        }


        public DataTable RetrieveAllOrchestrations(StringCollection applicationList)
        {
            DataTable dt = new DataTable();
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            ApplicationCollection appCollection = bceExplorer.Applications;

            // define the table's schema
            dt.Columns.Add(new DataColumn("HDName", typeof(string)));
            dt.Columns.Add(new DataColumn("Application", typeof(string)));
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));
            dt.Columns.Add(new DataColumn("AssemblyName", typeof(string)));
            dt.Columns.Add(new DataColumn("Version", typeof(string)));
            dt.Columns.Add(new DataColumn("Token", typeof(string)));

            try
            {
                string lowercaseApplication = string.Empty;

                foreach (Application application in appCollection)
                {
                    if (applicationList.Contains(application.Name))
                    {
                        Host host = null;

                        foreach (BtsOrchestration btsOrchestration in bceExplorer.Applications[application.Name].Orchestrations)
                        {
                            //// Create the rows
                            DataRow dr = dt.NewRow();
                            dr["HDName"] = btsOrchestration.FullName;
                            dr["Application"] = btsOrchestration.Application.Name;
                            dr["Name"] = btsOrchestration.FullName;

                            host = btsOrchestration.Host;

                            if (host != null && AreAllPortsBound(btsOrchestration) )
                            {
                                dr["Status"] = btsOrchestration.Status.ToString();
                            }
                            else
                            {
                                dr["Status"] = btsOrchestration.Status.ToString() + " (unbound)";
                            }

                            dr["AssemblyName"] = btsOrchestration.BtsAssembly.Name;
                            dr["Version"] = btsOrchestration.BtsAssembly.Version;
                            dr["Token"] = btsOrchestration.BtsAssembly.PublicKeyToken;
                            dt.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        public DataTable RetrieveAllPipelines(StringCollection applicationList)
        {
            DataTable dt = new DataTable();
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            // define the table's schema
            dt.Columns.Add(new DataColumn("Application", typeof(string)));
            dt.Columns.Add(new DataColumn("Assembly", typeof(string)));
            dt.Columns.Add(new DataColumn("FullName", typeof(string)));
            dt.Columns.Add(new DataColumn("Type", typeof(string)));

            try
            {
                DataRow dr = null;

                foreach (Pipeline pipeline in bceExplorer.Pipelines)
                {
                    if (applicationList.Contains(pipeline.Application.Name))
                    {
                        // create the rows
                        dr = dt.NewRow();
                        dr["Application"] = pipeline.Application.Name;
                        dr["Assembly"] = pipeline.BtsAssembly.Name;
                        dr["FullName"] = pipeline.FullName;
                        dr["Type"] = pipeline.Type.ToString();

                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        public DataTable RetrieveAllPipelines()
        {
            DataTable dt = new DataTable();
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            // define the table's schema
            dt.Columns.Add(new DataColumn("Application", typeof(string)));
            dt.Columns.Add(new DataColumn("Assembly", typeof(string)));
            dt.Columns.Add(new DataColumn("FullName", typeof(string)));
            dt.Columns.Add(new DataColumn("Type", typeof(string)));

            try
            {
                DataRow dr = null;

                foreach (Pipeline pipeline in bceExplorer.Pipelines)
                {
                    // create the rows
                    dr = dt.NewRow();
                    dr["Application"] = pipeline.Application.Name;
                    dr["Assembly"] = pipeline.BtsAssembly.Name;
                    dr["FullName"] = pipeline.FullName;
                    dr["Type"] = pipeline.Type.ToString();

                    dt.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataTable RetrieveAllSchemas()
        {
            DataTable dt = new DataTable();
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            // define the table's schema
            dt.Columns.Add(new DataColumn("SchemaName", typeof(string)));
            dt.Columns.Add(new DataColumn("MessageType", typeof(string)));
            dt.Columns.Add(new DataColumn("SchemaType", typeof(string)));
            dt.Columns.Add(new DataColumn("Assembly", typeof(string)));
            dt.Columns.Add(new DataColumn("Application", typeof(string)));

            try
            {
                DataRow dr = null;

                foreach (Schema schema in bceExplorer.Schemas)
                {

                    // create the rows
                    dr = dt.NewRow();
                    dr["SchemaName"] = schema.FullName;
                    dr["MessageType"] = schema.TargetNameSpace + "#" + schema.RootName;
                    dr["SchemaType"] = schema.Type.ToString();
                    dr["Assembly"] = schema.BtsAssembly.Name;
                    dr["Application"] = schema.Application.Name;

                    dt.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataTable RetrieveAllSchemas(StringCollection applicationList)
        {
            DataTable dt = new DataTable();
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            // define the table's schema
            dt.Columns.Add(new DataColumn("SchemaName", typeof(string)));
            dt.Columns.Add(new DataColumn("MessageType", typeof(string)));
            dt.Columns.Add(new DataColumn("SchemaType", typeof(string)));
            dt.Columns.Add(new DataColumn("Assembly", typeof(string)));
            dt.Columns.Add(new DataColumn("Application", typeof(string)));

            try
            {
                DataRow dr = null;

                foreach (Schema schema in bceExplorer.Schemas)
                {
                    if (applicationList.Contains(schema.Application.Name))
                    {
                        // create the rows
                        dr = dt.NewRow();
                        dr[0] = schema.FullName;
                        dr[1] = schema.TargetNameSpace + "#" + schema.RootName;
                        dr[2] = schema.Type.ToString();
                        dr[3] = schema.BtsAssembly.Name;
                        dr[4] = schema.Application.Name;

                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        public DataTable RetrieveAllMaps()
        {
            DataTable dt = new DataTable();
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            // define the table's schema
            dt.Columns.Add(new DataColumn("SourceSchema", typeof(string)));
            dt.Columns.Add(new DataColumn("SourceSchemaRoot", typeof(string)));
            dt.Columns.Add(new DataColumn("MapName", typeof(string)));
            dt.Columns.Add(new DataColumn("TargetSchema", typeof(string)));
            dt.Columns.Add(new DataColumn("TargetSchemaRoot", typeof(string)));
            dt.Columns.Add(new DataColumn("Assembly", typeof(string)));
            dt.Columns.Add(new DataColumn("Application", typeof(string)));

            try
            {
                DataRow dr = null;

                foreach (Transform map in bceExplorer.Transforms)
                {
                    // create the rows
                    dr = dt.NewRow();
                    dr["SourceSchema"] = map.SourceSchema.FullName;
                    dr["SourceSchemaRoot"] = map.SourceSchema.RootName;
                    dr["TargetSchema"] = map.TargetSchema.FullName;
                    dr["TargetSchemaRoot"] = map.TargetSchema.RootName;
                    
                    dr["MapName"] = map.FullName;
                    dr["Assembly"] = map.BtsAssembly.Name;
                    dr["Application"] = map.Application.Name;

                    dt.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        public DataTable RetrieveAllMaps(StringCollection applicationList)
        {
            DataTable dt = new DataTable();
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            // define the table's schema
            dt.Columns.Add(new DataColumn("SourceSchema", typeof(string)));
            dt.Columns.Add(new DataColumn("SourceSchemaRoot", typeof(string)));
            dt.Columns.Add(new DataColumn("MapName", typeof(string)));
            dt.Columns.Add(new DataColumn("TargetSchema", typeof(string)));
            dt.Columns.Add(new DataColumn("TargetSchemaRoot", typeof(string)));
            dt.Columns.Add(new DataColumn("Assembly", typeof(string)));
            dt.Columns.Add(new DataColumn("Application", typeof(string)));

            try
            {
                DataRow dr = null;

                foreach (Transform map in bceExplorer.Transforms)
                {
                    if (applicationList.Contains(map.Application.Name))
                    {
                        // create the rows
                        dr = dt.NewRow();
                        dr["SourceSchema"] = map.SourceSchema.FullName;
                        dr["SourceSchemaRoot"] = map.SourceSchema.RootName;
                        dr["TargetSchema"] = map.TargetSchema.FullName;
                        dr["TargetSchemaRoot"] = map.TargetSchema.RootName;
                        dr["MapName"] = map.FullName;
                        dr["Assembly"] = map.BtsAssembly.Name;
                        dr["Application"] = map.Application.Name;

                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }


        public DataTable RetrieveAllSendPorts()
        {
            DataTable dt = new DataTable();
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            // define the table's schema
            dt.Columns.Add(new DataColumn("SendPortName", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));
            dt.Columns.Add(new DataColumn("URI", typeof(string)));
            dt.Columns.Add(new DataColumn("TransportType", typeof(string)));
            dt.Columns.Add(new DataColumn("Application", typeof(string)));
            dt.Columns.Add(new DataColumn("PortType", typeof(string)));

            try
            {
                foreach (SendPort sp in bceExplorer.SendPorts)
                {
                    // create the rows
                    DataRow dr = dt.NewRow();
                    dr["SendPortName"] = sp.Name;
                    dr["Status"] = sp.Status.ToString();
                    dr["Application"] = sp.Application.Name;

                    if(sp.IsDynamic && sp.IsTwoWay)
                    {
                        dr["PortType"] = "Dynamic Solicit-Response";
                    }
                    else if (!sp.IsDynamic && sp.IsTwoWay)
                    {
                        dr["PortType"] = "Static Solicit-Response";
                    }
                    else if (sp.IsDynamic && !sp.IsTwoWay)
                    {
                        dr["PortType"] = "Dynamic One-way";
                    }
                    else 
                    {
                        dr["PortType"] = "Static One-way";
                    }

                    if (sp.PrimaryTransport != null)
                    {
                        dr["URI"] = sp.PrimaryTransport.Address;
                        dr["TransportType"] = sp.PrimaryTransport.TransportType.Name;
                    }
                    dt.Rows.Add(dr);
                }

                dt.DefaultView.Sort = "SendPortName ASC";
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        public DataTable RetrieveAllSendPorts(StringCollection applicationList)
        {
            DataTable dt = new DataTable();
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            // define the table's schema
            dt.Columns.Add(new DataColumn("SendPortName", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));
            dt.Columns.Add(new DataColumn("URI", typeof(string)));
            dt.Columns.Add(new DataColumn("TransportType", typeof(string)));
            dt.Columns.Add(new DataColumn("Application", typeof(string)));
            dt.Columns.Add(new DataColumn("PortType", typeof(string)));

            try
            {
                DataRow dr = null;

                foreach (SendPort sp in bceExplorer.SendPorts)
                {
                    if (applicationList.Contains(sp.Application.Name))
                    {
                        // create the rows
                        dr = dt.NewRow();
                        dr["SendPortName"] = sp.Name;
                        dr["Status"] = sp.Status.ToString();
                        dr["Application"] = sp.Application.Name;

                        if (sp.IsDynamic && sp.IsTwoWay)
                        {
                            dr["PortType"] = "Dynamic Solicit-Response";
                        }
                        else if (!sp.IsDynamic && sp.IsTwoWay)
                        {
                            dr["PortType"] = "Static Solicit-Response";
                        }
                        else if (sp.IsDynamic && !sp.IsTwoWay)
                        {
                            dr["PortType"] = "Dynamic One-way";
                        }
                        else
                        {
                            dr["PortType"] = "Static One-way";
                        }

                        if (sp.PrimaryTransport != null)
                        {
                            dr["URI"] = sp.PrimaryTransport.Address;
                            dr["TransportType"] = sp.PrimaryTransport.TransportType.Name;
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        private static void GetAllHostInstances()
        {
            //BtsCatalogExplorer btsCatExp = new BtsCatalogExplorer();
            ManagementClass objHostSettingClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_HostInstance", null);
            foreach (ManagementObject objShare in objHostSettingClass.GetInstances())
            {
                Console.WriteLine("MSBTS_HostInstance Name = " + objShare.Properties["Name"].Value);
                foreach (PropertyData dataProp in objShare.Properties)
                {
                    Console.WriteLine("PropName = " + dataProp.Name + "-" + dataProp.Value);
                }
            }
        }

        private static void GetAllHosts()
        {
            ManagementClass objHostSettingClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_HostSetting", null);
            foreach (ManagementObject objShare in objHostSettingClass.GetInstances())
            {
                Console.WriteLine("MSBTS_HostSetting Name = " + objShare.Properties["Name"].Value);
                //PropertyDataCollection propColl = objShare.Properties;
                foreach (PropertyData dataProp in objShare.Properties)
                {
                    Console.WriteLine("PropName = " + dataProp.Name + "-" + dataProp.Value);
                }
            }
        }

        public DataTable RetrieveAllReceivePorts(StringCollection applicationList)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("ReceivePortName", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));
            dt.Columns.Add(new DataColumn("ReceivePortLocation", typeof(string)));
            dt.Columns.Add(new DataColumn("Application", typeof(string)));
            dt.Columns.Add(new DataColumn("ReceiveLocationURI", typeof(string)));

            try
            {
                bceExplorer = new BtsCatalogExplorer();
                //Edit the following connection string to point to the correct database and server
                bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;

                DataRow dr = null;

                foreach (ReceivePort rp in bceExplorer.ReceivePorts)
                {
                    if (applicationList.Contains(rp.Application.Name))
                    {
                        foreach (ReceiveLocation rl in rp.ReceiveLocations)
                        {
                            // create the rows
                            dr = dt.NewRow();
                            dr["ReceivePortName"] = rp.Name;
                            dr["Status"] = (rl.Enable ? "Enabled" : "Disabled");
                            dr["ReceivePortLocation"] = rl.Name;
                            dr["ReceiveLocationURI"] = rl.Address;
                            dr["Application"] = rp.Application.Name;
                            dt.Rows.Add(dr);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
            return dt;
        }

        public DataTable RetrieveAllReceivePorts()
        {
            DataTable dt = new DataTable();
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            dt.Columns.Add(new DataColumn("ReceivePortName", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));
            dt.Columns.Add(new DataColumn("ReceivePortLocation", typeof(string)));
            dt.Columns.Add(new DataColumn("Application", typeof(string)));
            dt.Columns.Add(new DataColumn("ReceiveLocationURI", typeof(string)));


            try
            {
                foreach (ReceivePort rp in bceExplorer.ReceivePorts)
                {
                    foreach (ReceiveLocation rl in rp.ReceiveLocations)
                    {
                        // create the rows
                        DataRow dr = dt.NewRow();
                        dr["ReceivePortName"] = rp.Name;
                        dr["Status"] = (rl.Enable ? "Enabled" : "Disabled");
                        dr["ReceivePortLocation"] = rl.Name;
                        dr["Application"] = rp.Application.Name;
                        dr["ReceiveLocationURI"] = rl.Address;
                        dt.Rows.Add(dr);
                    }
                }

                dt.DefaultView.Sort = "ReceivePortName ASC";

            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        public ArrayList RetrieveAllReceiveLocations(string sReceivePortName)
        {
            ArrayList array = new ArrayList();
            try
            {
                bceExplorer = new BtsCatalogExplorer();
                //Edit the following connection string to point to the correct database and server
                bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
                ReceivePort rp = bceExplorer.ReceivePorts[sReceivePortName];
                foreach (ReceiveLocation rl in rp.ReceiveLocations)
                {
                    array.Add(rl.Name);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return array;
        }

        /// <summary>
        /// Bound/Started/Stopped
        /// </summary>
        /// <param name="sSendPortName"></param>
        /// <returns></returns>
        public string RetrieveSendPortStatus(string sSendPortName)
        {
            try
            {
                bceExplorer = new BtsCatalogExplorer();
                //Edit the following connection string to point to the correct database and server
                bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
                SendPort sp = bceExplorer.SendPorts[sSendPortName];
                return sp.Status.ToString();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public bool RetrieveReceiveLocationStatus(string sReceivePortName, string sReceiveLocationName)
        {
            bool status = false;

            try
            {
                bceExplorer = new BtsCatalogExplorer();
                //Edit the following connection string to point to the correct database and server
                bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
                ReceivePort rp = bceExplorer.ReceivePorts[sReceivePortName];
                foreach (ReceiveLocation rl in rp.ReceiveLocations)
                {
                    if (rl.Name.Equals(sReceiveLocationName))
                    {
                        status = rl.Enable;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                status = false;
            }

            return status;
        }


        public void StopSendPort(string sSendPortName)
        {
            try
            {
                bceExplorer = new BtsCatalogExplorer();
                //Edit the following connection string to point to the correct database and server
                bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
                SendPort sp = bceExplorer.SendPorts[sSendPortName];
                if (sp.Status == PortStatus.Started)
                {
                    sp.Status = PortStatus.Stopped;
                    bceExplorer.SaveChanges();

                    UnenlistSendPort(sSendPortName);
                }
            }
            catch (Exception e)
            {
                bceExplorer.DiscardChanges();
                throw e;
            }
        }


        public void StartSendPort(string sSendPortName)
        {
            try
            {
                bceExplorer = new BtsCatalogExplorer();
                //Edit the following connection string to point to the correct database and server
                bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
                SendPort sp = bceExplorer.SendPorts[sSendPortName];
                if ((sp.Status == PortStatus.Stopped) || (sp.Status == PortStatus.Bound))
                {
                    sp.Status = PortStatus.Started;
                    bceExplorer.SaveChanges();
                }
            }
            catch (Exception e)
            {
                bceExplorer.DiscardChanges();
                throw e;
            }
        }


        public void EnableDisableHostInstance(string hostInstance, bool isEnable)
        {
            try
            {
                bool hasData = false;
                ManagementClass objHostSettingClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_HostInstance", null);
                foreach (ManagementObject objShare in objHostSettingClass.GetInstances())
                {
                    if (hostInstance.Equals(objShare.Properties["HostName"].Value.ToString()))
                    {
                        hasData = true;
                        objShare.InvokeMethod(isEnable == true ? "Start" : "Stop", null);
                    }
                }
                if (!hasData)
                    throw new Exception("No host instance found - " + hostInstance);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void EnableReceiveLocation(string sReceivePortName, string sReceiveLocationName)
        {
            try
            {
                bceExplorer = new BtsCatalogExplorer();
                //Edit the following connection string to point to the correct database and server
                bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
                ReceivePort rp = bceExplorer.ReceivePorts[sReceivePortName];
                foreach (ReceiveLocation rl in rp.ReceiveLocations)
                {
                    if (rl.Name.Equals(sReceiveLocationName))
                    {
                        if (rl.Enable == false)
                        {
                            rl.Enable = true;
                        }
                    }
                }

                bceExplorer.SaveChanges();
            }
            catch (Exception e)
            {
                bceExplorer.DiscardChanges();
                throw e;
            }
        }

        public void DisableReceiveLocation(string sReceivePortName, string sReceiveLocationName)
        {
            try
            {
                bceExplorer = new BtsCatalogExplorer();
                //Edit the following connection string to point to the correct database and server
                bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
                ReceivePort rp = bceExplorer.ReceivePorts[sReceivePortName];
                foreach (ReceiveLocation rl in rp.ReceiveLocations)
                {
                    if (rl.Name.Equals(sReceiveLocationName))
                    {
                        if (rl.Enable == true)
                        {
                            rl.Enable = false;
                        }
                    }
                }

                bceExplorer.SaveChanges();
            }
            catch (Exception e)
            {
                bceExplorer.DiscardChanges();
                throw e;
            }
        }

        public void StartUnenlistOrchestration(string sOrchestrationName, bool isStart)
        {
            bceExplorer = new BtsCatalogExplorer();
            //Edit the following connection string to point to the correct database and server
            bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
            BtsAssemblyCollection btsAssemblyCollection = bceExplorer.Assemblies;

            foreach (BtsAssembly btsAssembly in btsAssemblyCollection)
            {
                foreach (BtsOrchestration btsOrchestration in btsAssembly.Orchestrations)
                {
                    if (sOrchestrationName.Equals(btsOrchestration.FullName))
                    {
                        try
                        {
                            if (isStart)
                            {
                                btsOrchestration.Status = OrchestrationStatus.Started;
                            }
                            else
                            {
                                btsOrchestration.AutoTerminateInstances = true;
                                btsOrchestration.Status = OrchestrationStatus.Unenlisted;
                            }
                            bceExplorer.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            bceExplorer.DiscardChanges();
                            throw e;
                        }

                    }
                }
            }
        }

        public bool UnenlistSendPort(string sSendPortName)
        {
            try
            {
                bceExplorer = new BtsCatalogExplorer();
                //Edit the following connection string to point to the correct database and server
                bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;
                SendPort sp = bceExplorer.SendPorts[sSendPortName];
                sp.Status = PortStatus.Bound;

                bceExplorer.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                bceExplorer.DiscardChanges();
                return false;
            }
        }

        public bool DisableAllReceiveLocations(string sReceivePortName)
        {
            try
            {
                bceExplorer = new BtsCatalogExplorer();
                //Edit the following connection string to point to the correct database and server
                bceExplorer.ConnectionString = bizTalkMgmtDbConnectionString;

                ReceivePort rp = bceExplorer.ReceivePorts[sReceivePortName];

                foreach (ReceiveLocation rl in rp.ReceiveLocations)
                {
                    rl.Enable = false;
                }

                bceExplorer.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                bceExplorer.DiscardChanges();
                return false;
            }
        }

     }

    /// <summary>
    /// Used to determine the scalability index of a BizTalk Application
    /// </summary>
    public class ScalabilityIndex
    {
        double orchestrationWeight = 2.4;
        double receivePortWeight = 1.9;
        double sendPortWeight = 2.1;

        /// <summary>
        /// Computes Scalability Index
        /// </summary>
        /// <param name="oNum">Number of Orchestrations</param>
        /// <param name="oaHosts">Actual number of hosts</param>
        /// <param name="oeHosts">Expected number of hosts</param>
        /// <param name="rNum">Number of receive locations</param>
        /// <param name="raHosts">Actual number of hosts for receive locations</param>
        /// <param name="reHosts">Expected number of hosts for receive locations</param>
        /// <param name="sNum">Number of send ports</param>
        /// <param name="saHosts">Actual number of hosts for send ports</param>
        /// <param name="seHosts">Expected number of hosts for send ports</param>
        /// <returns>Scalability index lies between 0.0 to 1.0</returns>
        public double Compute(int oNum, int oaHosts, out int oeHosts, int rNum, int raHosts, out int reHosts, int sNum, int saHosts, out int seHosts)
        {
            double sratio = 1.0;

            oeHosts = (int)Math.Truncate((double)oNum / orchestrationWeight);
            reHosts = (int)Math.Truncate((double)rNum / receivePortWeight);
            seHosts = (int)Math.Truncate((double)sNum / sendPortWeight);

            if (oeHosts == 0) oeHosts = 1;
            if (reHosts == 0) reHosts = 1;
            if (seHosts == 0) seHosts = 1;

            sratio = ((double)oaHosts / (double)oeHosts)
                * ((double)raHosts / (double)reHosts)
                * ((double)(saHosts / (double)seHosts));

            if (sratio > 1.0)
            {
                sratio = 1.0;
            }

            return sratio;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class BCCGridView
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="gv"></param>
        public static void Export(string fileName, GridView gv)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}", fileName));
            HttpContext.Current.Response.ContentType = "application/ms-excel";

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    //  Create a table to contain the grid
                    Table table = new Table();

                    //  include the gridline settings
                    table.GridLines = gv.GridLines;

                    //  add the header row to the table
                    if (gv.HeaderRow != null)
                    {
                        BCCGridView.PrepareControlForExport(gv.HeaderRow);
                        table.Rows.Add(gv.HeaderRow);
                    }

                    //  add each of the data rows to the table
                    foreach (GridViewRow row in gv.Rows)
                    {
                        BCCGridView.PrepareControlForExport(row);
                        table.Rows.Add(row);
                    }

                    //  add the footer row to the table
                    if (gv.FooterRow != null)
                    {
                        BCCGridView.PrepareControlForExport(gv.FooterRow);
                        table.Rows.Add(gv.FooterRow);
                    }

                    //  render the table into the htmlwriter
                    table.RenderControl(htw);

                    //  render the htmlwriter into the response
                    HttpContext.Current.Response.Write(sw.ToString());
                    HttpContext.Current.Response.End();
                }
            }
        }

        /// <summary>
        /// Replace any of the contained controls with literals
        /// </summary>
        /// <param name="control"></param>
        private static void PrepareControlForExport(Control control)
        {
            for (int i = 0; i < control.Controls.Count; i++)
            {
                Control current = control.Controls[i];
                if (current is LinkButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as LinkButton).Text));
                }
                else if (current is ImageButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as ImageButton).AlternateText));
                }
                else if (current is HyperLink)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as HyperLink).Text));
                }
                else if (current is DropDownList)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as DropDownList).SelectedItem.Text));
                }
                else if (current is CheckBox)
                {
                    control.Controls.Remove(current);
                    //control.Controls.AddAt(i, new LiteralControl((current as CheckBox).Checked ? "True" : "False"));
                    control.Controls.AddAt(i, new LiteralControl(""));
                }
                else if (current is HiddenField)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl(""));
                }
                else if (current is Label)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as Label).Text));
                }


                if (current.HasControls())
                {
                    BCCGridView.PrepareControlForExport(current);
                }
            }
        }
    }

}
