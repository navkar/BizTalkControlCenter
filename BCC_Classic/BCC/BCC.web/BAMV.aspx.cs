using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BCC.Core;
using BCC.Core.WMI.BizTalk;
using System.Collections.Specialized;

public partial class BiztalkMessageView : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCOperator bccOperator = new BCCOperator();
    BCCUIHelper uiHelper = new BCCUIHelper();
    public DataTable dt, dt1;
    string lastDirection = "DESC";
    string sortExpression = "CreationTime";
    public string totalMessages = "0";

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

        if (!Page.IsPostBack)
        {
            Session["SortDirection"] = lastDirection;
            Session["SortExpression"] = sortExpression;
        }
        else
        {
            lastDirection = Session["SortDirection"].ToString();
            sortExpression = Session["SortExpression"].ToString();
        }

        if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN) || User.IsInRole(BCCUIHelper.Constants.ROLE_ARTIFACT))
        {
            PopulateGrid();
            ActivateGrid();
            PopulateMessageStatusCount();
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

    private void PopulateMessageStatusCount()
    {
        int defaultMaxLimit = 500;

        try
        {

            try
            {
                BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                StringCollection maxLimitValue = props.ModuleDictionary[BCCUIHelper.Constants.BT_MSG_DISPLAY_LIMIT];
                Int32.TryParse(maxLimitValue[0], out defaultMaxLimit);
            }
            catch
            {
                // Ignore errors from Profile system
                defaultMaxLimit = 500;
            }

            DataTable dt = dataAccess.RetrieveAllMessages(defaultMaxLimit);
            string filterExpr = string.Empty;
            string activeMsgCount = "0", suspendedMsgCount = "0", suspendedNRMsgCount = "0", dehydratedMsgCount = "0";

            DataTable dtMsg = new DataTable();

            dtMsg.Columns.Add(new DataColumn("MessageState", typeof(string)));
            dtMsg.Columns.Add(new DataColumn("Count", typeof(string)));

            DataRow dr = null;

            if (dt.Rows.Count > 0)
            {
                filterExpr = "Status = 'Active'";
                activeMsgCount = dt.Select(filterExpr).Length + "";

                filterExpr = "Status = 'Suspended'";
                suspendedMsgCount = dt.Select(filterExpr).Length + "";

                filterExpr = "Status = 'SuspendedNotResumable'";
                suspendedNRMsgCount = dt.Select(filterExpr).Length + "";

                filterExpr = "Status = 'Dehydrated'";
                dehydratedMsgCount = dt.Select(filterExpr).Length + "";
            }

            dr = dtMsg.NewRow();
            dr[0] = "Active";
            dr[1] = activeMsgCount;
            dtMsg.Rows.Add(dr);

            dr = dtMsg.NewRow();
            dr[0] = "Suspended";
            dr[1] = suspendedMsgCount;
            dtMsg.Rows.Add(dr);

            dr = dtMsg.NewRow();
            dr[0] = "Suspended non-resumable";
            dr[1] = suspendedNRMsgCount;
            dtMsg.Rows.Add(dr);

            dr = dtMsg.NewRow();
            dr[0] = "Dehydrated";
            dr[1] = dehydratedMsgCount;
            dtMsg.Rows.Add(dr);

            gridMsgInfo.DataSource = dtMsg;
            gridMsgInfo.DataBind();
            gridMsgInfo.Visible = true;
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            gridMsgInfo.Visible = false;
        }
    }

    private void InitializeObjects()
    {
        gridBTMessages.Visible = false;
        lblError.Visible = false;
        btnSelectAll.Enabled = false;
        btnDeselectAll.Enabled = false;
        btnResume.Enabled = false;
        btnTerminate.Enabled = false;
        btnSelectAll2.Enabled = false;
        btnDeselectAll2.Enabled = false;
        btnResume2.Enabled = false;
        btnTerminate2.Enabled = false;
        gridBTMessages.Visible = false;
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        lblSubCaption.Text = "Information";
    }

    private void EnableObjects()
    {
        gridBTMessages.Visible = true;
        btnSelectAll.Enabled = true;
        btnDeselectAll.Enabled = true;
        btnResume.Enabled = true;
        btnTerminate.Enabled = true;
        btnSelectAll2.Enabled = true;
        btnDeselectAll2.Enabled = true;
        btnResume2.Enabled = true;
        btnTerminate2.Enabled = true;
    }

    private void PopulateGrid()
    {
        try
        {
            StringCollection applicationList = null;
            int defaultMaxLimit = 500;

            // Check whether the Profile is active and then proceed.
            if (Profile.ControlCenterProfile.IsProfileActive)
            {
                try
                {
                    BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                    applicationList = props.ModuleDictionary[BCCUIHelper.Constants.APP_LIST_KEY];

                    StringCollection maxLimitValue = props.ModuleDictionary[BCCUIHelper.Constants.BT_MSG_DISPLAY_LIMIT];
                    Int32.TryParse(maxLimitValue[0], out defaultMaxLimit);
                }
                catch
                {
                    // Ignore errors from Profile system
                    defaultMaxLimit = 500;
                }
            }

            if (applicationList != null && applicationList.Count > 0)
            {
                dt = dataAccess.RetrieveAllMessages(string.Empty, applicationList, defaultMaxLimit);
                
                // This is for the datalist view on the right side. 
                dlBTAppList.DataSource = applicationList;
                dlBTAppList.DataBind();
                dlBTAppList.Visible = true;

                DisplayInfoOnSideBar("<b>" + defaultMaxLimit + "</b>");
            }
            else
            {
                dt = dataAccess.RetrieveAllMessages(defaultMaxLimit);
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                //// Enable buttons
                EnableObjects();
                totalMessages = dt.Rows.Count + "";

                gridBTMessages.DataSource = dt;
                gridBTMessages.DataBind();
            }
            else
            {
                gridBTMessages.DataSource = new DataTable();
                gridBTMessages.DataBind();
                DisplayError("0 message(s) found.");
            }
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message + "\n" + ex.StackTrace);
        }
    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = false;
        errorImg.Visible = true;
    }

    protected void DisplayInfoOnSideBar(string message)
    {
        lblMsgLimit.Visible = true;
        lblMsgLimit.Text = message;
        lblMsgLimit.Font.Bold = false;
    }

    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
        bccOperator.SetCheckAllCheckBox(gridBTMessages, "chkBoxMessage", true);
    }

    protected void btnDeSelectAll_Click(object sender, EventArgs e)
    {
        bccOperator.SetCheckAllCheckBox(gridBTMessages, "chkBoxMessage", false);
    }

    private void ActOnMessages(bool isResumeMessage)
    {
        string errorMsg = string.Empty;
        string instanceID = string.Empty;
        // Iterate through the Gridview Rows property
        foreach (GridViewRow row in gridBTMessages.Rows)
        {
            // Access the CheckBox
            CheckBox cb = (CheckBox)row.FindControl("chkBoxMessage");
            if (cb != null && cb.Checked)
            {
                instanceID = row.Cells[3].Text;

                try
                {
                    if (isResumeMessage)
                    {
                        dataAccess.ResumeMessage(instanceID);
                        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "resumed instance id:" + instanceID, 204);
                    }
                    else
                    {
                        dataAccess.TerminateMessage(instanceID);
                        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "terminated instance id:" + instanceID, 204);
                    }
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                }
            }
        }
    }

    protected void btnResume_Click(object sender, EventArgs e)
    {
        ActOnMessages(true);
        PopulateGrid();
    }

    protected void btnTerminate_Click(object sender, EventArgs e)
    {
        ActOnMessages(false);
        PopulateGrid();
    }

    protected void btnMonitor_Click(object sender, EventArgs e)
    {
        Monitor();
        PopulateGrid();
    }

    private void Monitor()
    {
        string machineName = System.Environment.MachineName;

        try
        {
            BCCMonitoringDataAccess da = new BCCMonitoringDataAccess();
            da.CreateMonitoringEntry(ArtifactType.ServiceInstance, machineName);
            new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, " setup monitoring for service instance on '" + machineName + "'", 204);
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.Write(e.Message + e.StackTrace);
        }
    }

    protected void gridBTMessages_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.RowIndex != -1)
            {
                if (BCCUIHelper.Constants.STATUS_ACTIVE.Equals(e.Row.Cells[2].Text))
                {
                    e.Row.Cells[2].ForeColor = Color.Green;
                }
                else
                {
                    if (e.Row.Cells[2].Text.StartsWith(BCCUIHelper.Constants.STATUS_SUSPENDED))
                    {
                        e.Row.Cells[2].ForeColor = Color.Red;
                    }
                    else
                    {
                        e.Row.Cells[2].ForeColor = Color.Blue;
                    }
                }
            }

            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");
        }
    }
    
    protected void gridBTMessages_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        gridBTMessages.PageIndex = e.NewPageIndex;
        dt.DefaultView.Sort = sortExpression + " " + lastDirection;
        gridBTMessages.DataBind();
    }

    protected void gridBTMessages_Sorting(object sender, GridViewSortEventArgs e)
    {
        String sortDirection = "ASC";
        sortExpression = e.SortExpression;

        // If the last state is Ascending sort Descending
        if ((lastDirection != null) && (lastDirection.Equals("ASC")))
        {
            sortDirection = "DESC";
        }
        else
        {
            sortDirection = "ASC";
        }

        Session["SortDirection"] = sortDirection;
        Session["SortExpression"] = sortExpression;
        dt.DefaultView.Sort = sortExpression + " " + sortDirection;
        gridBTMessages.DataBind();
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        HtmlForm myMasterForm = Master.FindControl("form1") as HtmlForm;

        if (myMasterForm != null)
        {
            myMasterForm.Controls.Add(gridBTMessages);
        }

        BCCGridView.Export("ControlCenter_" + System.Environment.MachineName + "_Messages.xls", this.gridBTMessages);
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
	// DO NOT : remove this method. 
    }
   
}
