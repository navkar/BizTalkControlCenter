using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BCC.Core;
using BCC.Core.WMI.BizTalk;

public partial class Host : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCOperator bccOperator = new BCCOperator();
    BCCUIHelper uiHelper = new BCCUIHelper();
    const string CONTROL_NAME = "chkBoxHost";
    private int position = 6;

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

        if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN)
                || User.IsInRole(BCCUIHelper.Constants.ROLE_ARTIFACT))
        {
            InitializeObjects();
            PopulateGrid();
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

    private void PopulateGrid()
    {
        try
        {
            StringCollection hostInstanceList = null;
            DataTable dt = null;

            // Check whether the Profile is active and then proceed.
            if (Profile.ControlCenterProfile.IsProfileActive)
            {
                try
                {
                    BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                    hostInstanceList = props.ModuleDictionary[BCCUIHelper.Constants.HOST_INSTANCE_KEY];
                }
                catch
                {
                    // Ignore errors from Profile system
                }
            }

            if (hostInstanceList != null && hostInstanceList.Count > 0)
            {
                dt = dataAccess.RetrieveAllHostsAndInstance(hostInstanceList, true);
            }
            else
            {
                dt = dataAccess.RetrieveAllHostsAndInstance(null, false);
            }

            gridHost.DataSource = dt;
            gridHost.DataBind();
            gridHost.Visible = true;
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            gridHost.Visible = true;
        }
    }

    protected void DisplayError(string message)
    {
        errorImg.Visible = true;
        lblError.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = false;
    }

    private void InitializeObjects()
    {
        lblError.Visible = false;
    }
    
    protected void btnEnable_Click(object sender, EventArgs e)
    {
        string errorMsg = string.Empty;
        string status = string.Empty;
        string hostName = string.Empty;
        string hostType = string.Empty;
        // Iterate through the Gridview Rows property
        foreach (GridViewRow row in gridHost.Rows)
        {
            status = row.Cells[position].Text;
            hostName = row.Cells[2].Text;
            hostType = row.Cells[3].Text;

            // Access the CheckBox
            CheckBox cb = (CheckBox)row.FindControl(CONTROL_NAME);
            if (cb != null && cb.Checked)
            {
                try
                {
                    if ("STOPPED".Equals(status.ToUpper()))
                    {
                        if (!(row.Cells[4].Text.Equals("Yes")))
                        {
                            dataAccess.EnableDisableHostInstance(hostName, true);
                            new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "started " + hostName, 202);
                        }
                        else // Disabled case
                        {
                            DisplayError("Hosts which are 'Disabled' cannot be started or stopped. Refer to the notes section.");
                        }
                    }
                    else if ("ISOLATED".Equals(hostType.ToUpper()))
                    {
                        DisplayError("Hosts of type 'Isolated' cannot be started or stopped. Refer to the notes section.");
                    }
                }
                catch (Exception exception)
                {
                    DisplayError(exception.Message);
                }
            }
        }
        PopulateGrid();
    }

    protected void btnDisable_Click(object sender, EventArgs e)
    {
        string errorMsg = string.Empty;
        string status = string.Empty;
        string hostName = string.Empty;
        string hostType = string.Empty;
        // Iterate through the Gridview Rows property
        foreach (GridViewRow row in gridHost.Rows)
        {
            status = row.Cells[position].Text;
            hostName = row.Cells[2].Text;
            hostType = row.Cells[3].Text;

            // Access the CheckBox
            CheckBox cb = (CheckBox)row.FindControl(CONTROL_NAME);
            if (cb != null && cb.Checked)
            {
                try
                {
                    if ("RUNNING".Equals(status.ToUpper()))
                    {
                        dataAccess.EnableDisableHostInstance(hostName, false);
                        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "stopped " + hostName, 202);
                    }
                    else if ("ISOLATED".Equals(hostType.ToUpper()))
                    {
                        DisplayError("Hosts of type 'Isolated' cannot be started or stopped. Refer to the notes section.");
                    }
                }
                catch (Exception exception)
                {
                    DisplayError(exception.Message);
                }
            }
        }
        PopulateGrid();
    }

    protected void btnMonitor_Click(object sender, EventArgs e)
    {
        Monitor();
        PopulateGrid();
    }

    private void Monitor()
    {
        string hostName = string.Empty;

        // Iterate through the Gridview Rows property
        foreach (GridViewRow row in gridHost.Rows)
        {
            hostName = row.Cells[2].Text;
            // Access the CheckBox
            CheckBox cb = (CheckBox)row.FindControl(CONTROL_NAME);

            if (cb != null && cb.Checked)
            {
                try
                {
                    BCCMonitoringDataAccess da = new BCCMonitoringDataAccess();
                    da.CreateMonitoringEntry(ArtifactType.HostInstance, hostName);
                    new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, " setup monitoring for host instance " + hostName, 202);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Write(e.Message + e.StackTrace);
                }
            }
        }
    }

    protected void gridHost_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");

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
                    if (BCCUIHelper.Constants.STATUS_UNKNOWN.Equals(e.Row.Cells[position].Text))
                    {
                        e.Row.Cells[position].ForeColor = Color.Gray;
                    }
        }
    }

    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
        bccOperator.SetCheckAllCheckBox(gridHost, CONTROL_NAME, true);
    }

    protected void btnDeSelectAll_Click(object sender, EventArgs e)
    {
        bccOperator.SetCheckAllCheckBox(gridHost, CONTROL_NAME, false);
    }


}
