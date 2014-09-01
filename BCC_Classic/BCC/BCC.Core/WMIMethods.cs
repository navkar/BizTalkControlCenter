using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using Microsoft.BizTalk.ExplorerOM;

namespace BizTalkSetUp
{
    class WMIMethods
    {
        
        public static string MakeHost(string HostName, string Type, string HostNTGroup, bool AuthTrusted)
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
            if(Type == "Isolated")
                objHostSetting["HostType"] = HostType.Isolated;
            else
                objHostSetting["HostType"] = HostType.InProcess;

            objHostSetting["NTGroupName"] = HostNTGroup;

            objHostSetting["AuthTrusted"] = AuthTrusted;

            //create the Managementobject
            objHostSetting.Put(options);

            return Type + " Host - " + HostName + " - has been created. \r\n";
        }

        public static string InstallHosts(string ServerName, string HostName, string UserName, string Password, bool StartHost)
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
            
            if(StartHost)
                bts_AdminObjectHostInstance.InvokeMethod("Start", null);

            return "  Host - " + HostName + " - has been installed. \r\n";
        }

        public static string AddReceiveHostHandler(string Adapter, string HostName)
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

            return "    Receive Adapter " + Adapter + " set to use Host " + HostName + "\r\n";
        }

        public static string AddSendHostHandler(string Adapter, string HostName)
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

            return "    Send Adapter " + Adapter + " set to use Host " + HostName + "\r\n";
        }
    }
}
