using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Data;
using System.Xml;

namespace BCC.Core
{
    [Serializable]
    public class BCCProfile
    {
        public bool IsProfileActive;
        public string UserTheme;
        public DateTime Created;
        public DateTime LastAccessed;
        public DateTime LastUpdated;
        public bool IsFilterEnabled;
        public Dictionary<string, BCCModuleProperty> ModuleFilter = new Dictionary<string, BCCModuleProperty>();
    }

    [Serializable]
    public class BCCModuleProperty
    {
        private NameValueCollection _moduleKeys = null;
        private Dictionary<string, StringCollection> _moduleDictionary = null;

        public BCCModuleProperty()
        {
            _moduleKeys = new NameValueCollection();
            _moduleDictionary = new Dictionary<string, StringCollection>();
        }

        public NameValueCollection ModuleKeys
        {
            get
            {
                return _moduleKeys;
            }
            set
            {
                _moduleKeys = value;
            }
        }

        public Dictionary<string, StringCollection> ModuleDictionary
        {
            get
            {
                return _moduleDictionary;
            }
            set
            {
                _moduleDictionary = value;
            }
        }

        public Dictionary<string, StringCollection>.KeyCollection ModuleKeyCollection
        {
            get
            {
                return _moduleDictionary.Keys;
            }
        }

        public Dictionary<string, StringCollection>.ValueCollection ModuleValueCollection
        {
            get
            {
                return _moduleDictionary.Values;
            }
        }

        public Dictionary<string, StringCollection>.Enumerator ModuleEnumerator
        {
            get
            {
                return _moduleDictionary.GetEnumerator();
            }
        }
    }

    public class BCCProfileHelper
    {
        public static DataTable RetrieveUserProfile(BCCProfile userProfile)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn("IsFilterEnabled", typeof(string)));
            dt.Columns.Add(new DataColumn("IsProfileActive", typeof(string)));
            dt.Columns.Add(new DataColumn("LastAccessed", typeof(string)));
            dt.Columns.Add(new DataColumn("LastUpdated", typeof(string)));
            dt.Columns.Add(new DataColumn("Created", typeof(string)));
            dt.Columns.Add(new DataColumn("UserTheme", typeof(string)));

            DataRow dr = dt.NewRow();
            dr["IsFilterEnabled"] = userProfile.IsFilterEnabled;
            dr["IsProfileActive"] = userProfile.IsProfileActive;
            dr["LastAccessed"] = userProfile.LastAccessed;
            dr["LastUpdated"] = userProfile.LastUpdated;
            dr["Created"] = userProfile.Created;
            dr["UserTheme"] = userProfile.UserTheme;

            dt.Rows.Add(dr);

            return dt;
        }

        /// <summary>
        /// This is a very powerful and extensible parser for Page Profile.
        /// 
        ///    <Module ID="302" Title="Partner Validation - SQL Server Jobs" SubTitle="Information">
        ///    <Basic>
        ///        <Key ID="" Value="" />
        ///    </Basic>
        ///    <Extended>
        ///        <Key ID="JobServerList">
        ///            <Value></Value>
        ///            <Value></Value>
        ///            <Value></Value>
        ///            <Value></Value>
        ///        </Key>
        ///    </Extended>
        ///    </Module>
        /// </summary>
        /// <param name="basicProfileXmlDoc"></param>
        /// <returns>Profile</returns>
        public static BCCProfile CreateDefaultProfile(XmlDocument basicProfileXmlDoc)
        {
            BCCProfile defaultProfile = new BCCProfile();

            defaultProfile.IsFilterEnabled = false;
            defaultProfile.IsProfileActive = true;
            defaultProfile.LastAccessed = System.DateTime.Now;
            defaultProfile.LastUpdated = System.DateTime.Now;
            defaultProfile.Created = System.DateTime.Now;
            defaultProfile.UserTheme = String.Empty;

            XmlNode root = basicProfileXmlDoc.SelectSingleNode("/Profile");
            XmlNodeList moduleList = null;
            BCCModuleProperty moduleProp = null;
            NameValueCollection nvc = null;
            
            if (root != null)
            {
                foreach(XmlAttribute attr in root.Attributes)
                {
                    if (attr.Name == "UserTheme")
                    {
                        defaultProfile.UserTheme = attr.Value;
                        break;
                    }
                }
            
                if (root.HasChildNodes)
                {
                    moduleList = root.ChildNodes;

                    foreach (XmlNode module in moduleList)
                    {
                        // Create a new instance per module
                        moduleProp = new BCCModuleProperty();
                        nvc = new NameValueCollection();

                        string moduleID = string.Empty;

                        // <Module ID="302" Title="Partner Validation - SQL Server Jobs" SubTitle="Information">
                        if (module.Attributes != null)
                        {
                            foreach (XmlAttribute attr in module.Attributes)
                            {
                                if (attr.Name != string.Empty && attr.Value != string.Empty)
                                {
                                    nvc.Add(attr.Name, attr.Value);
                                }

                                if (attr.Name == "ID")
                                {
                                    moduleID = attr.Value;
                                }
                            }
                        }

                        //    <Basic>
                        //        <Key ID="" Value="" />
                        //    </Basic>
                        if (module.HasChildNodes)
                        {
                            foreach (XmlNode node in module.ChildNodes)
                            {
                                // <Key>
                                if (node.HasChildNodes)
                                {
                                    if (node.Name == "Basic")
                                    {
                                        // Set of all keys
                                        foreach (XmlNode key in node.ChildNodes)
                                        {
                                            string attrName = string.Empty;
                                            string attrValue = string.Empty;
                                            // Get all the attributes in each key
                                            foreach (XmlAttribute attr in key.Attributes)
                                            {
                                                if (attr.Name == "ID")
                                                {
                                                    attrName = attr.Value;
                                                }
                                                else if (attr.Name == "Value")
                                                {
                                                    attrValue = attr.Value;
                                                }

                                                if (attrName != string.Empty && attrValue != string.Empty)
                                                {
                                                    // TODO: add contains logic to filter out duplicates.
                                                    nvc.Add(attrName, attrValue);
                                                }
                                            }
                                        }
                                    }
                                    else // Extended
                                    {
                                        foreach (XmlNode key in node.ChildNodes)
                                        {
                                            bool skipFlag = false;
                                            string keyName = string.Empty;
                                            StringCollection valueColl = null;
                                            // Get all the attributes in each key
                                            foreach (XmlAttribute attr in key.Attributes)
                                            {
                                                if (attr.Name == "ID")
                                                {
                                                    if (attr.Value != string.Empty)
                                                    {
                                                        keyName = attr.Value;
                                                    }
                                                    else
                                                    {
                                                        skipFlag = true;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (key.HasChildNodes && !skipFlag)
                                            {
                                                valueColl = new StringCollection();
                                                
                                                foreach (XmlNode valueNode in key.ChildNodes)
                                                {
                                                    if (valueNode.InnerText != String.Empty)
                                                    {
                                                        valueColl.Add(valueNode.InnerText);
                                                    }
                                                }
                                            }

                                            // build a dict object
                                            if (!skipFlag)
                                            {
                                                moduleProp.ModuleDictionary.Add(keyName, valueColl);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Note that 'NameValueCollection' covers both the module tag and keys tag.
                        moduleProp.ModuleKeys = nvc;
                        defaultProfile.ModuleFilter.Add(moduleID, moduleProp);
                    }
                }
            }
          
            return defaultProfile;
        }
    }

}
