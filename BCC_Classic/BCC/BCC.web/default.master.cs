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

public partial class DefaultMaster : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lblInfo.Text = "BizTalk Control Center [" + System.Environment.MachineName + "]";
        Page.Title = "BCC - [" + System.Environment.MachineName + "]";
    }

    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static string GetDynamicContent(string contextKey)
    {
        return default(string);
    }

    public void DisplayError(string message)
    {
        lblStatus.Text = message;
        lblStatus.Font.Bold = true;

        lblStatus.Visible = true;
        errorImg.Visible = true;
    }
}
