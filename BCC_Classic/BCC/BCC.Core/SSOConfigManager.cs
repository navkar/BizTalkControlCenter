using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using Microsoft.EnterpriseSingleSignOn.Interop;

namespace BCC.Core
{
    public static class SSOConfigManager
    {
        //don't actually need a GUID value
        private static string idenifierGUID = "ConfigProperties";

        /// <summary>
        /// Creates a new SSO ConfigStore application.
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="description"></param>
        /// <param name="uAccountName"></param>
        /// <param name="adminAccountName"></param>
        /// <param name="propertiesBag"></param>
        /// <param name="maskArray"></param>
        public static void CreateConfigStoreApplication(string appName, string description, string uAccountName, string adminAccountName, SSOPropBag propertiesBag, ArrayList maskArray)
        {
            int appFlags = 0;

            //bitwise operation for flags
            appFlags |= SSOFlag.SSO_FLAG_APP_CONFIG_STORE;
            appFlags |= SSOFlag.SSO_FLAG_SSO_WINDOWS_TO_EXTERNAL;
            appFlags |= SSOFlag.SSO_FLAG_APP_ALLOW_LOCAL;

            ISSOAdmin ssoAdmin = new ISSOAdmin();

            //create app
            ssoAdmin.CreateApplication(appName, description, "ControlCenter", uAccountName, adminAccountName, appFlags, propertiesBag.PropertyCount);

            //create property fields
            int counter = 0;

            //create dummy field in first slot
            ssoAdmin.CreateFieldInfo(appName, "dummy", 0);
            //create real fields
            foreach (DictionaryEntry de in propertiesBag.properties)
            {
                string propName = de.Key.ToString();
                int fieldFlags = 0;
                fieldFlags |= Convert.ToInt32(maskArray[counter]);

                //create property
                ssoAdmin.CreateFieldInfo(appName, propName, fieldFlags);

                counter++;
            }

            //enable application
            ssoAdmin.UpdateApplication(appName, null, null, null, null, SSOFlag.SSO_FLAG_ENABLED, SSOFlag.SSO_FLAG_ENABLED);

        }

        /// <summary>
        /// Set values for application fields
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="propertyBag"></param>
        public static void SetConfigProperties(string appName, SSOPropBag propertyBag)
        {
            ISSOConfigStore configStore = new ISSOConfigStore();

            configStore.SetConfigInfo(appName, idenifierGUID, propertyBag);
 
        }

        /// <summary>
        /// Retrieve dictionary of field/value pairs
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="description"></param>
        /// <param name="contactInfo"></param>
        /// <param name="appUserAcct"></param>
        /// <param name="appAdminAcct"></param>
        /// <returns></returns>
        public static HybridDictionary GetConfigProperties(string appName, out string description, out string contactInfo, out string appUserAcct, out string appAdminAcct)
        {
            int flags;
            int count;

            //get config info
            ISSOAdmin ssoAdmin = new ISSOAdmin();
            ssoAdmin.GetApplicationInfo(appName, out description, out contactInfo, out appUserAcct, out appAdminAcct, out flags, out count);
            
            //get properties
            ISSOConfigStore configStore = new ISSOConfigStore();
            SSOPropBag propertiesBag = new SSOPropBag();

            configStore.GetConfigInfo(appName, idenifierGUID, SSOFlag.SSO_FLAG_RUNTIME, propertiesBag);

            return propertiesBag.properties;
        }

        /// <summary>
        /// Remove the application
        /// </summary>
        /// <param name="appName"></param>
        public static void DeleteApplication(string appName)
        {
            ISSOAdmin ssoAdmin = new ISSOAdmin();

            ssoAdmin.DeleteApplication(appName);
        }
    }

    public class SSOPropBag : IPropertyBag
    {
        internal HybridDictionary properties;

        public SSOPropBag()
        {
            properties = new HybridDictionary();
        }

        #region IPropertyBag Members

        public void Read(string propName, out object ptrVar, int errorLog)
        {
            ptrVar = properties[propName];
        }

        public void Write(string propName, ref object ptrVar)
        {
            properties.Add(propName, ptrVar);
        }

        public int PropertyCount
        {
            get
            {
                return properties.Count;
            }
        }

        #endregion
    }
}
