using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BCC.Core
{
    [Serializable]
    public class NameValuePair
    {
        private string _name;
        private string _displayName;
        private string _value;
        private string _toolTip;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public string ToolTip
        {
            get
            {
                return _toolTip;
            }
            set
            {
                _toolTip = value;
            }
        }
    }

    [Serializable]
    public class NameValuePairSet: IList<NameValuePair>
    {
        private List<NameValuePair> _list = new List<NameValuePair>(); 

        #region IList<NameValuePair> Members

        public int IndexOf(NameValuePair item)
        {
            return _list.IndexOf(item); 
        }

        public void Insert(int index, NameValuePair item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index); 
        }

        public NameValuePair this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        #endregion

        #region ICollection<NameValuePair> Members

        public void Add(NameValuePair item)
        {
            _list.Add(item); 
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(NameValuePair item)
        {
            return _list.Contains(item); 
        }

        public void CopyTo(NameValuePair[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex); 
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(NameValuePair item)
        {
            return _list.Remove(item); 
        }

        #endregion

        #region IEnumerable<NameValuePair> Members

        public IEnumerator<NameValuePair> GetEnumerator()
        {
            return _list.GetEnumerator(); 
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); 
        }

        #endregion
    }

    public class BCCManageConfigData
    {
        private NameValuePairSet nvc = null;
        private string speedcode = string.Empty;

        public string Speedcode
        {
            get
            {
                return speedcode;
            }
            set
            {
                speedcode = value;
            }
        }

        public NameValuePairSet ConfigurationData
        {
            get
            {
                return nvc;
            }
            set
            {
                nvc = value;
            }
        }

        public BCCManageConfigData()
        {
        }

        public BCCManageConfigData(string speedcode)
        {
            this.speedcode = speedcode;

            Query();
        }

        /// <summary>
        /// Return Hexadecimal data as a string. (Uses Unicode encoding)
        /// </summary>
        /// <param name="stringData"></param>
        /// <returns></returns>
        private string ConvertStringToHex(string stringData)
        {
            string hexData = string.Empty;

            UnicodeEncoding unicode = new UnicodeEncoding();
            byte[] byteData = unicode.GetBytes(stringData);
            hexData = BCCHexUtil.ToString(byteData);
            return hexData;
        }

        /// <summary>
        /// Returns string from Hexadecimal string. (Uses Unicode encoding)
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private string ConvertHexToString(string hexString)
        {
            string data = string.Empty;

            if (BCCHexUtil.IsHexString(hexString))
            {
                byte[] normalData = BCCHexUtil.GetBytes(hexString);
                UnicodeEncoding unicode = new UnicodeEncoding();
                data = unicode.GetString(normalData);
            }

            return data;
        }

        private string ConfigSerializer()
        {
            XmlSerializer xser = new XmlSerializer(typeof(NameValuePairSet));

            StringWriter sw = new StringWriter();
            xser.Serialize(sw, ConfigurationData);

            StringBuilder sb = sw.GetStringBuilder();

            return sb.ToString();
        }

        private void ConfigDeserializer(string configData)
        {
            try
            {
                XmlSerializer xser = new XmlSerializer(typeof(NameValuePairSet));
                StringReader sr = new StringReader(configData);
                XmlReader reader = new XmlTextReader(sr);

                ConfigurationData = (NameValuePairSet)xser.Deserialize(reader);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e.Message + e.StackTrace, "BCC-ManageConfigData");
            }
        }

        /// <summary>
        /// bcc_ConfigData_CreateEntry
        /// </summary>
        public void Update()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("bcc_ConfigData_CreateEntry", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@speedcode", Speedcode);
                command.Parameters.Add(param);

                // Serialize and then encode
                string serializedData = ConfigSerializer();
                string encodedData = ConvertStringToHex(serializedData);

                param = new SqlParameter("@configData", encodedData);
                command.Parameters.Add(param);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// bcc_ConfigData_Query
        /// </summary>
        public void Query()
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString))
            {
                SqlCommand command = new SqlCommand("bcc_ConfigData_Query", connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter("@speedcode", Speedcode);
                command.Parameters.Add(param);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                string configData = string.Empty;
                // Call Read before accessing data.
                while (reader.Read())
                {
                    configData = (string) reader[0];
                }

                // Call Close when done reading.
                reader.Close();

                // Decode and then deserialize
                string decodedData = ConvertHexToString(configData);
                ConfigDeserializer(decodedData);
            }
        }
    }
}
