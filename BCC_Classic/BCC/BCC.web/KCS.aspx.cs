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
using System.Drawing;

public partial class KnowledgeCenterSolutions : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCOperator hubOperator = new BCCOperator();
    BCCUIHelper uiHelper = new BCCUIHelper();
    public int count = 0;

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
        InitializeObjects();
        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "viewed", 701);
    }

    private void InitializeObjects()
    {
        lblError.Visible = false;
	    txtSearchKey.Focus();
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = true;
        
        errorImg.Visible = true;
    }

    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
    }
    protected void btnDeSelectAll_Click(object sender, EventArgs e)
    {
    }
    protected void btnStart_Click(object sender, EventArgs e)
    {
    }
    protected void btnStop_Click(object sender, EventArgs e)
    {
    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        DisplayError("This module has to be purchased.");
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        HtmlForm myMasterForm = (HtmlForm)Master.FindControl("form1");

        if (myMasterForm != null)
        {
           // myMasterForm.Controls.Add(gridSendPort);
        }

        //BCCGridView.Export("ControlCenter_" + System.Environment.MachineName + "_SendPorts.xls", this.gridSendPort);
    }
}
