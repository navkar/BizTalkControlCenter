using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using BCC.Core;

public partial class UserTrace : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCUIHelper uiHelper = new BCCUIHelper();

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
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";

        ActivateUserSummaryGrid();

        if (!IsPostBack)
        {
            if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN))
            {
                ActivateGrid();
            }
            else
            {
                DisplayError(BCCUIHelper.Constants.ACCESS_DENIED);
            }
        }
    }

    private void ActivateGrid()
    {
        UserActivityGrid.Visible = true;
    }

    private void ActivateUserSummaryGrid()
    {
        UserDataSource.SelectCommand = "SELECT EventCode, Message FROM aspnet_WebEvent_Events where Details LIKE '%" + this.User.Identity.Name + "%' ORDER BY EventTime DESC";
    }

    protected void DisplayError(string message)
    {
        lblStatus.Text = message;
        lblStatus.Font.Bold = false;
	    
        lblStatus.Visible = true;
        errorImg.Visible = true;
    }

    protected void DisplayInformation(string message)
    {
        lblStatus.Text = message;
        lblStatus.Font.Bold = false;
	    lblStatus.ForeColor = Color.Teal;
        lblStatus.Visible = true;
        errorImg.Visible = false;

    }
}
