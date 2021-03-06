using System;
using System.Data;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BCC.Core;
using System.Drawing;
using System.Xml;
using System.Data.SqlClient;

public partial class MonitorPerformance : System.Web.UI.Page
{
    private BCCUIHelper uiHelper = new BCCUIHelper();
    private BCCOperator bccOperator = new BCCOperator();
    private BCCSortDirection lastDirection = BCCSortDirection.DESC;
    private string sortExpression = "LastUpdate";

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
        Session["PAGE_THEME"] = this.Page.Theme;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeObjects();

        if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN) || User.IsInRole(BCCUIHelper.Constants.ROLE_ARTIFACT))
        {
            //if (!Page.IsPostBack)
            //{
                PopulateGrid(sortExpression, lastDirection);
                ActivateGrid();
                PopulateAnnoucements();
            //}
        }
        else
        {
            DisplayError(BCCUIHelper.Constants.ACCESS_DENIED);
        }

        if (Page.IsPostBack)
        {
            if (Session[SiteMap.CurrentNode.Description + "SortDirection"] != null)
            {
                lastDirection = (BCCSortDirection)Session[SiteMap.CurrentNode.Description + "SortDirection"];
            }
            else
            {
                Session[SiteMap.CurrentNode.Description + "SortDirection"] = lastDirection;
            }

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

    private void PopulateGrid(string orderByColumn, BCCSortDirection sortDirection)
    {
        SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString);
        SqlDataAdapter sqlDa = new SqlDataAdapter("select [ID], [PerfCategory], [PerfCounterName], [PerfInstance], [PollingInterval], [StatusFlag], [LastUpdate]  from dbo.[bcc_PerfCounterList] where [DeleteFlag] = 'False' order by [" 
            + orderByColumn 
            + "] " 
            + Enum.GetName(typeof(BCCSortDirection), sortDirection), sqlConn);

        DataSet ds = null;

        try
        {
            ds = new DataSet();
            sqlDa.Fill(ds, "[bcc_PerfCounterList]");
            gridPerfMon.DataSource = ds.Tables["[bcc_PerfCounterList]"].DefaultView;
            gridPerfMon.DataBind();
            gridPerfMon.Visible = true;
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            System.Diagnostics.Debug.Write(ex.Message + ex.StackTrace, SiteMap.CurrentNode.Description);
            gridPerfMon.Visible = false;
        }
    }

    private void PopulateAnnoucements()
    {
        StringCollection annoucementList = null;
        String data = string.Empty;
        // Retrieve the user profile and display annoucements if any.
        try
        {
            BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
            annoucementList = props.ModuleDictionary[BCCUIHelper.Constants.ANNOUNCE_APP_KEY];
            
            foreach (string yell in annoucementList)
            {
                data = yell + BCCUIHelper.Constants.LINE_BK + data;
            }

            DisplayAnnouncement(data);

            // Self destruct after display
            annoucementList.Clear();
        }
        catch
        {
            // Ignore errors from Profile system
        }
    }

    private void DisplayAnnouncement(string message)
    {
        if (message != null && message.Length > 0)
        {
            Label announcement = this.Master.FindControl("lblAnnouncement") as Label;
            Panel announcePanel = this.Master.FindControl("announcePanel") as Panel;

            if (announcement != null)
            {
                announcement.Text = message;
                announcement.Visible = true;
                announcePanel.Visible = true;
            }
        }
    }

    private void AddAnnoucement(string message)
    {
        StringCollection annoucementList = null;
        BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
        annoucementList = props.ModuleDictionary[BCCUIHelper.Constants.ANNOUNCE_APP_KEY];

        if (annoucementList.Count < 2)
        {
            annoucementList.Add(message);
        }
    }

    private void InitializeObjects()
    {
        lblError.Visible = false;
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";
        gridPerfMon.Visible = true;
    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        errorImg.Visible = true;
    }

    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
        bccOperator.SetCheckAllCheckBox(gridPerfMon, "chkBoxNotify", true);
    }

    protected void btnDeSelectAll_Click(object sender, EventArgs e)
    {
        bccOperator.SetCheckAllCheckBox(gridPerfMon, "chkBoxNotify", false);
    }

    protected void btnRemove_Click(object sender, EventArgs e)
    {
        string categoryName = string.Empty;
        string counterName = string.Empty;
        string instanceName = string.Empty;

        foreach (GridViewRow row in gridPerfMon.Rows)
        {
            CheckBox cb = (CheckBox)row.FindControl("chkBoxNotify");

            if (cb != null && cb.Checked)
            {
                Label lblCategory = row.FindControl("lblCategory") as Label;
                categoryName = lblCategory.Text;

                Label lblCounter = row.FindControl("lblCounter") as Label;
                counterName = lblCounter.Text;

                Label lblInstance = row.FindControl("lblInstance") as Label;
                instanceName = lblInstance.Text;

                try
                {
                    BCCPerfCounterEntry entry = new BCCPerfCounterEntry(categoryName, instanceName, counterName);
                    BCCPerfCounterDataAccess da = new BCCPerfCounterDataAccess();
                    da.PerformanceCounterEntryMarkedForDeletion(entry);

                    new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "removed '" + counterName + "'", 104);
                    PopulateGrid(sortExpression, lastDirection);
                    AddAnnoucement("You can also disable monitoring of Performance counters instead of removing them.");
                    AddAnnoucement("The BCC agent takes 60 seconds to register changes to the performance counters for monitoring.");
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                }
            }
        }
    }

    protected void gridPerfMon_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int position = 5;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");

            Label lblStatus = e.Row.Cells[position].FindControl("lblStatus") as Label;

            if (lblStatus != null)
            {
                if (lblStatus.Text == "True")
                {
                    lblStatus.Text = "Enabled";
                    lblStatus.ForeColor = Color.Green;
                }
                else
                {
                    lblStatus.Text = "Disabled";
                    lblStatus.ForeColor = Color.Gray;
                }
            }

            Label lblCategory = e.Row.Cells[1].FindControl("lblCategory") as Label;
            Label lblCounter = e.Row.Cells[2].FindControl("lblCounter") as Label;
            Label lblInstance = e.Row.Cells[2].FindControl("lblInstance") as Label;
            Label lblPollingInterval = e.Row.Cells[5].FindControl("lblPollingInterval") as Label;
            
            LinkButton lnkReport = e.Row.Cells[8].FindControl("lnkReport") as LinkButton;

            if (lblCategory != null && lblCounter != null && lblInstance != null && lnkReport != null && lblPollingInterval != null)
            {
                lnkReport.Attributes.Add("onclick", "window.open('BMMP-R.aspx?CAT=" +  Server.UrlEncode(lblCategory.Text) + "&CNTR=" +  Server.UrlEncode(lblCounter.Text) + "&POLL=" +  Server.UrlEncode(lblPollingInterval.Text) + "&INST=" + Server.UrlEncode(lblInstance.Text) + "','','height=480,width=800');return false;");
            }
        }
    }

    protected void gridPerfMon_OnSorting(object sender, GridViewSortEventArgs e)
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

        Session[SiteMap.CurrentNode.Description + "SortDirection"] = lastDirection;
        Session[SiteMap.CurrentNode.Description + "SortExpression"] = sortExpression2;
        PopulateGrid(sortExpression2, lastDirection);
    }

    protected void gridPerfMon_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        gridPerfMon.PageIndex = e.NewPageIndex;
        PopulateGrid(sortExpression, lastDirection);
    }

    protected void gridPerfMon_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gridPerfMon.EditIndex = e.NewEditIndex;
        PopulateGrid(sortExpression, lastDirection);
    }

    protected void gridPerfMon_RowCancelEditing(object sender, GridViewCancelEditEventArgs e)
    {
        gridPerfMon.EditIndex = -1;
        PopulateGrid(sortExpression, lastDirection);
    }

    protected void gridPerfMon_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridViewRow updatableRow = gridPerfMon.Rows[e.RowIndex] as GridViewRow;

        if (updatableRow != null)
        {
            Label lblCategory = updatableRow.FindControl("lblCategory") as Label;
            Label lblCounter = updatableRow.FindControl("lblCounter") as Label;
            Label lblInstance = updatableRow.FindControl("lblInstance") as Label;

            TextBox tPoll = gridPerfMon.Rows[e.RowIndex].FindControl("tPollInterval") as TextBox;
            DropDownList dStatus = gridPerfMon.Rows[e.RowIndex].FindControl("ddlStatus") as DropDownList;

            SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString);
            SqlCommand sqlCmd = new SqlCommand("[dbo].[bcc_PerfCounterList_Update]", sqlConn);
            
            // check for valid instance before enabling
            bool isValid = BCCPerformanceCounters.IsPerformanceCounterInstanceValid(lblCategory.Text, lblInstance.Text);

            if (!isValid)
            {
                DisplayError("Performance counter instance '" + lblInstance.Text + "' for '" + lblCounter.Text + "' does not exist. See speedcode '303'.");
            }
            else
            {
                int pollValue = 0;
                if (Int32.TryParse(tPoll.Text, out pollValue))
                {
                    try
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Add("@perfCategory", SqlDbType.VarChar, 255).Value = lblCategory.Text;
                        sqlCmd.Parameters.Add("@perfInstance", SqlDbType.VarChar, 255).Value = lblInstance.Text;
                        sqlCmd.Parameters.Add("@perfCounterName", SqlDbType.VarChar, 255).Value = lblCounter.Text;
                        sqlCmd.Parameters.Add("@pollingInterval", SqlDbType.Int).Value = pollValue.ToString();
                        sqlCmd.Parameters.Add("@Status", SqlDbType.Bit).Value = dStatus.SelectedValue;
                        sqlConn.Open();
                        sqlCmd.ExecuteNonQuery();

                        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "updated performance counter '" + lblCounter.Text + "'", 104);
                        AddAnnoucement("Use Speedcode '604' to make changes to BCC Agent email settings. ");
                        AddAnnoucement("The BCC agent takes 60 seconds to register changes to the performance counters for monitoring.");
                    }
                    catch(Exception ex)
                    {
                        DisplayError("Exception during update: " + ex.Message);
                    }
                }
                else
                {
                    DisplayError("Value '" + tPoll.Text + "' is invalid for polling interval.");
                }
            }
        }

        // Refresh the data
        gridPerfMon.EditIndex = -1;
        PopulateGrid(sortExpression, lastDirection);
    }
    
    public override void VerifyRenderingInServerForm(Control control)
    {
	// DO NOT : remove this method. 
    }
}
