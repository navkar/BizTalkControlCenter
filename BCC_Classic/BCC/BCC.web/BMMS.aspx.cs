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
using System.Threading;
using System.Drawing;

public partial class BiztalkMessages : System.Web.UI.Page
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
        try
        {
            lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
            new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "monitored", 102);
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
        }
    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        errorImg.Visible = true;
    }
      
    public override void VerifyRenderingInServerForm(Control control)
    {
	    // DO NOT : remove this method. 
    }
}

