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
using System.Diagnostics;
using System.Globalization;
using BCC.Core;
using System.Collections.Specialized;
using System.Drawing;
using BCC.Core.WMI.BizTalk;

public partial class EventLogs : System.Web.UI.Page
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
        InitializeEventCategoryTabs();
    }

    private void InitializeObjects()
    {
        lblError.Visible = false;
        errorImg.Visible = false;
        lblStatus.Visible = false;
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";
    }
    
    protected void LogGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        LogGrid.PageIndex = e.NewPageIndex;
        BindEventLogGrid(category.Value, rowFilter.Value);
    }

    protected void LogGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int position = 1;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (BCCUIHelper.Constants.LOG_EVENT_WARN.Equals(e.Row.Cells[position].Text))
            {
                e.Row.Cells[position].ForeColor = Color.Maroon;
            }
            else if (BCCUIHelper.Constants.LOG_EVENT_ERR.Equals(e.Row.Cells[position].Text))
            {
                e.Row.Cells[position].ForeColor = Color.Red;
            }
        }

        if (e.Row.RowIndex != -1)
        {
            // Last modified on
            int compare = System.DateTime.Compare(DateTime.Parse(e.Row.Cells[3].Text), System.DateTime.Today);

            if (compare > 0)
            {
                e.Row.Cells[3].ForeColor = Color.Teal;
                //e.Row.Cells[3].BackColor = Color.LightGray;
            }
        }
    }
    
    protected void LogGrid_Sort(object sender, GridViewSortEventArgs e)
    {
        //this.BindEventLogGrid(category.Value, filterExpr.Text);
    }

    /// <summary>
    /// Initialize - Event - Category - Tabs
    /// </summary>
    private void InitializeEventCategoryTabs()
    {
        StringCollection eventCategoryList = null;
        // Use filters here.
        // Check whether the Profile is active and then proceed.
        if (Profile.ControlCenterProfile.IsProfileActive)
        {
            try
            {
                BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                eventCategoryList = props.ModuleDictionary[BCCUIHelper.Constants.EVENT_PROFILE_KEY];
            }
            catch
            {
                // Ignore errors from Profile system
            }
        }

        if (eventCategoryList != null)
        {
            LinkButton linkButton = null;
            System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
            img.ImageUrl = @"~\Images\bullet.png";
            img.Visible = true;

            linkButtonGroup.Controls.Add(new LiteralControl("Category "));
            linkButtonGroup.Controls.Add(img);

            ListItem liEventCat = null;

            foreach (string categoryName in eventCategoryList)
            {
                linkButton = new LinkButton();
                linkButton.ID = categoryName;
                linkButton.Text = categoryName;
                linkButton.CssClass = "linkConfig";
                linkButton.ForeColor = Color.Teal;
                linkButton.Click += new EventHandler(linkButton_Click);
                linkButton.ToolTip = "View " + categoryName;
                linkButtonGroup.Controls.Add(linkButton);
                linkButtonGroup.Controls.Add(new LiteralControl("|"));

                liEventCat = new ListItem();
                liEventCat.Text = categoryName + " event log";
                liEventCat.Value = categoryName;

                if (eventLogList.Items.Count != eventCategoryList.Count)
                {
                    eventLogList.Items.Add(liEventCat);
                }

            }
        }
    }

    protected void btnMonitor_Click(object sender, EventArgs e)
    {
        foreach (ListItem liEventCat in eventLogList.Items)
        {
            if (liEventCat.Selected)
            {
                try
                {
                    BCCMonitoringDataAccess da = new BCCMonitoringDataAccess();
                    da.CreateMonitoringEntry(ArtifactType.EventLog, liEventCat.Value);
                    new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, " setup monitoring for " + liEventCat.Value + " event log", 401);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex.Message + ex.StackTrace);
                }
            }
        }
    }

    private void BindEventLogGrid(string categoryName, string rowFilter)
    {
        EventLog aLog = new EventLog();
        aLog.MachineName = System.Environment.MachineName;
        aLog.Log = categoryName;

        DataTable dt = PopulateEventLogEntries(aLog.Entries);

        if (rowFilter != string.Empty)
        {
            dt.DefaultView.RowFilter = filterExpr.Text;
        }

        dt.DefaultView.Sort = "DateTime DESC";
        LogGrid.DataSource = dt;
        LogGrid.DataBind();
        eventLogPanel.Visible = true;
    }

    private DataTable PopulateEventLogEntries(EventLogEntryCollection logCollection)
    {
        DataTable dt = new DataTable();

        // define the table's schema
        dt.Columns.Add(new DataColumn("EventID", typeof(string)));
        dt.Columns.Add(new DataColumn("EntryType", typeof(string)));
        dt.Columns.Add(new DataColumn("Source", typeof(string)));
        dt.Columns.Add(new DataColumn("DateTime", typeof(DateTime)));
        dt.Columns.Add(new DataColumn("Message", typeof(string)));

        DataRow dr = null;

        foreach (EventLogEntry entry in logCollection)
        {
            dr = dt.NewRow();

            dr[0] = entry.InstanceId;
            dr[1] = entry.EntryType;
            dr[2] = entry.Source;
            dr[3] = entry.TimeGenerated;
            dr[4] = entry.Message;

            dt.Rows.Add(dr);
        }

        return dt;
    }

    protected void linkButton_Click(object sender, EventArgs e)
    {
        LinkButton lnkButton = sender as LinkButton;
        category.Value = lnkButton.Text;
        // Reset the filter
        filterExpr.Text = "";
        rowFilter.Value = "";
        // Panel labelling
        eventLogPanel.GroupingText = lnkButton.Text + " log on machine " + System.Environment.MachineName;
        
        BindEventLogGrid(category.Value, string.Empty);
    }

    private void DisplayError(string message)
    {
        lblError.Visible = true;
        errorImg.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = false;
    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        if (filterExpr.Text != string.Empty && filterExpr.Text.Length > 0)
        {
            if (category.Value != string.Empty)
            {
                try
                {
                    eventLogPanel.GroupingText = category.Value + " log on machine " + System.Environment.MachineName;
                    rowFilter.Value = filterExpr.Text;
                    this.BindEventLogGrid(category.Value, filterExpr.Text);
                    new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, " searched for " + filterExpr.Text, 401);
                }
                catch (Exception exception)
                {
                    this.DisplayError(exception.Message);
                }

            }
        }
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        HtmlForm myMasterForm = (HtmlForm)Master.FindControl("form1");

        if (myMasterForm != null)
        {
            myMasterForm.Controls.Add(LogGrid);
        }

        BCCGridView.Export("ControlCenter_" + System.Environment.MachineName + "_EventLog.xls", this.LogGrid);
    }
}
