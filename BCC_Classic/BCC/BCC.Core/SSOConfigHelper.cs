//---------------------------------------------------------------------
// File: SSOConfigHelper.cs
// 
// Summary: SSOConfigHelper class for reading/writing cofiguration values to/from SSO
//
// Sample: SSO as Configuration Store (BizTalk Server Sample)   
//
//---------------------------------------------------------------------
// This file is part of the Microsoft BizTalk Server 2006 SDK
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
// This source code is intended only as a supplement to Microsoft BizTalk
// Server 2006 release and/or on-line documentation. See these other
// materials for detailed information regarding Microsoft code samples.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.BizTalk.SSOClient.Interop;
using System.Data.SqlClient;
using System.Xml;

namespace BCC.Core
{
    [Serializable]
    public class ConfigurationPropertyBag : IPropertyBag
    {
        internal HybridDictionary properties;
        internal ConfigurationPropertyBag()
        {
            properties = new HybridDictionary();
        }
        public void Read(string propName, out object ptrVar, int errLog)
        {
            ptrVar = properties[propName];
        }
        public void Write(string propName, ref object ptrVar)
        {
            properties.Add(propName, ptrVar);
        }
        public bool Contains(string key)
        {
            return properties.Contains(key);
        }
        public void Remove(string key)
        {
            properties.Remove(key);
        }
    }

    public static class SSOConfigHelper
    {
        private static string idenifierGUID = "ConfigProperties";

        /// <summary>
        /// Read method helps get configuration data
        /// </summary>        
        /// <param name="appName">The name of the affiliate application to represent the configuration container to access</param>
        /// <param name="propName">The property name to read</param>
        /// <returns>
        ///  The value of the property stored in the given affiliate application of this component.
        /// </returns>
        public static string Read(string appName, string propName)
        {
            try
            {
                SSOConfigStore ssoStore = new SSOConfigStore();
                ConfigurationPropertyBag appMgmtBag = new ConfigurationPropertyBag();
                ((ISSOConfigStore)ssoStore).GetConfigInfo(appName, idenifierGUID, SSOFlag.SSO_FLAG_RUNTIME, (IPropertyBag)appMgmtBag);
                object propertyValue = null;
                appMgmtBag.Read(propName, out propertyValue, 0);
                return (string)propertyValue;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                throw;
            }
        }

 
        /// <summary>
        /// CUSTOM
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static HybridDictionary ReadApp(string appName)
        {
            try
            {
                SSOConfigStore ssoStore = new SSOConfigStore();
                ConfigurationPropertyBag appMgmtBag = new ConfigurationPropertyBag();
                ((ISSOConfigStore)ssoStore).GetConfigInfo(appName, idenifierGUID, SSOFlag.SSO_FLAG_RUNTIME, (IPropertyBag)appMgmtBag);

                return appMgmtBag.properties;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Write method helps write configuration data
        /// </summary>        
        /// <param name="appName">The name of the affiliate application to represent the configuration container to access</param>
        /// <param name="propName">The property name to write</param>
        /// <param name="propName">The property value to write</param>
        public static void Write(string appName, string[] propNames, string[] propValues)
        {
            try
            {
                SSOConfigStore ssoStore = new SSOConfigStore();
                ConfigurationPropertyBag appMgmtBag = new ConfigurationPropertyBag();

                for (int i = 0; i < propNames.Length; i++)
                {
                    object currentValue = propValues[i];
                    string currentName = propNames[i].ToString();

                    appMgmtBag.Write(currentName, ref currentValue);
                }

               
                ((ISSOConfigStore)ssoStore).SetConfigInfo(appName, idenifierGUID, appMgmtBag);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                throw;
            }
        }

        public static ArrayList ListApp(string connectionString)
        {
            string queryString =
                "select ai_app_name from [SSODB].[dbo].[SSOX_ApplicationInfo] where ai_contact_info = 'ControlCenter' or LEN(ai_app_name) < 32";

            ArrayList applicationList = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                applicationList = new ArrayList();
                // Call Read before accessing data.
                while (reader.Read())
                {
                    applicationList.Add(reader[0]);
                }

                // Call Close when done reading.
                reader.Close();
            }

            return applicationList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public static string LoadSSOConfigXml(XmlDocument configDoc)
        {
            string appName = configDoc.SelectSingleNode("//application/@name").InnerText;
            string description = configDoc.SelectSingleNode("//description").InnerText;
            string appUserAcct = configDoc.SelectSingleNode("//appUserAccount").InnerText;
            string appAdminAcct = configDoc.SelectSingleNode("//appAdminAccount").InnerText;

            //grab fields
            XmlNodeList fields = configDoc.SelectNodes("//field");

            SSOPropBag propertiesBag = new SSOPropBag();
            ArrayList maskArray = new ArrayList();
            string label = string.Empty;
            string masked = string.Empty;
            string fieldValue = string.Empty;

            foreach (XmlNode field in fields)
            {
                label = field.Attributes["label"].InnerText;
                masked = field.Attributes["masked"].InnerText;

                if (label != null && label.Length > 0)
                {
                    fieldValue = field.InnerText;

                    if (fieldValue == null || fieldValue == string.Empty)
                    {
                        fieldValue = "none";
                    }

                    //set values
                    object objPropValue = fieldValue;
                    propertiesBag.Write(label, ref objPropValue);

                    //store mask
                    if (masked == "yes")
                    {
                        maskArray.Add(SSOFlag.SSO_FLAG_FIELD_INFO_MASK);
                    }
                    else
                    {
                        maskArray.Add(0);
                    }
                }
            }

            // First delete the application and then create a new one.
            try
            {
                SSOConfigManager.DeleteApplication(appName);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.Write(exception.Message, "BCC.Core.SSOConfigHelper");
            }
            //create and enable application
            SSOConfigManager.CreateConfigStoreApplication(appName, description, appUserAcct, appAdminAcct, propertiesBag, maskArray);

            //set default configuration field values
            SSOConfigManager.SetConfigProperties(appName, propertiesBag);

            return appName;
        }
    }
}
