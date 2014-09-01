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
using System.Collections.Specialized;

/// <summary>
/// 
/// </summary>
public partial class Service : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCOperator bccOperator = new BCCOperator();
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
        if (!Page.IsPostBack)
        {
            // nothing to do.
        }

        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Configuration Information";

        if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN)
            || User.IsInRole(BCCUIHelper.Constants.ROLE_ARTIFACT))
        {
            PopulateViewServiceDetails();
            ActivateGrid();
        }
        else
        {
            DisplayError(BCCUIHelper.Constants.ACCESS_DENIED);
        }
    }

    private void ActivateGrid()
    {
        UserActivityGrid.Visible = true;
    }

    private void PopulateViewServiceDetails()
    {
        try
        {
            StringCollection serviceList = null;

            // Check whether the Profile is active and then proceed.
            if (Profile.ControlCenterProfile.IsProfileActive)
            {
                try
                {
                    BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                    serviceList = props.ModuleDictionary[BCCUIHelper.Constants.SERVICE_PROFILE_KEY];
                }
                catch (Exception exception)
                {
                    throw new Exception("Control Center Profile error: " + exception.Message);
                }
            }

            if (serviceList != null && serviceList.Count > 0)
            {
                DataTable dtService = bccOperator.GetServiceStatus(serviceList);

                gridServices.DataSource = dtService;
                gridServices.DataBind();
                gridServices.Visible = true;
            }
        }
        catch (Exception ex)
        {
            DisplayError("Specify the correct service name in the filters. See 'Administration > System Settings'." + ex.Message);
        }
    }
    
    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = false;
        errorImg.Visible = true;
    }

    protected void gridServices_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int position = 3;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");
        }

        if (BCCUIHelper.Constants.STATUS_STOPPED.Equals(e.Row.Cells[position].Text))
        {
            e.Row.Cells[position].ForeColor = Color.Red;
        }
        else
        if (BCCUIHelper.Constants.STATUS_RUNNING.Equals(e.Row.Cells[position].Text))
        {
            e.Row.Cells[position].ForeColor = Color.Green;
        }
        else
        if (BCCUIHelper.Constants.STATUS_DISABLED.Equals(e.Row.Cells[position].Text))
        {
            e.Row.Cells[position].ForeColor = Color.Gray;
        }
    }

    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gridServices.Rows)
        {
            CheckBox cb = (CheckBox)row.FindControl("chkBoxService");
            if (cb != null)
            {
                cb.Checked = true;
            }
        }
    }

    protected void btnDeSelectAll_Click(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gridServices.Rows)
        {
            CheckBox cb = (CheckBox)row.FindControl("chkBoxService");
            if (cb != null)
            {
                cb.Checked = false;
            }
        }
    }

    protected void btnStart_Click(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gridServices.Rows)
        {
            CheckBox cb = (CheckBox)row.FindControl("chkBoxService");

            if (cb != null && cb.Checked)
            {
                try
                {
                    dataAccess.StartService(row.Cells[1].Text);
                    new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "started " + row.Cells[1].Text, 201);
                    PopulateViewServiceDetails();
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                }
            }
        }
    }

    protected void btnStop_Click(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gridServices.Rows)
        {
            CheckBox cb = (CheckBox)row.FindControl("chkBoxService");

            if (cb != null && cb.Checked)
            {
                try
                {
                    dataAccess.StopService(row.Cells[1].Text);
                    new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "stopped " + row.Cells[1].Text, 201);
                    PopulateViewServiceDetails();
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                }
            }
        }
    }
}
