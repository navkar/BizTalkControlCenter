using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;

using BCC.Core;
using BCC.Core.Task;

/// <summary>
/// Task Efforts
/// </summary>
public partial class ProjectTasksReport : System.Web.UI.Page
{
    private string projectName = string.Empty;
    private DateTime reportStartDate = DateTime.Today.Subtract(new TimeSpan(30, 0, 0, 0));
    private DateTime reportEndDate = DateTime.Today;
    private double totalTaskEffort = 0;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        projectName = this.Request.QueryString["projectName"];
        
        if (!Page.IsPostBack)
        {
            InitializeReports();
            BindProjectGrid(projectName, reportStartDate, reportEndDate);
            ViewState["TE" + "SortDirection"] = "ASC";
        }
    }

    #region Project View Grid event handlers

    /// <summary>
    /// BindProjectGrid
    /// </summary>
    /// <param name="projectName"></param>
    /// <param name="reportStartDate"></param>
    /// <param name="reportEndDate"></param>
    private void BindProjectGrid(string projectName, DateTime reportStartDate, DateTime reportEndDate)
    {
        BCCTaskEffort effort = new BCCTaskEffort();
        effort.ProjectName = projectName;
        effort.ReportStartDate = reportStartDate;
        effort.ReportEndDate = reportEndDate;

        headerCaption.Text = string.Format("Project Task Report - ({0})", projectName);

        DataTable dtTaskEffort = BCCTaskEffort.ReportProjectEfforts(effort);

        if (dtTaskEffort != null)
        {
            projectEffortView.DataSource = dtTaskEffort;
            projectEffortView.DataBind();
            projectEffortView.Visible = true;
        }
    }

    private void InitializeReports()
    {
        // Start date is 30 days behind UTC today
        tStartDate.Text = System.DateTime.UtcNow.Subtract(new TimeSpan(30, 0, 0, 0)).ToString("MMM-dd-yyyy");
        tEndDate.Text = System.DateTime.UtcNow.ToString("MMM-dd-yyyy");
    }

    protected string FormatTaskName(object taskID, object taskName)
    {
        return "#" + taskID + " " + taskName;
    }

    protected void taskEffortView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");

            Label lblAssignedToUserName = e.Row.Cells[0].FindControl("lblAssignedToUserName") as Label;
            Label lblTaskPriority = e.Row.Cells[1].FindControl("lblTaskPriority") as Label; //1
            Label lblTaskStatus = e.Row.Cells[3].FindControl("lblTaskStatus") as Label; //3
            Label lblTaskHours = e.Row.Cells[4].FindControl("lblTaskHours") as Label; //2
            HiddenField hTaskHours = e.Row.Cells[4].FindControl("hTaskHours") as HiddenField;

            if (lblAssignedToUserName.Text.Equals(HttpContext.Current.User.Identity.Name))
            {
                lblAssignedToUserName.ForeColor = Color.White;
                lblAssignedToUserName.BackColor = Color.Gray;
            }
            else
            {
                lblAssignedToUserName.ForeColor = Color.White;
                lblAssignedToUserName.BackColor = Color.RosyBrown;
            }

            if (lblTaskStatus != null &&
                    (lblTaskStatus.Text == Enum.GetName(typeof(BCCTask.TaskState), BCCTask.TaskState.Created)
                    || lblTaskStatus.Text == Enum.GetName(typeof(BCCTask.TaskState), BCCTask.TaskState.Pending)
                    || lblTaskStatus.Text == Enum.GetName(typeof(BCCTask.TaskState), BCCTask.TaskState.Deferred)
                    ))
            {
                lblTaskStatus.BackColor = Color.Khaki;
            }

            if (lblTaskPriority != null)
            {
                lblTaskPriority.ForeColor = Color.White;

                if (lblTaskPriority.Text.Equals("1"))
                {
                    lblTaskPriority.BackColor = Color.Orange;
                    lblTaskPriority.ToolTip = "High Priority";
                }
                else if (lblTaskPriority.Text.Equals("2"))
                {
                    lblTaskPriority.BackColor = Color.RoyalBlue;
                    lblTaskPriority.ToolTip = "Medium Priority";
                }
                else if (lblTaskPriority.Text.Equals("3"))
                {
                    lblTaskPriority.BackColor = Color.Teal;
                    lblTaskPriority.ToolTip = "Low Priority";
                }
            }

            if (hTaskHours != null)
            {
                double taskEffort = 0;

                if (Double.TryParse(hTaskHours.Value, out taskEffort))
                {
                    totalTaskEffort += taskEffort;
                }
            }

        } // end-if
        else if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblEffortSum = e.Row.Cells[3].FindControl("lblEffortSum") as Label;
            lblEffortSum.Text = totalTaskEffort + " hrs&nbsp;&nbsp;";
        }

    }

    protected void lnkViewReport_Click(object sender, EventArgs e)
    {

        DateTime startDate = DateTime.Today.Subtract(new TimeSpan(30, 0, 0, 0));
        reportStartDate = startDate;

        if (DateTime.TryParse(tStartDate.Text, out startDate))
        {
            reportStartDate = startDate;
        }

        DateTime endDate = DateTime.Today;
        reportEndDate = endDate;

        if (DateTime.TryParse(tEndDate.Text, out endDate))
        {
            reportEndDate = endDate;
        }

        BindProjectGrid(projectName, reportStartDate, reportEndDate);
    }

    protected void taskEffortView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        projectEffortView.PageIndex = e.NewPageIndex;

        BindProjectGrid(projectName, reportStartDate, reportEndDate);
    }

    protected void taskEffortView_Sorting(object sender, GridViewSortEventArgs e)
    {
        BCCTaskEffort effort = new BCCTaskEffort();
        effort.ProjectName = projectName;
        effort.ReportStartDate = reportStartDate;
        effort.ReportEndDate = reportEndDate;

        DataTable projectEffort = BCCTaskEffort.ReportProjectEfforts(effort);

        if (projectEffort != null)
        {
            DataView dataView = new DataView(projectEffort);
            dataView.Sort = e.SortExpression + " " + ToggleDirection();
            projectEffortView.DataSource = dataView;
            projectEffortView.DataBind();
        }
    }

    private string ToggleDirection()
    {
        string newSortDirection = ViewState["TE" + "SortDirection"] as string;

        switch (newSortDirection)
        {
            case "DESC":
                newSortDirection = "ASC";
                break;

            case "ASC":
                newSortDirection = "DESC";
                break;
        }

        ViewState["TE" + "SortDirection"] = newSortDirection;
        return newSortDirection;
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        HtmlForm myMasterForm = Page.FindControl("reportForm") as HtmlForm;

        if (myMasterForm != null)
        {
            myMasterForm.Controls.Add(projectEffortView);
        }

        BCCGridView.Export("ControlCenter_" + projectName + ".xls", this.projectEffortView);
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
    }

    #endregion

}