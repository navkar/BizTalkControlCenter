using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml;

namespace BCC.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class BCCTransformationHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public BCCTransformationHelper()
        {

        }

        /// <summary>
        /// Transform - XML from XSLT
        /// </summary>
        /// <param name="xmlDataToTransform"></param>
        /// <param name="stylesheetPath"></param>
        /// <returns></returns>
        public string Transform(string xmlDataToTransform, string stylesheetPath)
        {
            XslCompiledTransform xct = new XslCompiledTransform();

            XmlTextReader xr = new XmlTextReader(new StreamReader(stylesheetPath));
            xct.Load(xr, XsltSettings.Default, new XmlUrlResolver());
            xr.Close();

            XsltArgumentList args = new XsltArgumentList();

            return WriteTransformedXmlDataToString(xmlDataToTransform, xct, args);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlData"></param>
        /// <param name="transform"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private string WriteTransformedXmlDataToString(string xmlData, XslCompiledTransform transform, XsltArgumentList args)
        {
            MemoryStream ms = new MemoryStream();
            this.WriteTransformedXmlDataToStream(ms, xmlData, transform, args);

            ms.Position = 0;
            string s = Encoding.UTF8.GetString(ms.GetBuffer());
            ms.Close();
            ms = null;

            return s;
        }

        /// <summary>
        /// Always use UTF-8 for encoding, else you will screw it up.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="xmlData"></param>
        /// <param name="transform"></param>
        /// <param name="args"></param>
        private void WriteTransformedXmlDataToStream(Stream s, string xmlData, XslCompiledTransform transform, XsltArgumentList args)
        {
            XPathDocument data = new XPathDocument(new MemoryStream(Encoding.UTF8.GetBytes(xmlData)));

            transform.Transform(data, args, s);
            s.Flush();
            data = null;
        }
    }
}
