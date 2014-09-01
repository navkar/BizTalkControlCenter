using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BCC.Core;
using BCC.Core.WMI.BizTalk;

public partial class BiztalkReceivePort : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCOperator bccOperator = new BCCOperator();
    BCCUIHelper uiHelper = new BCCUIHelper();
    DataTable dt = null;
    string lastDirection = "ASC";
    string sortExpression = "ReceivePortLocation";

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

    private void ActivateGrid()
    {
        UserActivityGrid.Visible = true;
    }

    private void InitializeObjects()
    {
        gridReceivePort.Visible = false;
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
                dt = dataAccess.RetrieveAllReceivePorts(applicationList);

                // This is for the datalist view on the right side. 
                dlRLAppList.DataSource = applicationList;
                dlRLAppList.DataBind();
                dlRLAppList.Visible = true;
            }
            else
            {
                dt = dataAccess.RetrieveAllReceivePorts();
            }

            if (searchKey != null && searchKey.Length > 0)
            {
                dt.DefaultView.RowFilter = "Application LIKE '%" 
                    + searchKey 
                    + "%' OR ReceivePortName LIKE '%" 
                    + searchKey + "%' OR ReceivePortLocation LIKE '%" 
                    + searchKey + "%'";
            }

            gridReceivePort.DataSource = dt;
            gridReceivePort.DataBind();
            gridReceivePort.Visible = true;
            int count = gridReceivePort.Rows.Count;

            if (count == 0)
            {
                rcvPortPanel.Visible = false;
                rcvPortPanel.Enabled = false;
                // Enable empty panel
                emptyPanel.Visible = true;
            }

            UpdateLabel(count);
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            gridReceivePort.Visible = false;
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
        gridReceivePort.DataBind();
    }

    protected void gridReceivePort_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        gridReceivePort.PageIndex = e.NewPageIndex;
        gridReceivePort.DataBind();
    }

    protected void gridReceivePort_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int position = 4;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");

            if (BCCUIHelper.Constants.STATUS_ENABLED.Equals(e.Row.Cells[position].Text))
            {
                e.Row.Cells[position].ForeColor = Color.Green;
            }
            else
            if (BCCUIHelper.Constants.STATUS_DISABLED.Equals(e.Row.Cells[position].Text))
            {
                e.Row.Cells[position].ForeColor = Color.Gray;
            }
        }
    }

    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
        bccOperator.SetCheckAllCheckBox(gridReceivePort, "chkBoxRecvPort", true);
    }

    protected void btnDeSelectAll_Click(object sender, EventArgs e)
    {
        bccOperator.SetCheckAllCheckBox(gridReceivePort, "chkBoxRecvPort", false);
    }

    private void Monitor()
    {
        string receiveLocationName = string.Empty;
        string receivePortName = string.Empty;

        // Iterate through the Gridview Rows property
        foreach (GridViewRow row in gridReceivePort.Rows)
        {
            receivePortName = row.Cells[5].Text;
            receiveLocationName = row.Cells[1].Text;
            // Access the CheckBox
            CheckBox cb = (CheckBox)row.FindControl("chkBoxRecvPort");

            if (cb != null && cb.Checked)
            {
                try
                {
                    BCCMonitoringDataAccess da = new BCCMonitoringDataAccess();
                    da.CreateMonitoringEntry(ArtifactType.ReceivePort, receiveLocationName);
                    new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, " setup monitoring for receive location " + receiveLocationName, 206);
                }
                catch(Exception e)
                {
                    System.Diagnostics.Debug.Write(e.Message + e.StackTrace);
                }
            }
        }
    }

    private void OperateReceiveLocation(bool isEnabled)
    {
        string errorMsg = string.Empty;
        string receiveLocationName = string.Empty;
        string receivePortName = string.Empty;

        // Iterate through the Gridview Rows property
        foreach (GridViewRow row in gridReceivePort.Rows)
        {
            receivePortName = row.Cells[5].Text;
            receiveLocationName = row.Cells[1].Text;
            // Access the CheckBox
            CheckBox cb = (CheckBox)row.FindControl("chkBoxRecvPort");
            if (cb != null && cb.Checked)
            {
                try
                {
                    if (isEnabled)
                    {
                        dataAccess.EnableReceiveLocation(receivePortName, receiveLocationName);
                        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "enabled receive location " + receiveLocationName, 206);
                    }
                    else
                    {
                        dataAccess.DisableReceiveLocation(receivePortName, receiveLocationName);
                        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "disabled receive location " + receiveLocationName, 206);
                    }
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                }
            }
        }
    }

    protected void btnEnable_Click(object sender, EventArgs e)
    {
        OperateReceiveLocation(true);
        PopulateGrid(string.Empty);
    }

    protected void btnDisable_Click(object sender, EventArgs e)
    {
        OperateReceiveLocation(false);
        PopulateGrid(string.Empty);
    }

    protected void btnMonitor_Click(object sender, EventArgs e)
    {
        Monitor();
        PopulateGrid(string.Empty);
    }

    private void UpdateLabel(int count)
    {
        search.KeywordLabel = "(keywords: 'Enabled', 'WCF', 'Application' ...) [" + count + "]";
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        try
        {
            HtmlForm myMasterForm = (HtmlForm)Master.FindControl("form1");

            if (myMasterForm != null)
            {
                myMasterForm.Controls.Add(gridReceivePort);
            }

            BCCGridView.Export("ControlCenter_" + System.Environment.MachineName + "_ReceivePorts.xls", this.gridReceivePort);
        }
        catch
        {

        }
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        // DO NOT : remove this method. 
    }
}
