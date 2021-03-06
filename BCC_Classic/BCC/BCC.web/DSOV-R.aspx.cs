using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using BCC.Core;
using BCC.BEOM;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Text;

/// <summary>
/// Orchestration Viewer
/// </summary>
public partial class OrchDocView : System.Web.UI.Page
{
    private string orchestrationName = string.Empty;
    private string assemblyName = string.Empty;

    protected void Page_PreInit(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// Page load 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        orchestrationName = this.Request.QueryString["ON"];
        assemblyName = this.Request.QueryString["AN"];

        this.Page.Title = "Orchestration: " + orchestrationName + " [Assembly: " + assemblyName + "]";
        OdxViewHeader.Text = "Orchestration: " + orchestrationName + " [Assembly: " + assemblyName + "]";

        new ActivityHelper().RaiseAuditEvent(this, "Orchestration View", "viewed " + orchestrationName, 701);

        DisplayOrchestration(orchestrationName, assemblyName);
    }

    /// <summary>
    /// Display orchestration view pane...
    /// </summary>
    /// <param name="orchestrationName"></param>
    /// <param name="assemblyName"></param>
    private void DisplayOrchestration(string orchestrationName, string assemblyName)
    {
        try
        {
            BizTalkInstallation bizTalkInstallation = new BizTalkInstallation();
            bizTalkInstallation.Server = BCCOperator.BizTalkSQLServer();
            bizTalkInstallation.MgmtDatabaseName = BCCOperator.BizTalkMgmtDb();

            Orchestration oInstance = bizTalkInstallation.GetOrchestration(assemblyName, orchestrationName);

            if (oInstance != null)
            {
                string filePath = BuildFilePath(oInstance.Name);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                if (oInstance.SaveAsImage(filePath))
                {
                    // Left side panel 
                    this.leftSideView.Controls.Add(ShowOrchestration(oInstance.Name, orchestrationName));
                }

                if (oInstance.ShapeMap.Count > 0)
                {
                    XslCompiledTransform orchCodeTransform = new XslCompiledTransform();

                    XmlTextReader xr = new XmlTextReader(new StreamReader(StylesheetPath("Header")));
                    orchCodeTransform.Load(xr, XsltSettings.Default, new XmlUrlResolver());
                    xr.Close();

                    XsltArgumentList orchCodeXsltArgs = new XsltArgumentList();

                    string header = WriteTransformedXmlDataToString2(
                                      oInstance.GetXml(),
                                      orchCodeTransform,
                                      orchCodeXsltArgs);

                    Label lblOrchData = new Label();
                    lblOrchData.Text = header;
                    lblOrchData.BorderStyle = BorderStyle.Solid;
                    lblOrchData.BorderWidth = Unit.Pixel(2);
                    lblOrchData.ToolTip = "Orchestration header section";

                    // Right side panel
                    this.rightSideView.Controls.Add(lblOrchData);
                    // Right side panel
                    this.rightSideView.Controls.Add(new LiteralControl("<br/><br/>"));

                    // Orchestration data
                    orchCodeTransform = new XslCompiledTransform();

                    xr = new XmlTextReader(new StreamReader(StylesheetPath("Body") ) );
                    orchCodeTransform.Load(xr, XsltSettings.Default, new XmlUrlResolver());
                    xr.Close();

                    orchCodeXsltArgs = new XsltArgumentList();
                    orchCodeXsltArgs.AddParam("OrchName", string.Empty, oInstance.Name);

                    string body = WriteTransformedXmlDataToString(
                                      oInstance.ArtifactData,
                                      orchCodeTransform,
                                      orchCodeXsltArgs);

                    Label lblOrchData2 = new Label();
                    lblOrchData2.Text = body;
                    lblOrchData2.BorderStyle = BorderStyle.Solid;
                    lblOrchData2.BorderWidth = Unit.Pixel(2);
                    lblOrchData2.ToolTip = "Orchestration body section";

                    // Right side panel
                    this.rightSideView.Controls.Add(lblOrchData2);
                }
                else
                {
                    // Right side panel
                    this.rightSideView.Controls.Add(new LiteralControl("No orchestration shapes were found."));
                }
            }
        }
        catch (Exception exception)
        {
            this.leftSideView.Controls.Add(new LiteralControl("Exception:" + exception.Message + ". Try refreshing the page or recycling the application pool."));
        }
    }

    private System.Web.UI.WebControls.Image ShowOrchestration(string orchestrationName, string toolTip)
    {
        System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
        img.ImageUrl = @"~\Images\" + orchestrationName + ".jpg";
        img.Visible = true;
        img.ToolTip = toolTip;
        return img;
    }

    private string BuildFilePath(string orchestrationName)
    {
        return Server.MapPath(@"~\Images\" + orchestrationName + ".jpg");
    }

    private string StylesheetPath(string type)
    {
        if (type.Equals("Body"))
        {
            return Server.MapPath(@"~\xslt\OrchCodeView.xslt");
        }
        else
        {
            return Server.MapPath(@"~\xslt\OrchHeader.xslt");
        }
    }

    /// <summary>
    /// Always use UTF-8 for encoding, else you will screw it up.
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
    /// <param name="xmlData"></param>
    /// <param name="transform"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    private string WriteTransformedXmlDataToString2(string xmlData, XslCompiledTransform transform, XsltArgumentList args)
    {
        MemoryStream ms = new MemoryStream();

        XmlTextReader reader = new XmlTextReader(new StringReader(xmlData));
        XmlTextWriter writer = new XmlTextWriter(ms, Encoding.ASCII);

        transform.Transform(reader, args, writer);
        // Set the position to 0, to read the memory stream.
        ms.Position = 0;

        StreamReader sr = new StreamReader(ms);
        return sr.ReadToEnd();
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
