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
public partial class TaskEfforts : System.Web.UI.Page
{
    private string taskID = string.Empty;
    private string taskName = string.Empty;
    private string projectName = string.Empty;

    /// <summary>
    /// Page - Load 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        taskID = this.Request.QueryString["taskID"];
        taskName = this.Request.QueryString["taskName"];
        projectName = this.Request.QueryString["projectName"];

        if (!Page.IsPostBack)
        {
            BindTaskEffortGrid(taskID, taskName, projectName);
            ViewState["TE" + "SortDirection"] = "ASC";
        }
    }

    protected string TodaysDate
    {
        get
        {
            return DateTime.UtcNow.Date.ToString("MMM-dd-yyyy");
        }
    }

    protected string CurrentUser
    {
        get
        {
            return HttpContext.Current.User.Identity.Name;
        }
    }

    protected DataTable TaskStatusList
    {
        get
        {
            DataTable dt = new DataTable();
            DataRow dr = null;
            dt.Columns.Add(new DataColumn("taskStatus", typeof(string)));

            string[] taskStates = Enum.GetNames(typeof(BCC.Core.Task.BCCTask.TaskState));

            foreach (string taskState in taskStates)
            {
                dr = dt.NewRow();
                dr[0] = taskState;
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }

    private void FillTaskEfforts(Guid taskID)
    {
        BCCTaskEffort effort = new BCCTaskEffort();

        effort.TaskID = taskID;

        // Get me all the active tasks.
        DataTable dtTasks = BCCTaskEffort.RetrieveAllTaskEfforts(effort);

        if (dtTasks != null)
        {
            taskEffortView.DataSource = dtTasks;
            taskEffortView.DataBind();
            taskEffortView.Visible = true;
        }
    }

    #region Task Effort View GridView handlers

    private void BindTaskEffortGrid(string taskId, string taskName, string projectName)
    {
        Guid taskGuid = new Guid(taskId);

        BCCTaskEffort effort = new BCCTaskEffort();
        effort.TaskID = taskGuid;

        lblTaskEffortCaption.Text = string.Format("Project: {0}, Task: {1}", projectName, taskName);

        // Retrieves all the effort for a specific task.
        DataTable dtTaskEffort = BCCTaskEffort.RetrieveAllTaskEfforts(effort);

        if (dtTaskEffort != null)
        {
            taskEffortView.DataSource = dtTaskEffort;
            taskEffortView.DataBind();
            taskEffortView.Visible = true;
        }
    }

    protected void taskEffortView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");

            Label lblAssignedToUserName = e.Row.Cells[0].FindControl("lblAssignedToUserName") as Label;
            Label lblTaskDueDate = e.Row.Cells[1].FindControl("lblTaskDate") as Label; //1
            Label lblTaskHours = e.Row.Cells[2].FindControl("lblTaskHours") as Label; //2
            Label lblTaskStatus = e.Row.Cells[3].FindControl("lblTaskStatus") as Label; //3

            LinkButton lnkEdit = e.Row.Cells[5].FindControl("lnkEdit") as LinkButton;
            LinkButton lnkDelete = e.Row.Cells[5].FindControl("lnkDelete") as LinkButton;

            if (lblAssignedToUserName.Text.ToLower().Equals(HttpContext.Current.User.Identity.Name.ToLower()))
            {
                lblAssignedToUserName.ForeColor = Color.White;
                lblAssignedToUserName.BackColor = Color.Gray;
            }
            else
            {
                lblAssignedToUserName.ForeColor = Color.White;
                lblAssignedToUserName.BackColor = Color.RosyBrown;

                // Disable editing and deleting if possible.
                if (lnkEdit != null && lnkDelete != null)
                {
                    System.Web.UI.WebControls.Image editImage = e.Row.Cells[5].FindControl("editImage") as System.Web.UI.WebControls.Image;
                    System.Web.UI.WebControls.Image deleteImage = e.Row.Cells[5].FindControl("deleteImage") as System.Web.UI.WebControls.Image;

                    lnkEdit.Enabled = false;
                    lnkEdit.OnClientClick = "javascript:alert('You dont have the permission to edit this record');";
                    editImage.ToolTip = "You don't have the permission to edit this record";

                    lnkDelete.Enabled = false;
                    lnkDelete.OnClientClick = "javascript:alert('You dont have the permission to delete this record');";
                    deleteImage.ToolTip = "You don't have the permission to delete this record";
                }
            }

            if (lblTaskStatus != null &&
                    (lblTaskStatus.Text == Enum.GetName(typeof(BCCTask.TaskState), BCCTask.TaskState.Created)
                    || lblTaskStatus.Text == Enum.GetName(typeof(BCCTask.TaskState), BCCTask.TaskState.Pending)
                    || lblTaskStatus.Text == Enum.GetName(typeof(BCCTask.TaskState), BCCTask.TaskState.Deferred)
                    ))
            {
                lblTaskStatus.BackColor = Color.Khaki;
            }

            if (lblTaskDueDate != null)
            {
                int dueDayAfterTomorrow = System.DateTime.Compare(DateTime.Parse(lblTaskDueDate.Text), System.DateTime.Today.AddDays(2));
                int dueTomorrow = System.DateTime.Compare(DateTime.Parse(lblTaskDueDate.Text), System.DateTime.Today.AddDays(1));
                int dueToday = System.DateTime.Compare(DateTime.Parse(lblTaskDueDate.Text), System.DateTime.Today);
                int dueYesterday = System.DateTime.Compare(DateTime.Parse(lblTaskDueDate.Text), System.DateTime.Today.Subtract(new TimeSpan(1, 0, 0, 0)));

                //  Day after tomorrow                
                if (dueDayAfterTomorrow == 0)
                {
                    lblTaskDueDate.Text = DateTime.Today.AddDays(2).DayOfWeek.ToString();
                    lblTaskDueDate.BackColor = Color.NavajoWhite;
                    lblTaskDueDate.ToolTip = DateTime.Today.AddDays(2).ToLongDateString();
                }
                else
                    //  Tomorrow                
                    if (dueTomorrow == 0)
                    {
                        lblTaskDueDate.Text = "Tomorrow";
                        lblTaskDueDate.BackColor = Color.PaleVioletRed;
                        lblTaskDueDate.ForeColor = Color.White;
                        lblTaskDueDate.ToolTip = DateTime.Today.AddDays(1).ToLongDateString();
                    }
                    else
                        // Today     
                        if (dueToday == 0)
                        {
                            lblTaskDueDate.Text = "Today";
                            lblTaskDueDate.BackColor = Color.SandyBrown;
                            lblTaskDueDate.ForeColor = Color.White;
                            lblTaskDueDate.ToolTip = DateTime.Today.ToLongDateString();
                        }
                        else
                            // Yesterday              
                            if (dueYesterday == 0)
                            {
                                lblTaskDueDate.Text = "Yesterday";
                                lblTaskDueDate.BackColor = Color.PeachPuff;
                                lblTaskDueDate.ToolTip = System.DateTime.Today.Subtract(new TimeSpan(1, 0, 0, 0)).ToLongDateString();
                            }
                            else
                                // Older than yesterday
                                if (dueYesterday < 0)
                                {
                                    lblTaskDueDate.BackColor = Color.LightGray;
                                }

            }

        }
    }

    protected void taskEffortView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Add"))
        {
            TextBox tNewTaskDate = taskEffortView.FooterRow.FindControl("tNewTaskDate") as TextBox;
            TextBox tNewTaskHours = taskEffortView.FooterRow.FindControl("tNewTaskHours") as TextBox;
            DropDownList ddlNewTaskStatus = taskEffortView.FooterRow.FindControl("ddlNewTaskStatus") as DropDownList;
            TextBox tNewTaskRemark = taskEffortView.FooterRow.FindControl("tNewTaskRemark") as TextBox;

            Guid taskGuid = new Guid();

            if (taskID != null)
            {
                taskGuid = new Guid(taskID);
            }

            if (taskID != null && tNewTaskDate != null && ddlNewTaskStatus != null && tNewTaskHours != null && tNewTaskRemark != null)
            {
                BCCTaskEffort effort = new BCCTaskEffort();

                effort.TaskID = taskGuid;
                effort.TaskAssignedToUserName = HttpContext.Current.User.Identity.Name;

                DateTime today = DateTime.UtcNow.Date;
                DateTime.TryParse(tNewTaskDate.Text, out today);
                effort.TaskEffortDate = today;

                effort.TaskStatus = ddlNewTaskStatus.SelectedValue;
                effort.TaskRemark = tNewTaskRemark.Text;

                double taskEffort = 0;
                Double.TryParse(tNewTaskHours.Text, out taskEffort);
                effort.TaskEffort = taskEffort;

                BCCTaskEffort.CreateTaskEffort(effort);
            }

            FillTaskEfforts(taskGuid);
        }
        else if (e.CommandName.Equals("AddOnEmpty"))
        {
            TextBox tDate = taskEffortView.Controls[0].Controls[0].FindControl("tDate") as TextBox;
            DropDownList ddlTaskStatus2 = taskEffortView.Controls[0].Controls[0].FindControl("ddlTaskStatus2") as DropDownList;
            TextBox tRemark = taskEffortView.Controls[0].Controls[0].FindControl("tRemark") as TextBox;
            TextBox tEffort = taskEffortView.Controls[0].Controls[0].FindControl("tEffort") as TextBox;

            Guid taskGuid = new Guid();

            if (taskID != null)
            {
                taskGuid = new Guid(taskID);
            }

            if (taskID != null && tDate != null && ddlTaskStatus2 != null && tRemark != null && tEffort != null)
            {
                BCCTaskEffort effort = new BCCTaskEffort();

                effort.TaskID = taskGuid;
                effort.TaskAssignedToUserName = HttpContext.Current.User.Identity.Name;

                DateTime today = DateTime.UtcNow.Date;
                DateTime.TryParse(tDate.Text, out today);
                effort.TaskEffortDate = today;

                effort.TaskStatus = ddlTaskStatus2.SelectedValue;
                effort.TaskRemark = tRemark.Text;

                double taskEffort = 0;
                Double.TryParse(tEffort.Text, out taskEffort);
                effort.TaskEffort = taskEffort;

                BCCTaskEffort.CreateTaskEffort(effort);
            }

            FillTaskEfforts(taskGuid);
        }
 
    }

    protected void taskEffortView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        taskEffortView.PageIndex = e.NewPageIndex;

        Guid taskGuid = new Guid(taskID);
        FillTaskEfforts(taskGuid);
    }

    protected void taskEffortView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        taskEffortView.EditIndex = e.NewEditIndex;

        Guid taskGuid = new Guid(taskID);
        FillTaskEfforts(taskGuid);
    }

    protected void taskEffortView_RowCancelEditing(object sender, GridViewCancelEditEventArgs e)
    {
        taskEffortView.EditIndex = -1;

        Guid taskGuid = new Guid(taskID);
        FillTaskEfforts(taskGuid);
    }

    protected void taskEffortView_Sorting(object sender, GridViewSortEventArgs e)
    {
        Guid taskGuid = new Guid(taskID);
        BCCTaskEffort effort = new BCCTaskEffort();
        effort.TaskID = taskGuid;

        DataTable dtTasks = BCCTaskEffort.RetrieveAllTaskEfforts(effort);

        if (dtTasks != null)
        {
            DataView dataView = new DataView(dtTasks);
            dataView.Sort = e.SortExpression + " " + ToggleDirection();
            taskEffortView.DataSource = dataView;
            taskEffortView.DataBind();
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

    protected void taskEffortView_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        GridViewRow deletedRow = taskEffortView.Rows[e.RowIndex] as GridViewRow;
        Guid taskGuid = new Guid();

        if (deletedRow != null)
        {
            HiddenField hGuid = deletedRow.FindControl("taskID") as HiddenField;
            HiddenField taskRef = deletedRow.FindControl("taskRef") as HiddenField;

            taskGuid = new Guid(hGuid.Value);

            BCCTaskEffort taskEffort = new BCCTaskEffort();
            taskEffort.TaskID = taskGuid;
            taskEffort.TaskRef = taskRef.Value;
            // User trying to delete
            taskEffort.TaskAssignedToUserName = HttpContext.Current.User.Identity.Name;

            BCCTaskEffort.RemoveTaskEfforts(taskEffort);
        }

        // Refresh the data
        taskEffortView.EditIndex = -1;
        FillTaskEfforts(taskGuid);
    }

    protected void taskEffortView_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridViewRow updatableRow = taskEffortView.Rows[e.RowIndex] as GridViewRow;
        Guid taskGuid = new Guid();

        if (updatableRow != null)
        {
            HiddenField htaskID = updatableRow.FindControl("taskID") as HiddenField;
            HiddenField taskRef = updatableRow.FindControl("taskRef") as HiddenField;

            TextBox tTaskDate = updatableRow.FindControl("tTaskDate") as TextBox;
            DropDownList ddlTaskStatus = updatableRow.FindControl("ddlTaskStatus") as DropDownList;
            TextBox tTaskRemark = updatableRow.FindControl("tTaskRemark") as TextBox;
            TextBox tTaskHours = updatableRow.FindControl("tTaskHours") as TextBox;

            if (htaskID != null)
            {
                taskGuid = new Guid(htaskID.Value);

                BCCTaskEffort effort = new BCCTaskEffort();
                effort.TaskID = taskGuid;
                effort.TaskRef = taskRef.Value;

                effort.TaskAssignedToUserName = HttpContext.Current.User.Identity.Name;

                DateTime today = DateTime.Today;
                DateTime.TryParse(tTaskDate.Text, out today);
                effort.TaskEffortDate = today;

                effort.TaskStatus = ddlTaskStatus.SelectedValue;
                effort.TaskRemark = tTaskRemark.Text;

                double taskEffort = 0;
                Double.TryParse(tTaskHours.Text, out taskEffort);
                effort.TaskEffort = taskEffort;

                BCCTaskEffort.UpdateTaskEffort(effort);
            }
        }

        // Refresh the data
        taskEffortView.EditIndex = -1;
        FillTaskEfforts(taskGuid);
    }
    #endregion

}