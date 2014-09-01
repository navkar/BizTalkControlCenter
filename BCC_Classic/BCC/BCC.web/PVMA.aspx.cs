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

public partial class MessageAudit : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCOperator bccOperator = new BCCOperator();
    BCCUIHelper uiHelper = new BCCUIHelper();
    public int count = 0;
    private int startRow = 1;
    private int endRow = 250;
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

        if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN) || User.IsInRole(BCCUIHelper.Constants.ROLE_ARTIFACT))
        {
            if (!Page.IsPostBack)
            {
                PopulateGrid(string.Empty);
                ActivateGrid();
            }
        }
        else
        {
            DisplayError(BCCUIHelper.Constants.ACCESS_DENIED);
        }
    }

    private void ActivateGrid()
    {
        UserActivityGrid.Visible = true;
        Session[SiteMap.CurrentNode.Description + "SortDirection"] = sortDirection;
    }

    private void InitializeObjects()
    {
        gridBCCAudit.Visible = false;
        lblError.Visible = false;
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";
        this.search.SearchClick += new EventHandler(this.btnFilter_Click);
        UpdateLabel();
    }

    protected void gridBCCAudit_OnSort(object sender, GridViewSortEventArgs e)
    {
        DataTable dataTable = BCCAuditDataAccess.RetrieveAuditRecords(startRow, endRow);

        if (dataTable != null)
        {
            DataView dataView = new DataView(dataTable);
            dataView.Sort = e.SortExpression + " " + ToggleDirection();
            gridBCCAudit.DataSource = dataView;
            gridBCCAudit.DataBind();
        }
    }

    protected void gridBCCAudit_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        gridBCCAudit.PageIndex = e.NewPageIndex;

        // The index starts from 0.
        if (e.NewPageIndex != -1)
        {
            //if (e.NewPageIndex == 0)
            //{
            //    startRow = 1;
            //    endRow = rows_per_page;       // no of rows per page.
            //}
            //else
            //{
            //    startRow = e.NewPageIndex * rows_per_page;
            //    endRow = startRow + rows_per_page;
            //}

            DataTable dataTable = BCCAuditDataAccess.RetrieveAuditRecords(startRow, endRow);

            if (dataTable != null)
            {
                gridBCCAudit.DataSource = dataTable;
                gridBCCAudit.DataBind();
            }

        }

        e.Cancel = false;
    }

    private string ToggleDirection()
    {
        string newSortDirection = Session[SiteMap.CurrentNode.Description + "SortDirection"] as string;

        switch (newSortDirection)
        {
            case "DESC":
                newSortDirection = "ASC";
                break;

            case "ASC":
                newSortDirection = "DESC";
                break;
        }

        Session[SiteMap.CurrentNode.Description + "SortDirection"] = newSortDirection;
        return newSortDirection;
    }

    private void UpdateLabel()
    {
        search.KeywordLabel = "(keywords: 'session-id guid', 'Success', 'Request', ...) [" + count + "]";
    }

    private void PopulateGrid(string searchKeyword)
    {
        try
        {
            DataTable dt = BCCAuditDataAccess.RetrieveAuditRecords(startRow, endRow);

            if (dt != null)
            {
                if (searchKeyword != null && searchKeyword.Length > 0)
                {
                    dt.DefaultView.RowFilter = "AuditMessage LIKE '%" + searchKeyword + "%' OR WorkflowStatus LIKE '%" + searchKeyword + "%' OR SessionID = '" + searchKeyword + "'";
                    new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "searched for '" + searchKeyword + "'", 304);
                }
            }
            else
            {
                if (searchKeyword != null && searchKeyword.Length > 0)
                {
                    DisplayError("There were no record(s) found, to start a search.");
                }
            }

            gridBCCAudit.DataSource = dt;
            gridBCCAudit.DataBind();
            gridBCCAudit.Visible = true;
            count = gridBCCAudit.Rows.Count;
            UpdateLabel();
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            System.Diagnostics.Debug.Write(ex.Message + ex.StackTrace, SiteMap.CurrentNode.Description);
            gridBCCAudit.Visible = false;
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

    protected void gridBCCAudit_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // -0-
            Label lblSessionID = e.Row.Cells[0].FindControl("lblSessionID") as Label;

            if (lblSessionID != null)
            {
                //lblSessionID.ToolTip = lblSessionID.Text;
                //lblSessionID.Text = lblSessionID.Text.Substring(0, 5) + "...";
                //lblSessionID.ForeColor = Color.White;
                //lblSessionID.BackColor = Color.Gray;
            }

            // -1-
            LinkButton lblMessageID = e.Row.Cells[1].FindControl("lblMessageID") as LinkButton;

            if (lblMessageID != null)
            {
                lblMessageID.ToolTip = lblMessageID.Text;
                lblMessageID.Text = lblMessageID.Text.Substring(0, 6) + "...";
                lblMessageID.ForeColor = Color.White;
                lblMessageID.BackColor = Color.Gray;
            }

            Label lblParentService = e.Row.Cells[2].FindControl("lblParentService") as Label;

            if (lblParentService != null)
            {
                //lblParentService.ToolTip = lblParentService.Text;
                //lblParentService.Text = lblParentService.Text.Substring(0, 4) + "...";
                //lblParentService.ForeColor = Color.White;
                //lblParentService.BackColor = Color.Goldenrod;
            }

            Label lblChildService = e.Row.Cells[3].FindControl("lblChildService") as Label;

            if (lblChildService != null)
            {
                //lblChildService.ToolTip = lblChildService.Text;
                //lblChildService.Text = lblChildService.Text.Substring(0, 4) + "...";
                //lblChildService.ForeColor = Color.White;
                //lblChildService.BackColor = Color.BurlyWood;
            }

            Label lblInitiator = e.Row.Cells[4].FindControl("lblInitiator") as Label;

            //if (lblInitiator != null)
            //{
            //    lblInitiator.ForeColor = Color.White;
            //    lblInitiator.BackColor = Color.Gray;
            //}

            Label lblSource = e.Row.Cells[5].FindControl("lblSource") as Label;

            //if (lblSource != null)
            //{
            //    lblSource.ForeColor = Color.White;
            //    lblSource.BackColor = Color.Gray;
            //}

            // -06-
            Label lblTarget = e.Row.Cells[6].FindControl("lblTarget") as Label;

            //if (lblTarget != null)
            //{
            //    lblTarget.ForeColor = Color.White;
            //    lblTarget.BackColor = Color.Peru;
            //}

            // -07-
            Label lblProcessStep = e.Row.Cells[7].FindControl("lblProcessStep") as Label;

            //if (lblProcessStep != null)
            //{
            //    lblProcessStep.ForeColor = Color.White;
            //    lblProcessStep.BackColor = Color.Teal;
            //}

            // -08-
            Label lblEventType = e.Row.Cells[8].FindControl("lblEventType") as Label;

            if (lblEventType != null)
            {
                if (lblEventType.Text.Contains("REQUEST"))
                {
                    lblEventType.ForeColor = Color.White;
                    lblEventType.BackColor = Color.RosyBrown;
                }
                else
                {
                    lblEventType.ForeColor = Color.White;
                    lblEventType.BackColor = Color.Teal;
                }
            }

            // -09-
            Label lblWorkflowStatus = e.Row.Cells[9].FindControl("lblWorkflowStatus") as Label;

            if (lblWorkflowStatus != null)
            {
                if (lblWorkflowStatus.Text.Equals("SUCCESS"))
                {
                    lblWorkflowStatus.ForeColor = Color.White;
                    lblWorkflowStatus.BackColor = Color.Teal;
                    lblWorkflowStatus.ToolTip = "This transaction has succeeded.";
                }
                else
                {
                    lblWorkflowStatus.ForeColor = Color.White;
                    lblWorkflowStatus.BackColor = Color.RosyBrown;
                }
            }

            // -10-
            Label lblCreatedOn = e.Row.Cells[10].FindControl("lblCreatedOn") as Label;

            if (lblCreatedOn != null)
            {
                string dateTimeVal2 = lblCreatedOn.Text;
                lblCreatedOn.Text = DateTime.Parse(dateTimeVal2).ToLongTimeString();
                lblCreatedOn.ForeColor = Color.White;
                lblCreatedOn.BackColor = Color.DarkSalmon;
                lblCreatedOn.ToolTip = dateTimeVal2;
            }

            // -11-
            Label lblHostName = e.Row.Cells[11].FindControl("lblHostName") as Label;

            if (lblHostName != null)
            {
                lblHostName.ToolTip = lblHostName.Text;
                lblHostName.Text = lblHostName.Text.Substring(0, 4) + "...";
                lblHostName.ForeColor = Color.DarkGray;
                lblHostName.BackColor = Color.Bisque;
            }

            //Label lblJobStatus = e.Row.Cells[3].FindControl("lblJobStatus") as Label;

            //if (lblJobStatus != null)
            //{
            //    if (lblJobStatus.Text.Contains("Fail"))
            //    {
            //        lblJobStatus.ForeColor = Color.White;
            //        lblJobStatus.BackColor = Color.RosyBrown;
            //        lblJobStatus.ToolTip = "This job has failed, check with your administrator.";
            //    }
            //    else if (lblJobStatus.Text.Contains("Executing"))
            //    {
            //        lblJobStatus.ForeColor = Color.White;
            //        lblJobStatus.BackColor = Color.Teal;
            //        lblJobStatus.ToolTip = "This job is executing, wait for it to complete.";
            //    }
            //    else if (lblJobStatus.Text.Equals("Idle"))
            //    {
            //        lblJobStatus.ForeColor = Color.White;
            //        lblJobStatus.BackColor = Color.Gray;
            //        lblJobStatus.ToolTip = "This job is idle, check the next run time.";
            //    }
            //    else
            //    {
            //        lblJobStatus.ForeColor = Color.White;
            //        lblJobStatus.BackColor = Color.Gray;
            //        lblJobStatus.ToolTip = "This job is " + lblJobStatus.Text + ".";
            //    }
            //}

            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");
        }
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

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        HtmlForm myMasterForm = (HtmlForm)Master.FindControl("form1");

        if (myMasterForm != null)
        {
            myMasterForm.Controls.Add(gridBCCAudit);
        }

        BCCGridView.Export("ControlCenter_" + System.Environment.MachineName + "_BCCAudit.xls", this.gridBCCAudit);
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        // DO NOT : remove this method. 
    }
}
