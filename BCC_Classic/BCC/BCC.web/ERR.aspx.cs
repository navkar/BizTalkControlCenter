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
using System.IO;
using BCC.Core;
using System.Collections.Specialized;
using System.Drawing;

public partial class CustomError : System.Web.UI.Page
{
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
        InitializeObjects();

    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = false;
        errorImg.Visible = true;
    }

    private void InitializeObjects()
    {
        lblError.Visible = false;
     //   lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        lblError.Text = "Custom Errors";
        subCaption.Text = "Information";
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        // DO NOT : remove this method. 
    }
}
