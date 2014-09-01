using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BCC.Core;
using BCC.Core.Task;

/// <summary>
/// TaskList - Task List
/// </summary>
public partial class TaskList : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCOperator hubOperator = new BCCOperator();
    BCCUIHelper uiHelper = new BCCUIHelper();
    public int count = 0;
    private double totalTaskEffort = 0;
    private string textToDraw = string.Empty;
    private string sortDirection = "ASC";

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
        ActivateMultiView();

        if (!Page.IsPostBack)
        {
            // Databind charts
            DataBindChart();

            // Environments
            FillEnvironments();
            FillArchivedEnvironments();
            // Projects
            FillProjects();
            FillArchivedProjects();
            // Tasks
            FillTasks();
            FillArchivedTasks();
            // Reports
            InitializeReports();
            FillReport(DefaultReport);

            ViewState[SiteMap.CurrentNode.Description + "SortDirection"] = sortDirection;
        }
    }

    #region Page methods
    private void InitializeObjects()
    {
        lblInfo.Visible = false;
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";
    }

    private void DataBindChart()
    {
        // set series members names for the X and Y values 
        taskChart.Series["Default"]["PieLabelStyle"] = "Outside";
        taskChart.Series["Default"]["PieDrawingStyle"] = "SoftEdge";
        taskChart.Series["Default"].XValueMember = "TaskStatus";
        taskChart.Series["Default"].YValueMembers = "Count";
        // Refer - http://blogs.msdn.com/b/alexgor/archive/2008/11/11/microsoft-chart-control-how-to-using-keywords.aspx
        taskChart.Series["Default"].Label = "#VALX (#VALY)";

        // data bind to the selected data source
        taskChart.DataBind();
        taskChart.Visible = true;
    }

    protected DataTable Environments
    {
        get
        {
            BCCTaskEnvironment env = new BCCTaskEnvironment();
            env.EnvVisible = 1;
            return BCCTaskEnvironment.RetrieveEnvironments(env);
        }
    }

    protected DataTable Projects
    {
        get
        {
            BCCTaskProject project = new BCCTaskProject();
            project.ProjectVisible = 1;
            return BCCTaskProject.RetrieveProjects(project);
        }
    }

    protected DataTable SpeedcodeList
    {
        get
        {
            DataTable dt = new DataTable();
            DataRow dr = null;

            dt.Columns.Add(new DataColumn("speedcode", typeof(string)));
            dt.Columns.Add(new DataColumn("speedcodeDesc", typeof(string)));

            SiteMapNode root = SiteMap.RootNode;
            SiteMapNodeCollection collection = root.GetAllNodes();
            string naviUrl = string.Empty;

            foreach (SiteMapNode node in collection)
            {
                dr = dt.NewRow();

                if (node.Description != string.Empty)
                {
                    dr[0] = node.Description;
                    dr[1] = "[" + node.Description + "] " + node.Title;
                    dt.Rows.Add(dr);
                }
            }

            return dt;
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

    /// <summary>
    /// recurring
    /// </summary>
    protected DataTable TaskTypeList
    {
        get
        {
            DataTable dt = new DataTable();
            DataRow dr = null;
            dt.Columns.Add(new DataColumn("taskType", typeof(string)));

            string[] taskTypeStates = Enum.GetNames(typeof(BCC.Core.Task.BCCTask.TaskTypeState));

            foreach (string taskTypeState in taskTypeStates)
            {
                dr = dt.NewRow();
                dr[0] = taskTypeState;
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }

    protected DataTable UserList
    {
        get
        {
            DataTable dt = new DataTable();
            DataRow dr = null;
            dt.Columns.Add(new DataColumn("userName", typeof(string)));

            MembershipUserCollection userCollection = Membership.GetAllUsers();

            foreach(MembershipUser user in Membership.GetAllUsers())
            {
                dr = dt.NewRow();
                dr[0] = user.UserName;
                dt.Rows.Add(dr);
                
            }

            return dt;
        }
    }
    #endregion
    
    #region Fill methods

    private void FillEnvironments()
    {
        BCCTaskEnvironment env = new BCCTaskEnvironment();
        env.EnvVisible = 1;

        DataTable dtEnv = BCCTaskEnvironment.RetrieveEnvironments(env);

        if (dtEnv != null)
        {
            environmentView.DataSource = dtEnv;
            environmentView.DataBind();
            environmentView.Visible = true;
        }

        FillProjects();
    }

    private void FillArchivedEnvironments()
    {
        BCCTaskEnvironment env = new BCCTaskEnvironment();
        env.EnvVisible = 0;

        DataTable dtEnv = BCCTaskEnvironment.RetrieveEnvironments(env);

        if (dtEnv != null)
        {
            archivedEnvironments.DataSource = dtEnv;
            archivedEnvironments.DataBind();
            archivedEnvironments.Visible = true;
        }

        FillArchivedProjects();
    }

    private void FillProjects()
    {
        BCCTaskProject project = new BCCTaskProject();
        project.ProjectVisible = 1;

        DataTable dtEnv = BCCTaskProject.RetrieveProjects(project);

        if (dtEnv != null)
        {
            projectView.DataSource = dtEnv;
            projectView.DataBind();
            projectView.Visible = true;
        }
    }

    private void FillArchivedProjects()
    {
        BCCTaskProject project = new BCCTaskProject();
        project.ProjectVisible = 0;

        DataTable dtEnv = BCCTaskProject.RetrieveProjects(project);

        if (dtEnv != null)
        {
            archivedProjects.DataSource = dtEnv;
            archivedProjects.DataBind();
            archivedProjects.Visible = true;
        }
    }

    private void FillArchivedTasks()
    {
        BCCTask task = new BCCTask();
        task.TaskVisible = 0;
        // Get me all the active tasks.
        DataTable dtTasks = BCCTask.RetrieveAllTasks(task);

        if (dtTasks != null)
        {
            archivedTasks.DataSource = dtTasks;
            archivedTasks.DataBind();
            archivedTasks.Visible = true;
        }
    }

    private BCCTaskEffort DefaultReport
    {
        get
        {
            BCCTaskEffort effort = new BCCTaskEffort();

            effort.TaskStatus = null;
            effort.ReportStartDate = System.DateTime.UtcNow.Subtract(new TimeSpan(5,0,0,0)).Date;
            effort.ReportEndDate = System.DateTime.UtcNow.Date;
            effort.TaskAssignedToUserName = CurrentUser;

            return effort;
        }
    }

    private void FillReport(BCCTaskEffort effort)
    {
        DataTable dtTasks = BCCTaskEffort.ReportTaskEfforts(effort);

        if (dtTasks != null)
        {
            reportView.DataSource = dtTasks;
            reportView.DataBind();
            reportView.Visible = true;
        }
    }

    private DataTable FillTasks()
    {
        BCCTask task = new BCCTask();
        task.TaskVisible = 1;
        // Get me all the active tasks.

        DataTable dtTasks = null;

        if (ddlOptions.SelectedValue.Equals("1"))
        {
            task.AssignedToUser = HttpContext.Current.User.Identity.Name;
            task.TaskStatus = string.Empty;
            dtTasks = BCCTask.RetrieveTasks(task);
        }
        else if (ddlOptions.SelectedValue.Equals("2")) // All Tasks
        {
            task.TaskStatus = string.Empty;
            dtTasks = BCCTask.RetrieveTasks(task);
        }
        else
        {
            task.TaskStatus = BCCTask.TaskState.Completed.ToString();
            dtTasks = BCCTask.RetrieveTasks(task);
        }
        

        if (dtTasks != null)
        {
            taskView.DataSource = dtTasks;
            taskView.DataBind();
            taskView.Visible = true;
        }

        DataBindChart();

        return dtTasks;
    }

    #endregion

    #region Helper methods

    private void ActivateMultiView()
    {
        object activeViewObj = ViewState["TasksView.ActiveViewIndex"];

        if (activeViewObj != null)
        {
            tasksView.ActiveViewIndex = (int)activeViewObj;
        }
        else
        {
            tasksView.ActiveViewIndex = 0;
            ViewState["TasksView.ActiveViewIndex"] = 0;
        }

        //For dynamically added views, make sure they are added before or in Page_PreInit event.
        //taskView.ActiveViewIndex = 0;
    }

    protected void DisplayError(string message)
    {
        lblInfo.Visible = true;
        lblInfo.Text = message + ".";
        lblInfo.Font.Bold = false;
        errorImg.Visible = true;
        successImg.Visible = false;
    }

    protected void DisplayInfo(string message)
    {
        lblInfo.Visible = true;
        lblInfo.Text = message + ".";
        lblInfo.Font.Bold = false;
        errorImg.Visible = false;
        successImg.Visible = true;
    }

    protected void ResetError()
    {
        lblInfo.Visible = false;
        errorImg.Visible = false;
        successImg.Visible = false;
    }

    protected void tasksView_ActiveViewChanged(object sender, EventArgs e)
    {
        FillEnvironments();
        FillProjects();
        FillTasks();
    }

    protected void btnView_Click(object sender, EventArgs e)
    {
        LinkButton clickedBtn = sender as LinkButton;

        if (clickedBtn != null)
        {
            int activeIndex = 0;

            if (clickedBtn.CommandName.Equals("ViewTasks"))
            {
                activeIndex = 0;
                DataBindChart();
                FillTasks();
            }
            else if (clickedBtn.CommandName.Equals("ViewProjects"))
            {
                activeIndex = 1;
                FillProjects();
                FillArchivedProjects();
            }
            else if (clickedBtn.CommandName.Equals("ViewEnvironments"))
            {
                activeIndex = 2;
                FillEnvironments();
                FillArchivedEnvironments();
            }
            else if (clickedBtn.CommandName.Equals("ViewArchivedTasks"))
            {
                activeIndex = 3;
                FillArchivedTasks();
            }
            else if (clickedBtn.CommandName.Equals("ViewReports"))
            {
                activeIndex = 4;
                FillReport(DefaultReport);
            }

            tasksView.ActiveViewIndex = activeIndex;
            ViewState["TasksView.ActiveViewIndex"] = activeIndex;
            ResetError();
            // Databind charts
            DataBindChart();
        }
    }

    private string AppendEnvironmentName(string projectName, string environmentName)
    {
        string projectNameAppended = string.Empty;
        
        if ( projectName.IndexOf('$') == -1)
        {
            projectNameAppended = projectName + "$" + environmentName;
        }

        return projectNameAppended;
    }

    protected string FormatTaskName(object taskID, object taskName)
    {
        return "#" + taskID + " " + taskName;
    }

    protected void ddlOptions_OnSelectedIndexChanged(object source, EventArgs e)
    {
        FillTasks();
        ResetError();
    }

    private string ToggleDirection()
    {
        string newSortDirection = ViewState[SiteMap.CurrentNode.Description + "SortDirection"] as string;

        switch (newSortDirection)
        {
            case "DESC":
                newSortDirection = "ASC";
                break;

            case "ASC":
                newSortDirection = "DESC";
                break;
        }

        ViewState[SiteMap.CurrentNode.Description + "SortDirection"] = newSortDirection;
        return newSortDirection;
    }

    protected void taskChart_PostPaint(object sender, ChartPaintEventArgs e)
    {
        // If the series count is 0, this means that there is no data.
        if (taskChart.Series[0].Points.Count == 0)
        {
            // create text to draw
            if (textToDraw.Equals(string.Empty))
            {
                textToDraw = "No tasks found";
            }

            // get graphics tools
            System.Drawing.Graphics g = e.ChartGraphics.Graphics;
            System.Drawing.Font drawFont = System.Drawing.SystemFonts.DefaultFont;
            System.Drawing.Brush drawBrush = System.Drawing.Brushes.Gray;
            // see how big the text will be
            int txtWidth = (int)g.MeasureString(textToDraw, drawFont).Width;
            int txtHeight = (int)g.MeasureString(textToDraw, drawFont).Height;
            // where to draw
            int x = 120;  // a few pixels from the left border
            int y = (int)e.Chart.Height.Value;
            y = y - txtHeight - 100; // a few pixels off the bottom
            // draw the string        
            g.DrawString(textToDraw, drawFont, drawBrush, x, y);
        }
    }
    #endregion
    
    #region EnvironmentView GridView handlers

    protected void environmentView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");
        }

    }

    protected void environmentView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Add")
        {
            TextBox tNewEnvName = environmentView.FooterRow.FindControl("tNewEnvName") as TextBox;
            TextBox tNewEnvDesc = environmentView.FooterRow.FindControl("tNewEnvDesc") as TextBox;
            TextBox tNewMachineName = environmentView.FooterRow.FindControl("tNewMachineName") as TextBox;
            TextBox tNewDatabaseName = environmentView.FooterRow.FindControl("tNewDatabaseName") as TextBox;

            if (tNewEnvName != null && tNewEnvDesc != null)
            {
                BCCTaskEnvironment env = new BCCTaskEnvironment();

                env.EnvName = tNewEnvName.Text;
                env.EnvDesc = tNewEnvDesc.Text;
                env.MachineName = tNewMachineName.Text.Trim();
                env.DatabaseName = tNewDatabaseName.Text.Trim();

                BCCTaskEnvironment.AddTaskEnvironment(env);
            }

            FillEnvironments();
        }
        else if (e.CommandName == "AddOnEmpty")
        {
            TextBox tNewEnvName = environmentView.Controls[0].Controls[0].FindControl("tNewEnvName") as TextBox;
            TextBox tNewEnvDesc = environmentView.Controls[0].Controls[0].FindControl("tNewEnvDesc") as TextBox;
            TextBox tNewMachineName = environmentView.Controls[0].Controls[0].FindControl("tNewMachineName") as TextBox;
            TextBox tNewDatabaseName = environmentView.Controls[0].Controls[0].FindControl("tNewDatabaseName") as TextBox;

            if (tNewEnvName != null && tNewEnvDesc != null)
            {
                BCCTaskEnvironment env = new BCCTaskEnvironment();

                env.EnvName = tNewEnvName.Text.Trim();
                env.EnvDesc = tNewEnvDesc.Text.Trim();
                env.MachineName = tNewMachineName.Text.Trim();
                env.DatabaseName = tNewDatabaseName.Text.Trim();

                BCCTaskEnvironment.AddTaskEnvironment(env);
            }

            FillEnvironments();
        }
    }

    protected void environmentView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        environmentView.PageIndex = e.NewPageIndex;
        FillEnvironments();
    }

    protected void environmentView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        environmentView.EditIndex = e.NewEditIndex;
        FillEnvironments();
    }

    protected void environmentView_RowCancelEditing(object sender, GridViewCancelEditEventArgs e)
    {
        environmentView.EditIndex = -1;
        FillEnvironments();
    }

    protected void environmentView_RowUDeleting(object sender, GridViewDeleteEventArgs e)
    {
        GridViewRow deletedRow = environmentView.Rows[e.RowIndex] as GridViewRow;

        if (deletedRow != null)
        {
            Label lblEnvName = deletedRow.FindControl("lblEnvName") as Label;
            Label lblEnvDesc = deletedRow.FindControl("lblEnvDesc") as Label;
            HiddenField hGuid = deletedRow.FindControl("envID") as HiddenField;

            BCCTaskEnvironment env = new BCCTaskEnvironment();

            Guid envGUID = new Guid(hGuid.Value);

            env.EnvID = envGUID;
            env.EnvName = lblEnvName.Text;
            env.EnvDesc = lblEnvDesc.Text;
            env.EnvVisible = 0; // Inactive

            BCCTaskEnvironment.EnvironmentActivation(env);
        }

        // Refresh the data
        environmentView.EditIndex = -1;
        FillEnvironments();
        FillArchivedEnvironments();
    }

    protected void environmentView_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
       GridViewRow updatableRow = environmentView.Rows[e.RowIndex] as GridViewRow;

       if (updatableRow != null)
       {
           TextBox tEnvName = updatableRow.FindControl("tEnvName") as TextBox;
           TextBox tEnvDesc = updatableRow.FindControl("tEnvDesc") as TextBox;
           TextBox tMachineName = updatableRow.FindControl("tMachineName") as TextBox;
           TextBox tDatabaseName = updatableRow.FindControl("tDatabaseName") as TextBox;
           HiddenField hGuid = updatableRow.FindControl("envID") as HiddenField;

           BCCTaskEnvironment env = new BCCTaskEnvironment();

           Guid envGUID = new Guid(hGuid.Value);

           env.EnvID = envGUID;
           env.EnvName = tEnvName.Text;
           env.EnvDesc = tEnvDesc.Text;
           env.MachineName = tMachineName.Text;
           env.DatabaseName = tDatabaseName.Text;

           BCCTaskEnvironment.UpdateTaskEnvironment(env);
       }


       // Refresh the data
       environmentView.EditIndex = -1;
       FillEnvironments();
    }
    #endregion

    #region Archived-EnvironmentView GridView handlers

    protected void archivedEnvironments_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");
        }
    }

    protected void archivedEnvironments_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Activate")
        {
            GridViewRow clickedRow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);

            HiddenField hGuid = clickedRow.FindControl("envID") as HiddenField;

            if (hGuid != null)
            {
                BCCTaskEnvironment env = new BCCTaskEnvironment();
                Guid envGuid = new Guid(hGuid.Value);

                env.EnvID = envGuid;
                env.EnvVisible = 1;
                BCCTaskEnvironment.EnvironmentActivation(env);

                FillEnvironments();
                FillArchivedEnvironments();
            }
        }
    }

    protected void archivedEnvironments_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        archivedEnvironments.PageIndex = e.NewPageIndex;
        FillArchivedEnvironments();
    }

    #endregion

    #region ProjectView GridView handlers
    protected void projectView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");

           Label lblProjectPriority = e.Row.Cells[0].FindControl("lblProjectPriority") as Label;
           Label lblProjectName = e.Row.Cells[1].FindControl("lblProjectName") as Label;
           LinkButton lnkReport = e.Row.Cells[3].FindControl("lnkReport") as LinkButton;            

           if (lblProjectPriority != null)
           {
               lblProjectPriority.ForeColor = Color.White;

               if (lblProjectPriority.Text.Equals("1"))
               {
                   lblProjectPriority.BackColor = Color.Orange;
                   lblProjectPriority.ToolTip = "High Priority";
               }
               else if (lblProjectPriority.Text.Equals("2"))
               {
                   lblProjectPriority.BackColor = Color.RoyalBlue;
                   lblProjectPriority.ToolTip = "Medium Priority";
               }
               else if (lblProjectPriority.Text.Equals("3"))
               {
                   lblProjectPriority.BackColor = Color.Teal;
                   lblProjectPriority.ToolTip = "Low Priority";
               }
           }

           if (lblProjectName != null)
           {
               lnkReport.Attributes.Add("onclick", "window.open('KCTL-P.aspx?projectName=" + Server.UrlEncode(lblProjectName.Text) + "','','height=450,width=1000');return false;");
           }

        }
    }

    protected void projectView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Add")
        {
            TextBox tNewProjectName = projectView.FooterRow.FindControl("tNewProjectName") as TextBox;
            DropDownList ddlNewPriority = projectView.FooterRow.FindControl("ddlNewPriority") as DropDownList;
            DropDownList ddlNewEnvName = projectView.FooterRow.FindControl("ddlNewEnvName") as DropDownList;

            if (tNewProjectName != null && ddlNewPriority != null && ddlNewEnvName != null)
            {
                BCCTaskProject project = new BCCTaskProject();
                project.ProjectEnvironment = ddlNewEnvName.SelectedValue;
                project.ProjectName = tNewProjectName.Text;
               
                int priority = 1;
                    Int32.TryParse(ddlNewPriority.SelectedValue, out priority);
                project.ProjectPriority = priority;

                BCCTaskProject.AddTaskProject(project);
            }

            FillProjects();
        }
        else if (e.CommandName == "AddOnEmpty")
        {
            TextBox tNewProjectName = projectView.Controls[0].Controls[0].FindControl("tNewProjectName") as TextBox;
            DropDownList ddlNewPriority = projectView.Controls[0].Controls[0].FindControl("ddlNewPriority") as DropDownList;
            DropDownList ddlNewEnvName = projectView.Controls[0].Controls[0].FindControl("ddlNewEnvName") as DropDownList;

            if (tNewProjectName != null && ddlNewPriority != null && ddlNewEnvName != null)
            {
                BCCTaskProject project = new BCCTaskProject();
                project.ProjectEnvironment = ddlNewEnvName.SelectedValue;
                
                project.ProjectName = tNewProjectName.Text;
                
                int priority = 1;
                Int32.TryParse(ddlNewPriority.SelectedValue, out priority);
                project.ProjectPriority = priority;
                
                if (!(project.ProjectEnvironment.Equals(string.Empty)))
                {
                    BCCTaskProject.AddTaskProject(project);
                }
                else
                {
                    DisplayError("You must create an Environment before you can create a Project.");
                }
            }

            FillProjects();
        }
    }

    protected void projectView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        projectView.PageIndex = e.NewPageIndex;
        FillProjects();
    }

    protected void projectView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        projectView.EditIndex = e.NewEditIndex;
        FillProjects();
    }

    protected void projectView_RowCancelEditing(object sender, GridViewCancelEditEventArgs e)
    {
        projectView.EditIndex = -1;
        FillProjects();
    }

    protected void projectView_RowUDeleting(object sender, GridViewDeleteEventArgs e)
    {
        GridViewRow deletedRow = projectView.Rows[e.RowIndex] as GridViewRow;

        if (deletedRow != null)
        {
            Label lblProjectName = deletedRow.FindControl("lblProjectName") as Label;
            Label lblEnvName = deletedRow.FindControl("lblEnvName") as Label;
            Label lblProjectPriority = deletedRow.FindControl("lblProjectPriority") as Label;
            HiddenField hGuid = deletedRow.FindControl("projectID") as HiddenField;

            Guid projectGuid = new Guid(hGuid.Value);

            BCCTaskProject project = new BCCTaskProject();
            project.ProjectID = projectGuid;
            project.ProjectName = lblProjectName.Text;
            int priority = 1;
            Int32.TryParse(lblProjectPriority.Text, out priority);
            project.ProjectPriority = priority;
            project.ProjectEnvironment = lblEnvName.Text;
            project.ProjectVisible = 0;

            BCCTaskProject.UpdateTaskProject(project);
            BCCTaskProject.ProjectActivation(project);
        }

        // Refresh the data
        projectView.EditIndex = -1;
        FillProjects();
        FillArchivedProjects();
        FillTasks();
        FillArchivedTasks();
    }

    protected void projectView_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridViewRow updatableRow = projectView.Rows[e.RowIndex] as GridViewRow;

        if (updatableRow != null)
        {
            TextBox tProjectName = updatableRow.FindControl("tProjectName") as TextBox;
            DropDownList ddlPriority = updatableRow.FindControl("ddlPriority") as DropDownList;
            DropDownList ddlEnvName = updatableRow.FindControl("ddlEnvName") as DropDownList;
            HiddenField hPrjGuid = updatableRow.FindControl("projectID") as HiddenField;

            if (tProjectName != null && ddlPriority != null && ddlEnvName != null && hPrjGuid != null)
            {
                BCCTaskProject project = new BCCTaskProject();

                Guid projectGuid = new Guid(hPrjGuid.Value);

                project.ProjectID = projectGuid;
                project.ProjectName = tProjectName.Text;
                int priority = 1;
                Int32.TryParse(ddlPriority.SelectedValue, out priority);
                project.ProjectPriority = priority;
                project.ProjectEnvironment = ddlEnvName.SelectedValue;

                BCCTaskProject.UpdateTaskProject(project);
            }
        }

        // Refresh the data
        projectView.EditIndex = -1;
        FillProjects();
    }
    #endregion

    #region Archived-ProjectView GridView handlers

    protected void archivedProjects_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");
        }
    }

    protected void archivedProjects_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Activate")
        {
            GridViewRow clickedRow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);

            HiddenField hGuid = clickedRow.FindControl("projectID") as HiddenField;

            if (hGuid != null)
            {
                BCCTaskProject project = new BCCTaskProject();
                Guid projectGuid = new Guid(hGuid.Value);

                project.ProjectID = projectGuid;
                project.ProjectVisible = 1;
                BCCTaskProject.ProjectActivation(project);

                FillProjects();
                FillArchivedProjects();
            }
        }
    }

    protected void archivedProjects_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        archivedProjects.PageIndex = e.NewPageIndex;
        FillArchivedProjects();
    }

    #endregion

    #region TaskView GridView handlers
    protected void taskView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");

            Label lblTaskPriority = e.Row.Cells[0].FindControl("lblTaskPriority") as Label;

            HiddenField htaskID = e.Row.Cells[1].FindControl("taskID") as HiddenField;
            Label lblTaskTitle = e.Row.Cells[1].FindControl("lblTaskTitle") as Label;

            Label lblProjectName = e.Row.Cells[2].FindControl("lblProjectName") as Label; 
            Label lblTaskType = e.Row.Cells[3].FindControl("lblTaskType") as Label;
            Label lblTaskDueDate = e.Row.Cells[4].FindControl("lblTaskDueDate") as Label;
            Label lblTaskStatus = e.Row.Cells[5].FindControl("lblTaskStatus") as Label;
            Label lblAssignedToUser = e.Row.Cells[6].FindControl("lblAssignedToUser") as Label;

            LinkButton lnkEffort = e.Row.Cells[8].FindControl("lnkEffort") as LinkButton;

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

            if (lblTaskTitle != null && lblAssignedToUser.Text == string.Empty)
            {
                lblTaskTitle.Font.Italic = true;
                lblTaskTitle.ForeColor = Color.Gray;
                lblTaskTitle.ToolTip = "This task has not been assigned";
            }

            if (lblTaskStatus != null && 
                    ( lblTaskStatus.Text == Enum.GetName(typeof(BCCTask.TaskState), BCCTask.TaskState.Created)
                    || lblTaskStatus.Text == Enum.GetName(typeof(BCCTask.TaskState), BCCTask.TaskState.Pending)
                    || lblTaskStatus.Text == Enum.GetName(typeof(BCCTask.TaskState), BCCTask.TaskState.Deferred)
                    ))
            {
                lblTaskStatus.BackColor = Color.Khaki;
            }

            if (lblTaskType != null)
            {
                lblTaskType.BackColor = Color.PaleGoldenrod;

                if (!(lblTaskType.Text.Equals(BCCTask.TaskTypeState.Once.ToString())))
                {
                    lblTaskType.BorderStyle = BorderStyle.Solid;
                    lblTaskType.BorderWidth = 1;
                    lblTaskType.ForeColor = Color.Gray;
                }
            }

            if (lblAssignedToUser != null && lblAssignedToUser.Text == string.Empty)
            {
                lblAssignedToUser.BorderStyle = BorderStyle.Solid;
                lblAssignedToUser.BorderWidth = 1;
                lblAssignedToUser.BorderColor = Color.Black;
                lblAssignedToUser.Text = BCCUIHelper.Constants.NOT_ASSIGNED;
                lblAssignedToUser.ForeColor = Color.Black;
                lblAssignedToUser.BackColor = Color.NavajoWhite;
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

            if (htaskID != null && lblTaskTitle != null && lblProjectName != null)
            {
                lnkEffort.Attributes.Add("onclick", "window.open('KCTL-E.aspx?taskID=" + htaskID.Value + "&taskName=" + Server.UrlEncode(lblTaskTitle.Text) + "&projectName=" + Server.UrlEncode(lblProjectName.Text) + "','','height=450,width=1000');return false;");
            }
        }
    }

    protected void taskView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Add"))
        {
            TextBox tNewProjectName = taskView.FooterRow.FindControl("tNewTaskTitle") as TextBox;
            DropDownList ddlNewProjectName = taskView.FooterRow.FindControl("ddlNewProjectName") as DropDownList;
            DropDownList ddlNewTaskType = taskView.FooterRow.FindControl("ddlNewTaskType") as DropDownList;

            if (tNewProjectName != null && ddlNewProjectName != null && ddlNewTaskType != null)
            {
                BCCTask task = new BCCTask();

                task.TaskID = System.Guid.NewGuid();
                task.TaskTitle = tNewProjectName.Text;
                task.TaskProjectName = ddlNewProjectName.SelectedValue;
                task.TaskCreatedDate = DateTime.Now;
                // 7 days from today!
                task.TaskDueDate = DateTime.Today.AddDays(7);
                task.TaskDescription = string.Empty;
                task.TaskType = ddlNewTaskType.SelectedValue;
                task.CreatedbyUser = HttpContext.Current.User.Identity.Name;
                task.AssignedToUser = HttpContext.Current.User.Identity.Name;

                BCCTask.CreateTask(task);
                DisplayInfo("New task created successfully");
            }

            FillTasks();
        }
        else if (e.CommandName.Equals("AddOnEmpty"))
        {
            TextBox tNewTaskName = taskView.Controls[0].Controls[0].FindControl("tNewTaskName") as TextBox;
            DropDownList ddlPriority = taskView.Controls[0].Controls[0].FindControl("ddlPriority") as DropDownList;
            DropDownList ddlNewProjectName = taskView.Controls[0].Controls[0].FindControl("ddlNewProjectName") as DropDownList;
            DropDownList ddlNewTaskType = taskView.Controls[0].Controls[0].FindControl("ddlNewTaskType") as DropDownList;

            if (tNewTaskName != null && ddlPriority != null && ddlNewProjectName != null && ddlNewTaskType != null)
            {
                BCCTask task = new BCCTask();

                task.TaskID = System.Guid.NewGuid();
                task.TaskTitle = tNewTaskName.Text;
                task.TaskProjectName = ddlNewProjectName.SelectedValue;
                task.TaskCreatedDate = DateTime.Now;
                // 7 days from today!
                task.TaskDueDate = DateTime.Today.AddDays(7);
                task.TaskDescription = string.Empty;
                task.TaskType = ddlNewTaskType.SelectedValue;
                task.CreatedbyUser = HttpContext.Current.User.Identity.Name;
                task.AssignedToUser = HttpContext.Current.User.Identity.Name;

                if (!(task.TaskProjectName.Equals(string.Empty)))
                {
                    BCCTask.CreateTask(task);
                    DisplayInfo("New task created successfully");
                }
                else
                {
                    DisplayError("You must create a Project before you can create a task");
                }
            }

            FillTasks();
        }
        else if (e.CommandName.Equals("TaskComplete"))
        {
            GridViewRow clickedRow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);

            HiddenField hGuid = clickedRow.FindControl("taskID") as HiddenField;
            HiddenField taskRef = clickedRow.FindControl("taskRef") as HiddenField;
            Label lblAssignedToUser = clickedRow.FindControl("lblAssignedToUser") as Label;

            if (hGuid != null && taskRef != null && lblAssignedToUser != null)
            {
                Guid taskGuid = new Guid(hGuid.Value);

                BCCTask task = new BCCTask();

                task.TaskID = taskGuid;
                task.TaskRef = taskRef.Value;
                task.TaskStatus = Enum.GetName(typeof(BCCTask.TaskState), BCCTask.TaskState.Completed);
                task.UpdatedbyUser = HttpContext.Current.User.Identity.Name;

                if (lblAssignedToUser.Text.Equals(BCCUIHelper.Constants.NOT_ASSIGNED))
                {
                    task.AssignedToUser = string.Empty;
                }
                else
                {
                    task.AssignedToUser = lblAssignedToUser.Text;
                }

                BCCTaskSystemMessage message = BCCTask.VerifyTask(task);

                if (message != null)
                {
                    DisplayError(message.ErrorMessage);
                }
                else
                {
                    BCCTask.UpdateTaskStatus(task);
                }
            }

            FillTasks();
        }
    }

    protected void taskView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        taskView.PageIndex = e.NewPageIndex;
        FillTasks();
        DataBindChart();
    }

    protected void taskView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        taskView.EditIndex = e.NewEditIndex;
        FillTasks();
        ResetError();
        DataBindChart();
    }

    protected void taskView_RowCancelEditing(object sender, GridViewCancelEditEventArgs e)
    {
        taskView.EditIndex = -1;
        FillTasks();
    }

    protected void taskView_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        GridViewRow deletedRow = taskView.Rows[e.RowIndex] as GridViewRow;

        if (deletedRow != null)
        {
            HiddenField hGuid = deletedRow.FindControl("taskID") as HiddenField;
            Guid taskGuid = new Guid(hGuid.Value);
            BCCTask task = new BCCTask();
            task.TaskID = taskGuid;
            task.UpdatedbyUser = HttpContext.Current.User.Identity.Name; 
            // Just do a soft delete
            BCCTask.DeactivateTask(task, false);
        }

        // Refresh the data
        taskView.EditIndex = -1;
        FillTasks();
        FillArchivedTasks();
    }

    protected void taskView_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridViewRow updatableRow = taskView.Rows[e.RowIndex] as GridViewRow;

        if (updatableRow != null)
        {
            HiddenField htaskID = updatableRow.FindControl("taskID") as HiddenField;
            HiddenField taskRef = updatableRow.FindControl("taskRef") as HiddenField;

            DropDownList ddlPriority = updatableRow.FindControl("ddlPriority") as DropDownList;
            DropDownList ddlProjectName = updatableRow.FindControl("ddlProjectName") as DropDownList;
            DropDownList ddlTaskType = updatableRow.FindControl("ddlTaskType") as DropDownList;
            DropDownList ddlUsers = updatableRow.FindControl("ddlUsers") as DropDownList;

            TextBox tTaskDueDate = updatableRow.FindControl("tTaskDueDate") as TextBox;
            TextBox tTaskTitle = updatableRow.FindControl("tTaskTitle") as TextBox;

            if (htaskID != null)
            {
                BCCTask task = new BCCTask();

                Guid taskGuid = new Guid(htaskID.Value);
                task.TaskID = taskGuid;
                task.TaskRef = taskRef.Value;
                task.TaskTitle = tTaskTitle.Text;
                task.AssignedToUser = ddlUsers.SelectedValue;
               
                int priority = 1;
                Int32.TryParse(ddlPriority.SelectedValue, out priority);
                task.TaskPriority = priority;

                task.TaskDueDate = DateTime.Parse(tTaskDueDate.Text);
                task.TaskType = ddlTaskType.SelectedValue;
                task.TaskProjectName = ddlProjectName.SelectedValue;
                task.UpdatedbyUser = HttpContext.Current.User.Identity.Name;

                BCCTaskSystemMessage message = BCCTask.VerifyTask(task);
                     
                if (message != null)
                {
                    DisplayError(message.ErrorMessage);
                }
                else
                {
                    BCCTask.UpdateTask(task);
                    DisplayInfo( string.Format("Task #{0} was updated", taskRef.Value));
                }
            }
        }

        // Refresh the data
        taskView.EditIndex = -1;
        FillTasks();
        DataBindChart();
    }

    protected void taskView_OnSort(object sender, GridViewSortEventArgs e)
    {
        DataTable dataTable = FillTasks();

        if (dataTable != null)
        {
            DataView dataView = new DataView(dataTable);
            dataView.Sort = e.SortExpression + " " + ToggleDirection();
            taskView.DataSource = dataView;
            taskView.DataBind();
        }
    }
    #endregion

    #region Archived-TaskView GridView handlers
    protected void archivedTasks_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");
        }
    }

    protected void archivedTasks_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Archive")
        {
            GridViewRow clickedRow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);

            HiddenField hTaskID = clickedRow.FindControl("hTaskID") as HiddenField;

            if (hTaskID != null)
            {
                Guid taskGuid = new Guid(hTaskID.Value);

                //Clone the task
                BCCTask task = new BCCTask();
                task.TaskID = taskGuid;

                // Hard delete
                BCCTask.DeactivateTask(task, true);
            }

            FillArchivedTasks();
        }
    }

    protected void archivedTasks_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        archivedTasks.PageIndex = e.NewPageIndex;
        FillArchivedTasks();
    }

    #endregion

    #region Task Report GridView handlers

    protected void taskReport_OnSort(object sender, GridViewSortEventArgs e)
    {
        BCCTaskEffort effort = new BCCTaskEffort();

        effort.TaskAssignedToUserName = CurrentUser;
        effort.TaskStatus = null;
        DateTime startDate = DateTime.Today.Subtract(new TimeSpan(7, 0, 0, 0));
        effort.ReportStartDate = startDate;

        if (DateTime.TryParse(tStartDate.Text, out startDate))
        {
            effort.ReportStartDate = startDate;
        }

        DateTime endDate = DateTime.Today;
        effort.ReportEndDate = endDate;

        if (DateTime.TryParse(tEndDate.Text, out endDate))
        {
            effort.ReportEndDate = endDate;
        }

        DataTable dtTasks = BCCTaskEffort.ReportTaskEfforts(effort);

        if (dtTasks != null)
        {
            DataView dataView = new DataView(dtTasks);
            dataView.Sort = e.SortExpression + " " + ToggleDirection();
            reportView.DataSource = dataView;
            reportView.DataBind();
            reportView.Visible = true;
        }
    }

    protected void taskReport_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        reportView.PageIndex = e.NewPageIndex;
        FillReport(DefaultReport);
    }

    protected void taskReport_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblTaskPriority = e.Row.Cells[0].FindControl("lblTaskPriority") as Label;
            Label lblTaskTitle = e.Row.Cells[1].FindControl("lblTaskTitle") as Label;
            Label lblTaskDueDate = e.Row.Cells[2].FindControl("lblTaskDueDate") as Label;
            Label lblTaskStatus = e.Row.Cells[3].FindControl("lblTaskStatus") as Label;
            HiddenField hTaskHours = e.Row.Cells[4].FindControl("hTaskHours") as HiddenField;

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
        BCCTaskEffort effort = new BCCTaskEffort();

        effort.TaskAssignedToUserName = CurrentUser;
        effort.TaskStatus = null;
        DateTime startDate = DateTime.Today.Subtract(new TimeSpan(5, 0, 0, 0));
        effort.ReportStartDate = startDate;

        if (DateTime.TryParse(tStartDate.Text, out startDate))
        {
            effort.ReportStartDate = startDate;
        }

        DateTime endDate = DateTime.Today;
        effort.ReportEndDate = endDate;

        if (DateTime.TryParse(tEndDate.Text, out endDate))
        {
            effort.ReportEndDate = endDate;
        }
        
        FillReport(effort);
        // Databind charts
        DataBindChart();
    }

    private void InitializeReports()
    {
        // Start date is 7 days behind UTC today
        tStartDate.Text = System.DateTime.UtcNow.Subtract(new TimeSpan(7, 0, 0, 0)).ToString("MMM-dd-yyyy");
        tEndDate.Text = TodaysDate;
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        try
        {
            HtmlForm myMasterForm = Master.FindControl("form1") as HtmlForm;

            if (myMasterForm != null)
            {
                myMasterForm.Controls.Add(reportView);
            }

            //BCCGridView.Export("ControlCenter_Tasks_" + CurrentUser + "_" + DateTime.Today.ToShortDateString() + ".xls", reportView);
        }
        catch
        {

        }
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
    }

    #endregion
}
