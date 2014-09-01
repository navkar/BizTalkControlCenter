using BCC.Core;
using System;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.Web.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;

public partial class PartnerJobs : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCOperator bccOperator = new BCCOperator();
    BCCUIHelper uiHelper = new BCCUIHelper();
    public int count = 0;

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

        if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN) || User.IsInRole(BCCUIHelper.Constants.ROLE_ARTIFACT))
        {
            PopulateGrid(string.Empty);
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

    private void InitializeObjects()
    {
        gridSQLJobs.Visible = false;
        lblError.Visible = false;
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";
        this.search.SearchClick += new EventHandler(this.btnFilter_Click);
    }

    private void UpdateLabel()
    {
        search.KeywordLabel = "(keywords: 'Purge', 'Started', 'MessageBox', ...) [" + count + "]";
    }

    private void PopulateGrid(string searchKeyword)
    {
        try
        {
            StringCollection jobConnectionStringList = null;
            StringCollection jobsList = null;

            // Check whether the Profile is active and then proceed.
            if (Profile.ControlCenterProfile.IsProfileActive)
            {
                try
                {
                    BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];

                    if (props != null)
                    {
                        jobConnectionStringList = props.ModuleDictionary[BCCUIHelper.Constants.JOB_CONNECTION_STRING_LIST_KEY];
                        jobsList = props.ModuleDictionary[BCCUIHelper.Constants.JOB_LIST_KEY];
                    }
                }
                catch
                {
                    // Ignore errors from Profile system
                }
            }

            BCCJobsHelper sqlJobs = null;
            DataTable dt = null;

            if (jobConnectionStringList != null)
            {
                // The system should work even if one SQL job connection string goes bad.
                foreach (string jobConnectionString in jobConnectionStringList)
                {
                    DataTable dtTemp = null;
                    try
                    {
                        sqlJobs = new BCCJobsHelper(jobConnectionString);
                        // If SQL Jobs is NULL, an exception is thrown.
                        dtTemp = sqlJobs.EnumerateAllSQLJobs(jobsList);

                        // If the dataTable is empty you must clone the table to get the structure.
                        if (dt == null && dtTemp != null)
                        {
                            dt = dtTemp.Clone();
                        }

                        foreach (DataRow dr in dtTemp.Rows)
                        {
                            dt.ImportRow(dr);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Write(ex.Message + ex.StackTrace, SiteMap.CurrentNode.Description);
                        DisplayError(ex.Message);
                    }
                }
            }

            if (dt != null)
            {
                if (searchKeyword != null && searchKeyword.Length > 0)
                {
                    dt.DefaultView.RowFilter = "JobName LIKE '%" + searchKeyword + "%' OR JobExecutionStatus LIKE '%" + searchKeyword + "%' OR JobServer LIKE '%" + searchKeyword + "%'";
                }
            }
            else
            {
                if (searchKeyword != null && searchKeyword.Length > 0)
                {
                    DisplayError("There were no record(s) found, to start a search.");
                }
            }

            gridSQLJobs.DataSource = dt;
            gridSQLJobs.DataBind();
            gridSQLJobs.Visible = true;
            count = gridSQLJobs.Rows.Count;
            UpdateLabel();
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            System.Diagnostics.Debug.Write(ex.Message + ex.StackTrace, SiteMap.CurrentNode.Description);
            gridSQLJobs.Visible = false;
        }
    }
    
    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        errorImg.Visible = true;
        lblError.Font.Bold = false;
    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        SearchUserControl search = sender as SearchUserControl;
        PopulateGrid(search.SearchKeyword);
    }

    protected void gridSQLJobs_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblIsJobEnabled = e.Row.Cells[2].FindControl("lblIsJobEnabled") as Label;

            if (lblIsJobEnabled != null)
            {
                if (lblIsJobEnabled.Text.Equals("True"))
                {
                    lblIsJobEnabled.Text = "Yes";
                    lblIsJobEnabled.ForeColor = Color.White;
                    lblIsJobEnabled.BackColor = Color.Teal;
                    lblIsJobEnabled.ToolTip = "This job is enabled.";
                }
                else
                {
                    lblIsJobEnabled.Text = "No";
                    lblIsJobEnabled.ForeColor = Color.White;
                    lblIsJobEnabled.BackColor = Color.RosyBrown;
                    lblIsJobEnabled.ToolTip = "This job is NOT enabled.";
                }
            }

            //lblJobResult
            Label lblJobResult = e.Row.Cells[4].FindControl("lblJobResult") as Label;

            if (lblJobResult != null)
            {
                if (lblJobResult.Text.Equals("Succeeded"))
                {
                    lblJobResult.Text = "OK";
                    lblJobResult.ForeColor = Color.White;
                    lblJobResult.BackColor = Color.Teal;
                    lblJobResult.ToolTip = "This job has succeeded.";
                }
                else
                {
                    lblJobResult.ForeColor = Color.White;
                    lblJobResult.BackColor = Color.RosyBrown;
                    lblJobResult.ToolTip = "This job has NOT succeeded.";
                }
            }

            Label lblLastRun = e.Row.Cells[5].FindControl("lblLastRun") as Label;

            if (lblLastRun != null)
            {
                string dateTimeVal = lblLastRun.Text;
                lblLastRun.Text = DateTime.Parse(dateTimeVal).ToShortTimeString();
                lblLastRun.ForeColor = Color.White;
                lblLastRun.BackColor = Color.Gray;
                lblLastRun.ToolTip = dateTimeVal;
            }

            Label lblNextRun = e.Row.Cells[6].FindControl("lblNextRun") as Label;

            if (lblNextRun != null)
            {
                string dateTimeVal2 = lblNextRun.Text;
                lblNextRun.Text = DateTime.Parse(dateTimeVal2).ToShortTimeString();
                lblNextRun.ForeColor = Color.White;
                lblNextRun.BackColor = Color.Peru;
                lblNextRun.ToolTip = dateTimeVal2;
            }

            Label lblJobStatus = e.Row.Cells[3].FindControl("lblJobStatus") as Label;

            if (lblJobStatus != null)
            {
                if (lblJobStatus.Text.Contains("Fail"))
                {
                    lblJobStatus.ForeColor = Color.White;
                    lblJobStatus.BackColor = Color.RosyBrown;
                    lblJobStatus.ToolTip = "This job has failed, check with your administrator.";
                }
                else if (lblJobStatus.Text.Contains("Executing"))
                {
                    lblJobStatus.ForeColor = Color.White;
                    lblJobStatus.BackColor = Color.Teal;
                    lblJobStatus.ToolTip = "This job is executing, wait for it to complete.";
                }
                else if (lblJobStatus.Text.Equals("Idle"))
                {
                    lblJobStatus.ForeColor = Color.White;
                    lblJobStatus.BackColor = Color.Gray;
                    lblJobStatus.ToolTip = "This job is idle, check the next run time.";
                }
                else
                {
                    lblJobStatus.ForeColor = Color.White;
                    lblJobStatus.BackColor = Color.Gray;
                    lblJobStatus.ToolTip = "This job is " + lblJobStatus.Text + ".";
                }
            }

            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");
        }
    }

    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
        bccOperator.SetCheckAllCheckBox(gridSQLJobs, "chkJobID", true);
    }

    protected void btnDeSelectAll_Click(object sender, EventArgs e)
    {
        bccOperator.SetCheckAllCheckBox(gridSQLJobs, "chkJobID", false);
    }

    protected void btnEnableDisable_Click(object sender, EventArgs e)
    {
        ToggleStatus();
        PopulateGrid(string.Empty);
    }

    private bool ConvertStringToBool(string status)
    {
        if (status.ToLower().Equals("yes"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ToggleStatus()
    {
        string errorMsg = string.Empty;
        string jobName = string.Empty;
        BCCJobsHelper sqlJobs = null;
        bool isEnabled = false;
        // Iterate through the Gridview Rows property
        foreach (GridViewRow row in gridSQLJobs.Rows)
        {
            HiddenField connectionString = row.FindControl("jobConnectionString") as HiddenField;
            Label lblIsJobEnabled = row.FindControl("lblIsJobEnabled") as Label;

            sqlJobs = new BCCJobsHelper(connectionString.Value);
            jobName = row.Cells[1].Text;
            isEnabled = ConvertStringToBool(lblIsJobEnabled.Text);
            
            // Access the CheckBox
            CheckBox cb = (CheckBox)row.FindControl("chkJobID");

            if (cb != null && cb.Checked)
            {
                try
                {
                    if (!isEnabled)
                    {
                        sqlJobs.EnableJob(jobName, true);
                        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "enabled job " + jobName, 302);
                    }
                    else
                    {
                        sqlJobs.EnableJob(jobName, false);
                        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "disabled job " + jobName, 302);
                    }
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                    System.Diagnostics.Debug.Write(ex.Message + ex.StackTrace, SiteMap.CurrentNode.Description);
                }
            }
        }
    }
    
    private void OperateJobs(bool isEnabled)
    {
        string errorMsg = string.Empty;
        string jobName = string.Empty;
        BCCJobsHelper sqlJobs = null;

        // Iterate through the Gridview Rows property
        foreach (GridViewRow row in gridSQLJobs.Rows)
        {
            HiddenField connectionString = (HiddenField)row.FindControl("jobConnectionString");
            sqlJobs = new BCCJobsHelper(connectionString.Value);
            jobName = row.Cells[1].Text;
            // Access the CheckBox
            CheckBox cb = (CheckBox)row.FindControl("chkJobID");
            if (cb != null && cb.Checked)
            {
                try
                {
                    if (isEnabled)
                    {   
                        sqlJobs.StartJob(jobName, true);
                        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "started job " + jobName, 302);
                    }
                    else
                    {
                        sqlJobs.StartJob(jobName, false);
                        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "stopped job " + jobName, 302);
                    }
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                    System.Diagnostics.Debug.Write(ex.Message + ex.StackTrace, SiteMap.CurrentNode.Description);
                }
            }
        }
    }

    protected void btnStart_Click(object sender, EventArgs e)
    {
        OperateJobs(true);
        PopulateGrid(string.Empty);
    }

    protected void btnStop_Click(object sender, EventArgs e)
    {
        OperateJobs(false);
        PopulateGrid(string.Empty);
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        HtmlForm myMasterForm = (HtmlForm)Master.FindControl("form1");

        if (myMasterForm != null)
        {
            myMasterForm.Controls.Add(gridSQLJobs);
        }

        BCCGridView.Export("ControlCenter_" + System.Environment.MachineName + "_SQLServerJobs.xls", this.gridSQLJobs);
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        // DO NOT : remove this method. 
    }
}
