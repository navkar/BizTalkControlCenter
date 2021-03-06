using System;
using System.Data;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BCC.Core;
using System.Drawing;
using System.Xml;
using System.Data.SqlClient;

public partial class PerfCounters : System.Web.UI.Page
{
    private string sortExpression = "PerfCategoryName";
    private string filterExpression = string.Empty;
    private BCCSortDirection lastDirection = BCCSortDirection.ASC;
    private string CACHE_KEY = "PerfCounters";

    enum BCCSortDirection
    {
        ASC,
        DESC
    }

    protected void Page_PreInit(object sender, EventArgs e)
    {
        string defaultTheme = Profile.ControlCenterProfile.UserTheme;

        if (defaultTheme != null && defaultTheme != string.Empty)
        {
            this.Page.Theme = defaultTheme;
        }

        // This will help all the pages get the session value.
        ViewState["PAGE_THEME"] = this.Page.Theme;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeObjects();

        if (!(User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN) || User.IsInRole(BCCUIHelper.Constants.ROLE_ARTIFACT)))
        {
            DisplayError(BCCUIHelper.Constants.ACCESS_DENIED);
        }
        else
        {
            if (Page.IsPostBack)
            {
                lastDirection = (BCCSortDirection)ViewState[SiteMap.CurrentNode.Description + "SortDirection"];
                sortExpression = ViewState[SiteMap.CurrentNode.Description + "SortExpression"] as string;
            }
            else
            {
                ViewState[SiteMap.CurrentNode.Description + "SortDirection"] = lastDirection;
                ViewState[SiteMap.CurrentNode.Description + "SortExpression"] = sortExpression;
            }
        }
    }

    private void ActivateGrid()
    {
        UserActivityGrid.Visible = true;
    }

    protected void btnCache_Click(object sender, EventArgs e)
    {
        Cache.Remove(CACHE_KEY);
        DisplayError("Performance counter cache is empty. Searching again will re-build the cache.");
        System.Diagnostics.Debug.Write("Cache cleared.", SiteMap.CurrentNode.Description);
    }

    private DataTable FetchPerformanceCounters()
    {
        DataTable dt = Cache[CACHE_KEY] as DataTable;

        if (dt == null)
        {
            BCCPerformanceCounters counters = new BCCPerformanceCounters();
            dt = counters.EnumeratePerformanceCounters();
            Cache.Add(CACHE_KEY, dt, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 5, 0), CacheItemPriority.Default, null);
        }

        if (filterExpression != string.Empty && filterExpression.Length > 0)
        {
            dt.DefaultView.RowFilter = "PerfCategoryName LIKE '%" + filterExpr.Text + "%'";
        }

        return dt;
    }

    private void PopulateGrid(string sortExpression, BCCSortDirection direction)
    {
        try
        {
            DataTable dt = FetchPerformanceCounters();
            dt.DefaultView.Sort = sortExpression + " " + direction;

            gridPerfCounters.DataSource = dt;
            gridPerfCounters.DataBind();
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            System.Diagnostics.Debug.Write(ex.Message + ex.StackTrace, SiteMap.CurrentNode.Description);
            gridPerfCounters.Visible = false;
        }
    }

    private void InitializeObjects()
    {
        lblError.Visible = false;
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";
    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        errorImg.Visible = true;
    }

    protected void ResetError()
    {
        lblError.Visible = false;
        lblError.Text = string.Empty;
        errorImg.Visible = false;
    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        try
        {
            if (filterExpr.Text != null && filterExpr.Text.Length > 0)
            {
                filterExpression = filterExpr.Text;
                PopulateGrid(sortExpression, lastDirection);
                new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, " searched for '" + filterExpr.Text + "'", 303);
                ResetError();
            }
            else
            {
                DisplayError("Specify a Performance counter category name, such as 'BizTalk' or 'Send Adapter'.");
            }
        }
        catch (Exception exception)
        {
            this.DisplayError(exception.Message);
        }
    }

    protected void gridPerfCounters_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Monitor"))
        {
            LinkButton lnkMonitor = e.CommandSource as LinkButton;

            if (lnkMonitor != null)
            {
                GridViewRow row = (GridViewRow)(lnkMonitor.NamingContainer);

                if (row != null)
                {
                    string categoryName = row.Cells[0].Text;
                    string counterName = row.Cells[3].Text;

                    Label lblInstanceName = row.Cells[2].FindControl("lblInstanceName") as Label;
                    string instanceName = lblInstanceName.Text;

                    string logData = string.Format("setup '{0}::{1}' for monitoring", categoryName, counterName);
                    
                    BCCPerfCounterEntry entry = new BCCPerfCounterEntry(categoryName, instanceName, counterName);

                    if (entry.PerfInstance != string.Empty)
                    {
                        BCCPerfCounterDataAccess dataAccess = new BCCPerfCounterDataAccess();
                        dataAccess.CreatePerformanceCounterEntry(entry);

                        lnkMonitor.Text = "Added";
                        lnkMonitor.Enabled = false;
                    }

                    new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, logData, 303);
                }

            }
        }
    }

    protected void gridPerfCounters_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int position = 0;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");

            Label lblInstanceName = e.Row.Cells[position + 2].FindControl("lblInstanceName") as Label;

            if (lblInstanceName.Text.Equals(BCCUIHelper.Constants.SC303_INSTANCE_NOT_FOUND))
            {
                lblInstanceName.ForeColor = Color.White;
                lblInstanceName.BackColor = Color.RosyBrown;
                lblInstanceName.ToolTip = "The performance counter instance was not found, you may want to start it.";

                LinkButton lnkMonitor = e.Row.Cells[position + 3].FindControl("lnkMonitor") as LinkButton;
 
                if (lnkMonitor != null)
                {
                    lnkMonitor.Enabled = false;
                    lnkMonitor.ToolTip = "The performance counter instance was not found, you may want to start it.";
                }
            }
        }
    }

    protected void gridPerfCounters_OnSorting(object sender, GridViewSortEventArgs e)
    {
        string sortExpression2 = e.SortExpression;

        if (lastDirection == BCCSortDirection.ASC)
        {
            lastDirection = BCCSortDirection.DESC;
        }
        else
        {
            lastDirection = BCCSortDirection.ASC;
        }

        ViewState[SiteMap.CurrentNode.Description + "SortDirection"] = lastDirection;
        ViewState[SiteMap.CurrentNode.Description + "SortExpression"] = sortExpression2;
        PopulateGrid(sortExpression2, lastDirection);
    }

    protected void gridPerfCounters_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        gridPerfCounters.PageIndex = e.NewPageIndex;
        PopulateGrid(sortExpression, lastDirection);
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
	// DO NOT : remove this method. 
    }
}
