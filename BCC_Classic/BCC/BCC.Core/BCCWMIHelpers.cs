using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using Microsoft.BizTalk.ExplorerOM;
using System.Xml;

namespace BCC.Core
{
    public enum OperationState
    {
        SUCCESS = 0,
        FAILURE = 1
    }

    public class Status
    {
        // Default is 'SUCCESS' unless its a 'FAILURE'.
        private OperationState _state = OperationState.SUCCESS;
        private string _message;

        public Status()
        {
        }

        public OperationState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }
    }
    
    public class BCCWMIHelpers
    {
        public BCCWMIHelpers()
        {

        }

        public void CreateHosts(XmlDocument oDoc)
        {
            Status status = null;

            try
            {
                // Get the list of Hosts
                XmlNodeList oNodes = oDoc.SelectNodes(@"BizTalkHostConfig/MakeHosts/Host");

                // Loop over the nodes
                foreach(XmlNode oNode in oNodes)
                {
                    string hostName = oNode["HostName"].InnerText;
                    string hostType = oNode["Type"].InnerText;
                    string hostNTGroup = oNode["NTGroup"].InnerText;
                    bool authTrusted = Convert.ToBoolean(oNode["AuthTrusted"].InnerText);

                    status = MakeHost(hostName, hostType, hostNTGroup, authTrusted);

                    if (status.State == OperationState.FAILURE)
                    {
                        throw new Exception(hostName + " " + status.Message);
                    }

                    if (oNode["InstallServers"].GetAttribute("Action") == "true")
                    {
                        // Get the list of Servers - Not tested with remote servers
                        XmlNodeList oNodeServers = oNode["InstallServers"].ChildNodes;
                        foreach (XmlNode oNodeServer in oNodeServers)
                        {
                            string serverName = oNodeServer["ServerName"].InnerText;
                            string userName = oNodeServer["UserName"].InnerText;
                            string password = oNodeServer["Password"].InnerText;
                            bool startHost = Convert.ToBoolean(oNodeServer["ServerName"].GetAttribute("Start"));

                            status = InstallHosts(serverName, hostName, userName, password, startHost);

                            if (status.State == OperationState.FAILURE)
                            {
                                throw new Exception(status.Message);
                            }
                        }  
                    }

                    if (oNode["SetAdapters"].GetAttribute("Action") == "true")
                    {
                        // Get the list of Servers - Not tested with remote servers
                        XmlNodeList oNodeAdapters = oNode["SetAdapters"].ChildNodes;
                        foreach (XmlNode oNodeAdapter in oNodeAdapters)
                        {
                            string adapterName = oNodeAdapter["AdapterName"].InnerText;

                            if (oNodeAdapter["AdapterName"].GetAttribute("Type") == "Receive")
                            {
                                status = AddReceiveHostHandler(adapterName, hostName);

                                if (status.State == OperationState.FAILURE)
                                {
                                    throw new Exception(status.Message);
                                }
                            }
                            else
                            {
                                status = AddSendHostHandler(adapterName, hostName);

                                if (status.State == OperationState.FAILURE)
                                {
                                    throw new Exception(status.Message);
                                }
                            }
                        }
                    }
                }   
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public void ResetAdapters(XmlDocument oDoc)
        {
            try
            {
                string defaulInProcessHost = oDoc.DocumentElement.GetAttribute("defaultHost").ToString();
                string defaulIsoHost = oDoc.DocumentElement.GetAttribute("defaultIsoHost").ToString();

                PutOptions options = new PutOptions();
                options.Type = PutType.UpdateOnly;

                //Look for the target WMI Class MSBTS_ReceiveHandler instance
                string strWQL = "SELECT * FROM MSBTS_ReceiveHandler";
                ManagementObjectSearcher searcherReceiveHandler = new ManagementObjectSearcher(new ManagementScope("root\\MicrosoftBizTalkServer"), new WqlObjectQuery(strWQL), null);

                string recName = string.Empty;
                string recHost = string.Empty;
                string sndName = string.Empty;
                string sndHost = string.Empty;

                if (searcherReceiveHandler.Get().Count > 0)
                {
                    foreach (ManagementObject objReceiveHandler in searcherReceiveHandler.Get())
                    {
                        //Get the Adapter Name
                        recName = objReceiveHandler["AdapterName"].ToString();

                        // Get the Current Host
                        recHost = objReceiveHandler["HostName"].ToString();

                        // Find the Host Type
                        string strWQLHost = "SELECT * FROM MSBTS_HostInstanceSetting where HostName = '" + recHost + "'";
                        ManagementObjectSearcher searcherHostHandler = new ManagementObjectSearcher(new ManagementScope("root\\MicrosoftBizTalkServer"), new WqlObjectQuery(strWQLHost), null);

                        foreach (ManagementObject objHostHandler in searcherHostHandler.Get())
                        {
                            // Type 1 is In Process
                            if (objHostHandler["HostType"].ToString() == "1")
                            {
                                objReceiveHandler.SetPropertyValue("HostNameToSwitchTo", defaulInProcessHost);
                                objReceiveHandler.Put();
                            }
                            // Otherwise it is Isolated
                            else
                            {
                                objReceiveHandler.SetPropertyValue("HostNameToSwitchTo", defaulIsoHost);
                                objReceiveHandler.Put();
                            }
                        }
                    }
                }

                //Look for the target WMI Class MSBTS_SendHandler instance
                string strWQLsnd = "SELECT * FROM MSBTS_SendHandler2";
                ManagementObjectSearcher searcherSendHandler = new ManagementObjectSearcher(new ManagementScope("root\\MicrosoftBizTalkServer"), new WqlObjectQuery(strWQLsnd), null);

                if (searcherSendHandler.Get().Count > 0)
                {
                    foreach (ManagementObject objSendHandler in searcherSendHandler.Get())
                    {
                        //Get the Adapter Name
                        sndName = objSendHandler["AdapterName"].ToString();

                        // Get the Current Host
                        sndHost = objSendHandler["HostName"].ToString();

                        // Find the Host Type
                        string strWQLHost = "SELECT * FROM MSBTS_HostInstanceSetting where HostName = '" + sndHost + "'";
                        ManagementObjectSearcher searcherHostHandler = new ManagementObjectSearcher(new ManagementScope("root\\MicrosoftBizTalkServer"), new WqlObjectQuery(strWQLHost), null);

                        foreach (ManagementObject objHostHandler in searcherHostHandler.Get())
                        {
                            // Type 1 is In Process
                            if (objHostHandler["HostType"].ToString() == "1")
                            {
                                objSendHandler.SetPropertyValue("HostNameToSwitchTo", defaulInProcessHost);
                                objSendHandler.Put();
                            }
                            // Otherwise it is Isolated
                            else
                            {
                                objSendHandler.SetPropertyValue("HostNameToSwitchTo", defaulIsoHost);
                                objSendHandler.Put();
                            }
                        }
                    }
                }

            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public static Status MakeHost(string HostName, string Type, string HostNTGroup, bool AuthTrusted)
        {
            Status operationStatus = new Status();

            try
            {
                PutOptions options = new PutOptions();
                options.Type = PutType.CreateOnly;

                // create a ManagementClass object and spawn a ManagementObject instance
                ManagementClass objHostSettingClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_HostSetting", null);
                ManagementObject objHostSetting = objHostSettingClass.CreateInstance();

                // set the properties for the Managementobject
                // Host Name
                objHostSetting["Name"] = HostName;

                // Host Type
                if (Type == "Isolated")
                    objHostSetting["HostType"] = HostType.Isolated;
                else
                    objHostSetting["HostType"] = HostType.InProcess;

                objHostSetting["NTGroupName"] = HostNTGroup;

                objHostSetting["AuthTrusted"] = AuthTrusted;

                //create the Managementobject
                objHostSetting.Put(options);

                operationStatus.Message = Type + " Host - " + HostName + " - has been created.";
            }
            catch (Exception exception)
            {
                operationStatus.Message = exception.Message;
                operationStatus.State = OperationState.FAILURE;
            }

            return operationStatus;
        }

        public static Status InstallHosts(string ServerName, string HostName, string UserName, string Password, bool StartHost)
        {
            Status operationStatus = new Status();

            try
            {
                PutOptions options = new PutOptions();
                options.Type = PutType.CreateOnly;
                ObjectGetOptions bts_objOptions = new ObjectGetOptions();

                // Creating instance of BizTalk Host.
                ManagementClass bts_AdminObjClassServerHost = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_ServerHost", bts_objOptions);
                ManagementObject bts_AdminObjectServerHost = bts_AdminObjClassServerHost.CreateInstance();

                // Make sure to put correct Server Name,username and // password
                bts_AdminObjectServerHost["ServerName"] = ServerName;
                bts_AdminObjectServerHost["HostName"] = HostName;
                bts_AdminObjectServerHost.InvokeMethod("Map", null);

                ManagementClass bts_AdminObjClassHostInstance = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_HostInstance", bts_objOptions);
                ManagementObject bts_AdminObjectHostInstance = bts_AdminObjClassHostInstance.CreateInstance();

                bts_AdminObjectHostInstance["Name"] = "Microsoft BizTalk Server " + HostName + " " + ServerName;

                //Also provide correct user name and password.
                ManagementBaseObject inParams = bts_AdminObjectHostInstance.GetMethodParameters("Install");
                inParams["GrantLogOnAsService"] = false;
                inParams["Logon"] = UserName;
                inParams["Password"] = Password;

                bts_AdminObjectHostInstance.InvokeMethod("Install", inParams, null);

                if (StartHost)
                {
                    bts_AdminObjectHostInstance.InvokeMethod("Start", null);
                }

                operationStatus.Message = "Host - " + HostName + " - has been installed. \r\n";
            }
            catch (Exception exception)
            {
                operationStatus.Message = exception.Message;
                operationStatus.State = OperationState.FAILURE;
            }

            return operationStatus;
            
        }

        public static Status AddReceiveHostHandler(string Adapter, string HostName)
        {
            Status operationStatus = new Status();

            try
            {
                PutOptions options = new PutOptions();
                options.Type = PutType.UpdateOnly;

                //Look for the target WMI Class MSBTS_ReceiveHandler instance
                string strWQL = "SELECT * FROM MSBTS_ReceiveHandler WHERE AdapterName = '" + Adapter + "'";
                ManagementObjectSearcher searcherReceiveHandler = new ManagementObjectSearcher(new ManagementScope("root\\MicrosoftBizTalkServer"), new WqlObjectQuery(strWQL), null);

                foreach (ManagementObject objReceiveHandler in searcherReceiveHandler.Get())
                {
                    objReceiveHandler.SetPropertyValue("HostNameToSwitchTo", HostName);
                    objReceiveHandler.Put();
                }

                operationStatus.Message = "Receive Adapter " + Adapter + " set to use Host " + HostName;
            }
            catch (Exception exception)
            {
                operationStatus.Message = exception.Message;
                operationStatus.State = OperationState.FAILURE;
            }

            return operationStatus;
        }

        public static Status AddSendHostHandler(string Adapter, string HostName)
        {
            Status operationStatus = new Status();

            try
            {
                PutOptions options = new PutOptions();
                options.Type = PutType.UpdateOnly;

                //Look for the target WMI Class MSBTS_SendHandler2 instance
                string strWQL = "SELECT * FROM MSBTS_SendHandler2 WHERE AdapterName = '" + Adapter + "'";
                ManagementObjectSearcher searcherSendHandler = new ManagementObjectSearcher(new ManagementScope("root\\MicrosoftBizTalkServer"), new WqlObjectQuery(strWQL), null);

                foreach (ManagementObject objSendHandler in searcherSendHandler.Get())
                {
                    objSendHandler.SetPropertyValue("HostNameToSwitchTo", HostName);
                    objSendHandler.Put();
                }

                operationStatus.Message = "Send Adapter " + Adapter + " set to use Host " + HostName;
            }
            catch (Exception exception)
            {
                operationStatus.Message = exception.Message;
                operationStatus.State = OperationState.FAILURE;
            }

            return operationStatus;
        }
    }
}
