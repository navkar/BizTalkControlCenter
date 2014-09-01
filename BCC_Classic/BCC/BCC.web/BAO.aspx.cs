using System;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using BCC.Core;
using Microsoft.BizTalk.ExplorerOM;
using System.Collections.Specialized;


public partial class Orchestration : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCOperator bccOperator = new BCCOperator();
    BCCUIHelper uiHelper = new BCCUIHelper();
    private const string SELECT_CONTROL_NAME = "chkBoxOdx";

    DataTable dt = null;
    string lastDirection = "ASC";
    string sortExpression = "Name";

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
        sqlDS.SelectCommand = BCC.Core.BCCUIHelper.BizTalkOrchestrationQueryString();

        InitializeObjects();

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

        if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN)
            || User.IsInRole(BCCUIHelper.Constants.ROLE_ARTIFACT))
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
        gridOdx.Visible = false;
        lblError.Visible = false;
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";
        this.search.SearchClick += new EventHandler(this.btnFilter_Click);
    }

    private void UpdateLabel(int count)
    {
        search.KeywordLabel = "(keywords: '1.0', 'Started', 'Application', ...) [" + count + "]";
    }

    protected void gridOdx_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        gridOdx.PageIndex = e.NewPageIndex;
        gridOdx.DataBind();
    }

    protected void OnSort(object sender, GridViewSortEventArgs e)
    {
        string sortExpression2 = e.SortExpression;
        lastDirection = (string) Session[SiteMap.CurrentNode.Description + "SortDirection"];

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
        gridOdx.DataBind();
    }

    //private void BindGridView()
    //{
    //    StringCollection applicationList = null;

    //    // Check whether the Profile is active and then proceed.
    //    if (Profile.ControlCenterProfile.IsProfileActive)
    //    {
    //        try
    //        {
    //            BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
    //            applicationList = props.ModuleDictionary[BCCUIHelper.Constants.APP_LIST_KEY];
    //        }
    //        catch
    //        {
    //            // Ignore errors from Profile system
    //        }
    //    }

    //    if (applicationList != null && applicationList.Count > 0)
    //    {
    //        dt = dataAccess.RetrieveAllOrchestrations(applicationList);
    //    }
    //    else
    //    {
    //        dt = dataAccess.RetrieveAllOrchestrations();
    //    }

    //    gridOdx.DataSource = dt;
    //    int count = gridOdx.Rows.Count;
    //    gridOdx.DataBind();
    //}

    private void Debug(string message)
    {
        System.Diagnostics.Debug.WriteLine(message, SiteMap.CurrentNode.Description);
    }

    private void PopulateGrid(string searchKeyword)
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
                dt = dataAccess.RetrieveAllOrchestrations(applicationList);

                // This is for the datalist view on the right side. 
                dlOAppList.DataSource = applicationList;
                dlOAppList.DataBind();
                dlOAppList.Visible = true;
            }
            else
            {
                dt = dataAccess.RetrieveAllOrchestrations();
            }

            if (searchKeyword != null && searchKeyword.Length > 0)
            {
                dt.DefaultView.RowFilter = "VERSION LIKE '%"
                    + searchKeyword + "%' OR APPLICATION LIKE '%"
                    + searchKeyword + "%' OR NAME LIKE '%"
                    + searchKeyword + "%' OR STATUS LIKE '%"
                    + searchKeyword + "%'";
            }

            dt.DefaultView.Sort = "NAME ASC";
            gridOdx.DataSource = dt;
            gridOdx.DataBind();
            gridOdx.Visible = true;
            int count = gridOdx.Rows.Count;

            if (count == 0)
            {
                masterPanel.Visible = false;
                detailPanel.Visible = false;
                masterPanel.Enabled = false;
                detailPanel.Enabled = false;

                // Enable empty panel
                emptyPanel.Visible = true;
            }

            UpdateLabel(count);
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            gridOdx.Visible = false;
        }
    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        errorImg.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = false;
    }

    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
        bccOperator.SetCheckAllCheckBox(gridOdx, SELECT_CONTROL_NAME, true);
    }

    protected void btnDeSelectAll_Click(object sender, EventArgs e)
    {
        bccOperator.SetCheckAllCheckBox(gridOdx, SELECT_CONTROL_NAME, false);
    }

    private void ChangeOrchestrationState()
    {
        string errorMsg = string.Empty;
        string status = string.Empty;
        string orchestrationName = string.Empty;
        // Iterate through the Gridview Rows property
        foreach (GridViewRow row in gridOdx.Rows)
        {
            status = row.Cells[3].Text;
            // Access the CheckBox
            CheckBox cb = (CheckBox)row.FindControl(SELECT_CONTROL_NAME);
            LinkButton lb = (LinkButton)row.FindControl("btnDetail");
            if (lb != null && (cb != null && cb.Checked))
            {
                orchestrationName = lb.Text;

                try
                {
                    if ("UNENLISTED".Equals(status.ToUpper()) || "ENLISTED".Equals(status.ToUpper()))
                    {
                        dataAccess.StartUnenlistOrchestration(orchestrationName, true);
                        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "started " + orchestrationName, 203);
                    }
                    else if ("STARTED".Equals(status.ToUpper()))
                    {
                        dataAccess.StartUnenlistOrchestration(orchestrationName, false);
                        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "stopped " + orchestrationName, 203);
                    }
                }
                catch (Exception exception)
                {
                    DisplayError(exception.Message);
                }
            }
        }
    }

    protected void btnStart_Click(object sender, EventArgs e)
    {
        ChangeOrchestrationState();
        PopulateGrid(string.Empty);
    }

    protected void btnStop_Click(object sender, EventArgs e)
    {
        ChangeOrchestrationState();
        PopulateGrid(string.Empty);
    }

    // PortView_RowDataBound
    protected void PortView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (BCCUIHelper.Constants.STATUS_STARTED.Equals(e.Row.Cells[1].Text))
            {
                e.Row.Cells[1].ForeColor = Color.Green;
            }
            else if (BCCUIHelper.Constants.STATUS_STOPPED.Equals(e.Row.Cells[1].Text))
            {
                e.Row.Cells[1].ForeColor = Color.Red;
            }
            else if (BCCUIHelper.Constants.STATUS_UNENLISTED.Equals(e.Row.Cells[1].Text))
            {
                e.Row.Cells[1].ForeColor = Color.DodgerBlue;
            }

            if (BCCUIHelper.Constants.STATUS_STARTED.Equals(e.Row.Cells[6].Text) || BCCUIHelper.Constants.STATUS_ENABLED.Equals(e.Row.Cells[6].Text))
            {
                e.Row.Cells[6].ForeColor = Color.Green;
            }
            else
            {
                e.Row.Cells[6].ForeColor = Color.Red;
            }
        }
    }

    protected void gridOdxDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (BCCUIHelper.Constants.STATUS_STARTED.Equals(e.Row.Cells[1].Text))
        {
            e.Row.Cells[1].ForeColor = Color.Green;
        }
        else if (BCCUIHelper.Constants.STATUS_STOPPED.Equals(e.Row.Cells[1].Text))
        {
            e.Row.Cells[1].ForeColor = Color.Red;
        }
        else if (BCCUIHelper.Constants.STATUS_UNENLISTED.Equals(e.Row.Cells[1].Text))
        {
            e.Row.Cells[1].ForeColor = Color.DodgerBlue;
        }

        if (e.Row.Cells[1].Text.Contains("Host"))
        {
            e.Row.Cells[1].ForeColor = Color.Brown;
        }
        else if (e.Row.Cells[1].Text.Contains("unbound"))
        {
            e.Row.Cells[1].ForeColor = Color.Red;
        }
    }

    protected void gridOdx_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int position = 3;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");

            if (BCCUIHelper.Constants.STATUS_UNENLISTED.Equals(e.Row.Cells[position].Text))
            {
                e.Row.Cells[position].ForeColor = Color.DodgerBlue;
            }
            else if (BCCUIHelper.Constants.STATUS_ENLISTED.Equals(e.Row.Cells[position].Text))
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

            if (e.Row.Cells[5].Text.Contains("1.0.0.0"))
            {
                e.Row.Cells[5].ForeColor = Color.SteelBlue;
                e.Row.Cells[5].ForeColor = Color.SteelBlue;
            }

            if (e.Row.Cells[position].Text.Contains("unbound"))
            {
                e.Row.Cells[position].ForeColor = Color.Red;
            }
        }
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
            HtmlForm myMasterForm = (HtmlForm)Master.FindControl("form1");

            if (myMasterForm != null)
            {
                myMasterForm.Controls.Add(gridOdx);
            }

            BCCGridView.Export("ControlCenter_" + System.Environment.MachineName + "_Orchestrations.xls", this.gridOdx);
        }
        catch
        {

        }
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        // DO NOT : remove this method. 
    }

    protected void btnGoBack_Click(object sender, EventArgs e)
    {
        ShowMasterPanel();
    }

    protected void btnDetail_Click(object sender, EventArgs e)
    {
        LinkButton linkButton = (LinkButton)sender;

        fillInOrchestrationDetails(linkButton.Text);
    }

    private void ShowDetailPanel()
    {
        masterPanel.Visible = false;
        detailPanel.Visible = true;
    }

    private void ShowMasterPanel()
    {
        detailPanel.Visible = false;
        masterPanel.Visible = true;
    }

    private void fillInOrchestrationDetails(String orchestrationName)
    {
        BtsOrchestration orchestration = dataAccess.GetOrchestrationByName(orchestrationName);

        if (orchestration != null)
        {
            DataTable orchDetails = new DataTable();
            orchDetails.Columns.Add(new DataColumn("PropertyName", typeof(string)));
            orchDetails.Columns.Add(new DataColumn("PropertyValue", typeof(string)));

            DataRow dr = null;
            dr = orchDetails.NewRow();
            dr[0] = "Orchestration";
            dr[1] = orchestration.FullName;
            orchDetails.Rows.Add(dr);
            //--------------------------------------------------------
            dr = orchDetails.NewRow();
            dr[0] = "Application";
            dr[1] = orchestration.Application.Name;
            orchDetails.Rows.Add(dr);

            dr = orchDetails.NewRow();
            dr[0] = "Assembly Name";
            dr[1] = orchestration.BtsAssembly.Name;
            orchDetails.Rows.Add(dr);

            dr = orchDetails.NewRow();
            dr[0] = "Assembly Version";
            dr[1] = orchestration.BtsAssembly.Version;
            orchDetails.Rows.Add(dr);

            dr = orchDetails.NewRow();
            dr[0] = "Public Key Token";
            dr[1] = orchestration.BtsAssembly.PublicKeyToken;
            orchDetails.Rows.Add(dr);

            dr = orchDetails.NewRow();
            dr[0] = "Status";
            dr[1] = orchestration.Status.ToString();
            orchDetails.Rows.Add(dr);

            dr = orchDetails.NewRow();
            dr[0] = "Configured Host";

            Microsoft.BizTalk.ExplorerOM.Host host = orchestration.Host;

            if (host != null)
            {
                dr[1] = orchestration.Host.Name;
            }
            else
            {
                dr[1] = "unbound";
            }

            orchDetails.Rows.Add(dr);

           
            DataTable portInfo = new DataTable();

            portInfo.Columns.Add(new DataColumn("LogicalPortName", typeof(string)));
            portInfo.Columns.Add(new DataColumn("PortBinding", typeof(string)));
            portInfo.Columns.Add(new DataColumn("PortOperation", typeof(string))); 
            portInfo.Columns.Add(new DataColumn("PhysicalPortName", typeof(string)));
            portInfo.Columns.Add(new DataColumn("PortParent", typeof(string)));  // Receive Port / Send Port Group
            portInfo.Columns.Add(new DataColumn("PortStatus", typeof(string)));  // Receive Port / Send Port Group
            portInfo.Columns.Add(new DataColumn("PortDirection", typeof(string))); // Send or Receive

            foreach (OrchestrationPort port in orchestration.Ports)
            {
                bool emptyFlag = true;
                dr = portInfo.NewRow();
                dr[0] = port.Name;
                dr[1] = port.Binding.ToString();
                
                // Operation
                foreach (PortTypeOperation portOperation in port.PortType.Operations)
                {
                    dr[2] = portOperation.Name;
                }

                if (port.ReceivePort != null)
                {
                    emptyFlag = false;
                    dr[4] = port.ReceivePort.Name;
                    dr[6] = "Receive";

                    foreach (ReceiveLocation receiveLocation in port.ReceivePort.ReceiveLocations)
                    {
                        dr[3] = receiveLocation.Name;
                        dr[5] = (receiveLocation.Enable ? "Enabled" : "Disabled");
                    }
                }
                else
                    if (port.SendPort != null)
                    {
                        emptyFlag = false;
                        dr[3] = port.SendPort.Name;
                        dr[4] = "none";
                        dr[5] = port.SendPort.Status.ToString();
                        dr[6] = "Send";
                    }
                    else
                        if (port.SendPortGroup != null)
                        {
                            emptyFlag = false;
                            dr[3] = port.SendPortGroup.Name;
                            dr[4] = "none";
                            dr[5] = port.SendPortGroup.Status.ToString();
                            dr[6] = "Send";
                        }

                if (emptyFlag)
                {
                    dr[3] = "unbounded";
                    dr[4] = "unbounded";
                    dr[5] = "unbounded";
                    dr[6] = "none";
                }

                portInfo.Rows.Add(dr);
            }

            if (portInfo.Rows.Count > 0)
            {
                PortView.DataSource = portInfo;
                PortView.DataBind();
                PortView.Visible = true;
            }

            gridOdxDetail.DataSource = orchDetails;
            gridOdxDetail.DataBind();
            gridOdxDetail.Visible = true;

            ShowDetailPanel();
        }
    }
}
