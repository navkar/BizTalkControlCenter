using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BCC.Core;

public partial class VersionControl : System.Web.UI.Page
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        string theme = Session["PAGE_THEME"] as string;

        if (theme != null)
        {
            this.Page.Theme = theme;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";
        DisplayError("This module is not enabled. Check with your administrator.");
    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        errorImg.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = true;
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
	// DO NOT : remove this method. 
    }
}
