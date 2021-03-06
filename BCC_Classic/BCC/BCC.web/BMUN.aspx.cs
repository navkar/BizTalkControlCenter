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

public partial class UserNotifications : System.Web.UI.Page
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
            if (!Page.IsPostBack)
            {
                PopulateGrid(sortExpression, lastDirection);
                ActivateGrid();
                PopulateAnnoucements();
            }
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
                sortExpression = Session[SiteMap.CurrentNode.Description + "SortExpression"] as string;
            }
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
        SqlDataAdapter sqlDa = new SqlDataAdapter("select [ID], [ArtifactName], [ArtifactType], [ArtifactDesc], [PollingInterval], [StatusFlag], [LastUpdate]  from dbo.bcc_MonitoringList where [DeleteFlag] = 'False' order by [" + orderByColumn + "] " + Enum.GetName(typeof(BCCSortDirection), sortDirection), sqlConn);

        DataSet ds = null;

        try
        {
            ds = new DataSet();
            sqlDa.Fill(ds, "bcc_MonitoringList");
            gridNotifView.DataSource = ds.Tables["bcc_MonitoringList"].DefaultView;
            gridNotifView.DataBind();
            gridNotifView.Visible = true;
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            System.Diagnostics.Debug.Write(ex.Message + ex.StackTrace, SiteMap.CurrentNode.Description);
            gridNotifView.Visible = false;
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
        gridNotifView.Visible = true;
    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        errorImg.Visible = true;
    }

    protected void btnSelectAll_Click(object sender, EventArgs e)
    {
        bccOperator.SetCheckAllCheckBox(gridNotifView, "chkBoxNotify", true);
    }

    protected void btnDeSelectAll_Click(object sender, EventArgs e)
    {
        bccOperator.SetCheckAllCheckBox(gridNotifView, "chkBoxNotify", false);
    }

    protected void btnRemove_Click(object sender, EventArgs e)
    {
        string artifactType = string.Empty;
        string artifactName = string.Empty;

        foreach (GridViewRow row in gridNotifView.Rows)
        {
            CheckBox cb = (CheckBox)row.FindControl("chkBoxNotify");

            if (cb != null && cb.Checked)
            {
                Label lblType = row.FindControl("lblArtifactType") as Label;
                artifactType = lblType.Text;
                Label lblName = row.FindControl("lblArtifactName") as Label;
                artifactName = lblName.Text;

                try
                {
                    BCCMonitoringDataAccess da = new BCCMonitoringDataAccess();
                    da.MonitoringEntryMarkedForDeletion((BCC.Core.WMI.BizTalk.ArtifactType)Enum.Parse(typeof(BCC.Core.WMI.BizTalk.ArtifactType), artifactType), artifactName);

                    new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "removed " + artifactName, 103);
                    PopulateGrid(sortExpression, lastDirection);
                    AddAnnoucement("You can also disable monitoring of BizTalk artifacts instead of removing them.");
                    AddAnnoucement("The BCC agent takes 60 seconds to register new or changes to the BizTalk artifacts for monitoring.");
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                }
            }
        }
    }

    protected void gridNotifView_RowDataBound(object sender, GridViewRowEventArgs e)
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

            Label lblArtifactType = e.Row.Cells[1].FindControl("lblArtifactType") as Label;
            Label lblArtifactName = e.Row.Cells[2].FindControl("lblArtifactName") as Label;
            LinkButton lnkReport = e.Row.Cells[8].FindControl("lnkReport") as LinkButton;

            if (lblArtifactType != null && lblArtifactName != null && lnkReport != null)
            {
                lnkReport.Attributes.Add("onclick", "window.open('BMUN-R.aspx?AT="+  Server.UrlEncode(lblArtifactType.Text)  +"&AN=" +  Server.UrlEncode(lblArtifactName.Text) + "','','height=480,width=620');return false;");
            }
        }
    }

    protected void gridNotifView_OnSorting(object sender, GridViewSortEventArgs e)
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

    protected void gridNotifView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        gridNotifView.PageIndex = e.NewPageIndex;
        PopulateGrid(sortExpression, lastDirection);
    }

    protected void gridNotifView_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gridNotifView.EditIndex = e.NewEditIndex;
        PopulateGrid(sortExpression, lastDirection);
    }

    protected void gridNotifView_RowCancelEditing(object sender, GridViewCancelEditEventArgs e)
    {
        gridNotifView.EditIndex = -1;
        PopulateGrid(sortExpression, lastDirection);
    }

    protected void gridNotifView_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridViewRow updatableRow = gridNotifView.Rows[e.RowIndex] as GridViewRow;

        if (updatableRow != null)
        {
            Label lblArtifactType = updatableRow.FindControl("lblArtifactType") as Label;
            Label lblArtifactName = updatableRow.FindControl("lblArtifactName") as Label;

            TextBox tPoll = gridNotifView.Rows[e.RowIndex].FindControl("tPollInterval") as TextBox;
            DropDownList dStatus = gridNotifView.Rows[e.RowIndex].FindControl("ddlStatus") as DropDownList;

            SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["authStore"].ConnectionString);
            SqlCommand sqlCmd = new SqlCommand("[dbo].[bcc_MonitoringList_Update]", sqlConn);
            
            try
            {
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@artifactType", SqlDbType.VarChar, 25).Value = lblArtifactType.Text;
                sqlCmd.Parameters.Add("@artifactName", SqlDbType.VarChar, 255).Value = lblArtifactName.Text;
                sqlCmd.Parameters.Add("@artifactDesc", SqlDbType.VarChar, 2000).Value = "Notify after " + tPoll.Text + " seconds.";
                sqlCmd.Parameters.Add("@pollingInterval", SqlDbType.Int).Value = tPoll.Text;
                sqlCmd.Parameters.Add("@Status", SqlDbType.VarChar, 25).Value = dStatus.SelectedValue;
                sqlConn.Open();
                sqlCmd.ExecuteNonQuery();

                new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "updated monitoring data for artifact " + lblArtifactName.Text, 103);
                AddAnnoucement("Use Speedcode '603' to make changes to BCC Agent email settings. ");
                AddAnnoucement("The BCC agent takes 60 seconds to register new or changes to the BizTalk artifacts for monitoring.");
            }
            catch
            {

            }
        }

        // Refresh the data
        gridNotifView.EditIndex = -1;
        PopulateGrid(sortExpression, lastDirection);
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        HtmlForm myMasterForm = (HtmlForm)Master.FindControl("form1");

        if (myMasterForm != null)
        {
            myMasterForm.Controls.Add(gridNotifView);
        }

        BCCGridView.Export("ControlCenter_" + System.Environment.MachineName + "_NotificationAlerts.xls", this.gridNotifView);
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
	    // DO NOT : remove this method. 
    }
}
