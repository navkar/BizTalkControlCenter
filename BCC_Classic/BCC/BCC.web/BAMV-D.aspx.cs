using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BCC.Core;
using Microsoft.BizTalk.Operations;
using Microsoft.BizTalk.Message;
using Microsoft.BizTalk.Message.Interop;
using System.Drawing;
using System.Xml;

public partial class BiztalkMessageDetail : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    string messageInstanceID = "";
    string messageID = "";

    protected void Page_PreInit(object sender, EventArgs e)
    {
        string defaultTheme = Profile.ControlCenterProfile.UserTheme;
        // This will help to refresh page immediately.
        if (Request.Cookies["theme"] != null)
        {
            this.Page.Theme = Request.Cookies["theme"].Value;
        }
        else if (defaultTheme != null && defaultTheme != string.Empty)
        {
            this.Page.Theme = defaultTheme;
        }

        Session["PAGE_THEME"] = this.Page.Theme;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        lblCaption.Text = "BizTalk Artifacts - Message View Detail";

        messageID = Request.QueryString.Get("ID");

        if (messageID != null && messageID.Length > 0 && !messageID.Equals("0"))
        {
            msgDetailPanel.Visible = true;
            gridMsg.DataSource = BuildMessageTable(messageID);
            gridMsg.DataBind();
            gridMsg.Visible = true;
        }
        else
        {
            DisableView();
        }
    }

    private void DisableView()
    {
        msgDetailPanel.Visible = false;
    }

    private DataTable BuildMessageTable(string messageID)
    {
        System.Data.DataTable dt = null;

        try
        {
            DataColumn dcMP = new DataColumn("MsgProperty", typeof(System.String));
            DataColumn dcMV = new DataColumn("MsgValue", typeof(System.String));

            dt = new System.Data.DataTable();
            dt.Columns.Add(dcMP);
            dt.Columns.Add(dcMV);

            BizTalkMessage message = dataAccess.RetrieveBiztalkMessage(messageID);

            if (message != null)
            {
                dt.Rows.Add("Adapter", message.AdapterName);
                dt.Rows.Add("Creation Time", message.CreationTime.ToString());
                messageInstanceID = message.InstanceID.ToString();
                dt.Rows.Add("Message ID", message.MessageID.ToString());
                dt.Rows.Add("Message Status", message.MessageStatus);
                ServiceClass sc = message.Class;
                dt.Rows.Add("Message Class", sc.ToString());
                dt.Rows.Add("Message Type", message.MessageType);
                dt.Rows.Add("Host Name", message.HostName);
                dt.Rows.Add("Instance ID", messageInstanceID);
                dt.Rows.Add("Instance Status", message.InstanceStatus.ToString());
                dt.Rows.Add("MBox-DBName", message.MessageBox.DBName);
                dt.Rows.Add("Part count", message.PartCount);
                dt.Rows.Add("Retry count", message.RetryCount);
                dt.Rows.Add("Service Type", message.ServiceType);
                dt.Rows.Add("Submitter", message.Submitter);
                dt.Rows.Add("URL", message.Url);

                if (message.ErrorCode != null)
                {
                    dt.Rows.Add("Error Code", message.ErrorCode);
                }
                else
                {
                    dt.Rows.Add("Error Code", "none");
                }

                if (message.ErrorDescription != null)
                {
                    dt.Rows.Add("Error Description", message.ErrorDescription);
                }
                else
                {
                    dt.Rows.Add("Error Description", "none");
                }
            }
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
        }

        return dt;
    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = true;
    }

    protected string GetMessageDetail()
    {
        string msgData = "";

        try
        {
            if (messageID != null && messageID.Length > 0)
            {
                msgData = FormatHTMLTable(dataAccess.GetBiztalkMessage(messageID));
            }

        }
        catch
        {
            msgData = "Message Detail: No additional information has been found for the message.";
        }

        return msgData;
    }

    private string FormatHTMLTable(string data)
    {
        string tableStart = "<table width=\"80%\" class=\"formattedsourcecode\" cellpadding=\"0\" cellspacing=\"0\">";
        string tableEnd = "</table>";
        string trStart = "<tr bgcolor=\"#F5F5F5\">";
        string tdLine = "<td class=\"linenumber\">";
        string tdContent = "<td class=\"content\">";
        string trEnd = "</tr>";
        string tdEnd = "</td>";

        string tableData = tableStart;

        string[] rows = data.Split('>');
        int counter = 1;
        string formattedRow = "";

        foreach (string row in rows)
        {
            if (row.Length > 0)
            {
                formattedRow = row + "&gt;";

                formattedRow = formattedRow.Replace("&lt;", "<");
                formattedRow = formattedRow.Replace("&gt;", ">");

                formattedRow = formattedRow.Replace("<", "&lt;");
                formattedRow = formattedRow.Replace(">", "&gt;");

                tableData = tableData +
                        trStart +
                        tdLine + counter + tdEnd +
                        tdContent + formattedRow + tdEnd +
                        trEnd;
            }
            counter++;
        }

        tableData = tableData + tableEnd;
        return tableData;
    } 

    protected void btnResumeMessage_Click(object sender, EventArgs e)
    {
        dataAccess.ResumeMessage(messageInstanceID);
        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "resumed " + messageInstanceID, 204);
        btnResumeMessage.Enabled = false;
        btnTerminate.Enabled = false;
    }

    protected void btnTerminate_Click(object sender, EventArgs e)
    {
        dataAccess.TerminateMessage(messageInstanceID);
        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "terminated " + messageInstanceID, 204);
        btnResumeMessage.Enabled = false;
        btnTerminate.Enabled = false;
    }

    protected void gridMsg_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.Cells[1].Text.Contains("Suspended"))
        {
            e.Row.Cells[1].ForeColor = Color.Red;
        }
    }
}
