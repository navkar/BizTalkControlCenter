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

public partial class BiztalkSendPort : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCOperator bccOperator = new BCCOperator();
    BCCUIHelper uiHelper = new BCCUIHelper();
    DataTable dt = null;
    string lastDirection = "ASC";
    string sortExpression = "SendPortName";

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

        if (Page.IsPostBack)
        {
            lastDirection = Session[SiteMap.CurrentNode.Description + "SortDirection"] as string;
            sortExpression = Session[SiteMap.CurrentNode.Description + "SortExpression"] as string;
        }
        else
        {
            Session[SiteMap.CurrentNode.Description + "SortDirection"] = lastDirection;
            Session[SiteMap.CurrentNode.Description + "SortExpression"] = sortExpression;
        }
    }

    protected void OnSort(object sender, GridViewSortEventArgs e)
    {
        string sortExpression2 = e.SortExpression;

        if ((lastDirection != null) && (lastDirection.Equals("ASC")))
        {
            lastDirection = "DESC";
        }
        else
        {
            lastDirection = "ASC";
        }

        Session[SiteMap.CurrentNode.Description + "SortDirection"] = lastDirection;
        Session[SiteMap.CurrentNode.Description + "SortExpression"] = sortExpression2;
        dt.DefaultView.Sort = sortExpression2 + " " + lastDirection;
        gridSendPort.DataBind();
    }

    protected void gridSendPort_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        gridSendPort.PageIndex = e.NewPageIndex;
        gridSendPort.DataBind();
    }

    private void ActivateGrid()
    {
        UserActivityGrid.Visible = true;
    }

    private void UpdateLabel(int count)
    {
        search.KeywordLabel = "(keywords: 'File', 'WCF', 'Started', 'Static' ...) [" + count + "]";
    }

    private void InitializeObjects()
    {
        gridSendPort.Visible = false;
        lblError.Visible = false;
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";
        this.search.SearchClick += new EventHandler(this.btnFilter_Click);
    }

    private void PopulateGrid(string searchKey)
    {
        try
        {
            StringCollection applicationList = null;

            // Check whether the Profile is active and then proceed.
            if (Profile.ControlCenterProfile.IsProfileActive)
            {
                try
                {
                    BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                    applicationList = props.ModuleDictionary[BCCUIHelper.Constants.APP_LIST_KEY];
                }
                catch
                {
                    // Ignore errors from Profile system
                }
            }

            if (applicationList != null && applicationList.Count > 0)
            {
                dt = dataAccess.RetrieveAllSendPorts(applicationList);

                // This is for the datalist view on the right side. 
                dlSPAppList.DataSource = applicationList;
                dlSPAppList.DataBind();
                dlSPAppList.Visible = true;
            }
            else
            {
                dt = dataAccess.RetrieveAllSendPorts();
            }

            if (searchKey != null && searchKey.Length > 0)
            {
                dt.DefaultView.RowFilter = "PortType LIKE '%" 
                    + searchKey 
                    + "%' OR  Application LIKE '%"
                    + searchKey 
                    + "%' OR SendPortName LIKE '%"
                    + searchKey 
                    + "%' OR URI LIKE '%"
                    + searchKey + "%'";
            }

            gridSendPort.DataSource = dt;
            gridSendPort.DataBind();
            gridSendPort.Visible = true;
            int count = gridSendPort.Rows.Count;

            if (count == 0)
            {
                sendPortPanel.Visible = false;
                sendPortPanel.Enabled = false;
                // Enable empty panel
                emptyPanel.Visible = true;
            }

            UpdateLabel(count);
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            gridSendPort.Visible = false;
        }
    }
    
    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        errorImg.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = false;
    }

    protected void gridSendPort_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int position = 3;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");

            if ( (BCCUIHelper.Constants.STATUS_UNENLISTED.Equals(e.Row.Cells[position].Text))
                || (BCCUIHelper.Constants.STATUS_ENLISTED.Equals(e.Row.Cells[position].Text))
                || (BCCUIHelper.Constants.STATUS_BOUND.Equals(e.Row.Cells[position].Text))
                )
            {
                e.Row.Cells[position].ForeColor = Color.DodgerBlue;
            }
            else if (BCCUIHelper.Constants.STATUS_STARTED.Equals(e.Row.Cells[position].Text))
            {
                e.Row.Cells[position].ForeColor = Color.Green;
            }
            else if (BCCUIHelper.Constants.STATUS_STOPPED.Equals(e.Row.Cells[position].Text))
            {
                e.Row.Cells[position].ForeColor = Color.Red;
            }
        }
    }

    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
        bccOperator.SetCheckAllCheckBox(gridSendPort, "chkBoxSendPort", true);
    }

    protected void btnDeSelectAll_Click(object sender, EventArgs e)
    {
        bccOperator.SetCheckAllCheckBox(gridSendPort, "chkBoxSendPort", false);
    }

    private void Monitor()
    {
        string sendPortName = string.Empty;

        // Iterate through the Gridview Rows property
        foreach (GridViewRow row in gridSendPort.Rows)
        {
            sendPortName = row.Cells[1].Text;
            // Access the CheckBox
            CheckBox cb = (CheckBox)row.FindControl("chkBoxSendPort");

            if (cb != null && cb.Checked)
            {
                try
                {
                    BCCMonitoringDataAccess da = new BCCMonitoringDataAccess();
                    da.CreateMonitoringEntry(ArtifactType.SendPort, sendPortName);
                    new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, " setup monitoring for send port " + sendPortName, 205);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Write(e.Message + e.StackTrace);
                }
            }
        }
    }

    private void OperateSendPort()
    {
        string errorMsg = string.Empty;
        string status = string.Empty;
        string sendPortName = string.Empty;

        // Iterate through the Gridview Rows property
        foreach (GridViewRow row in gridSendPort.Rows)
        {
            status = row.Cells[3].Text;
            sendPortName = row.Cells[1].Text;
            // Access the CheckBox
            CheckBox cb = (CheckBox)row.FindControl("chkBoxSendPort");
            if (cb != null && cb.Checked)
            {
                try
                {
                    if (("STOPPED".Equals(status.ToUpper()) || "BOUND".Equals(status.ToUpper())))
                    {
                        dataAccess.StartSendPort(sendPortName);
                        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "started send port " + sendPortName, 205);
                    }
                    else if ("STARTED".Equals(status.ToUpper()))
                    {
                        dataAccess.StopSendPort(sendPortName);
                        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "stopped send port " + sendPortName, 205);
                    }
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                }
            }
        }
    }
    
    protected void btnStart_Click(object sender, EventArgs e)
    {
        OperateSendPort();
        PopulateGrid(string.Empty);
    }

    protected void btnStop_Click(object sender, EventArgs e)
    {
        OperateSendPort();
        PopulateGrid(string.Empty);
    }

    protected void btnMonitor_Click(object sender, EventArgs e)
    {
        Monitor();
        PopulateGrid(string.Empty);
    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        SearchUserControl search = sender as SearchUserControl;
        PopulateGrid(search.SearchKeyword);
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        try
        {
            HtmlForm myMasterForm = Master.FindControl("form1") as HtmlForm;

            if (myMasterForm != null)
            {
                myMasterForm.Controls.Add(gridSendPort);
            }

            BCCGridView.Export("ControlCenter_" + System.Environment.MachineName + "_SendPorts.xls", this.gridSendPort);
        }
        catch
        {

        }
    }
}
