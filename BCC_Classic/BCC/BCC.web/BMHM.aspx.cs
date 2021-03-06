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

public partial class HealthMatrix : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCUIHelper uiHelper = new BCCUIHelper();
    BCCOperator bccOperator = new BCCOperator();

    public string identifier = "\u25BA";
    public string messageID = "0";

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
        PopulateViewServiceDetails();
        PopulateHostInstances();
        PopulateMessageStatusCount();
        PopulateGridSendPort();
        PopulateGridReceivePort();
        PopulateDBStatus();
        PopulateAnnoucements();

        if (!Page.IsPostBack)
        {
            new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "viewed", 101);
            // Bind it for the first time alone.
            BindModuleConfiguration(SiteMap.CurrentNode.Description, BCC.Core.BCCUIHelper.Constants.REM_CHK_LIST_KEY);
        }
    }

    private void BindModuleConfiguration(string speedCode, string keyName)
    {
        StringCollection stringCollection = null;

        try
        {
            BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[speedCode];

            if (props != null)
            {
                stringCollection = props.ModuleDictionary[keyName];

                if (stringCollection != null)
                {
                    reminderCheckList.KeyName = keyName;
                    reminderCheckList.ControlDataSource = stringCollection;
                    reminderCheckList.DataBind();
                    reminderCheckList.Visible = true;
                }
            }
        }
        catch
        {
            reminderCheckList.Visible = false;
        }
    }

    /// <summary>
    /// Event handler for the Datagrid custom control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void reminderCheckList_ViewEvent(object sender, CollectionViewEventArgs e)
    {
        string speedCode = SiteMap.CurrentNode.Description;
        string keyName = BCC.Core.BCCUIHelper.Constants.REM_CHK_LIST_KEY;

        StringCollection stringCollection = null;

        // this is sweet code. loved it. 
        if (e.OperationCode == "Add" || e.OperationCode == "AddOnEmpty")
        {
            BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[speedCode];

            if (props != null)
            {
                if (e.KeyName != string.Empty)
                {
                    stringCollection = props.ModuleDictionary[keyName];
                }
            }

            if (stringCollection != null)
            {
                stringCollection.Add(e.ItemName);
            }
        }
        else if (e.OperationCode == "Remove")
        {
            BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[speedCode];

            if (props != null)
            {
                if (e.KeyName != string.Empty)
                {
                    stringCollection = props.ModuleDictionary[keyName];
                }
            }

            if (stringCollection != null)
            {
                stringCollection.Remove(e.ItemName);
            }
        }

        BindModuleConfiguration(speedCode, keyName);
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
            
            foreach(string yell in annoucementList)
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

    private void InitializeObjects()
    {
        gridServices.Visible = false;
        lblError.Visible = false;
        lblDefaultBizTalkApp.Text = dataAccess.RetrieveDefaultBizTalkApplication();
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";
    }

    private void PopulateHostInstances()
    {
        StringCollection hostInstanceList = null;
        DataTable dt = null;

        try
        {
            // Check whether the Profile is active and then proceed.
            if (Profile.ControlCenterProfile.IsProfileActive)
            {
                try
                {
                    BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                    hostInstanceList = props.ModuleDictionary[BCCUIHelper.Constants.HOST_INSTANCE_KEY];
                }
                catch
                {
                    // Ignore errors from Profile system
                }
            }

            if (hostInstanceList != null && hostInstanceList.Count > 0)
            {
                dt = dataAccess.RetrieveAllHostsAndInstance(hostInstanceList, true);
            }
            else
            {
                dt = dataAccess.RetrieveAllHostsAndInstance(null, false);
            }

            gridHost.DataSource = dt;
            gridHost.DataBind();
            gridHost.Visible = true;
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
        }
    }

    protected void gridHost_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        gridHost.PageIndex = e.NewPageIndex;
        PopulateHostInstances();
    }

    protected void lnkHost_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/BAHI.aspx");
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

            // Specifying the maximum limit
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
            dr[0] = "Suspended (R)";
            dr[1] = suspendedMsgCount;
            dtMsg.Rows.Add(dr);

            dr = dtMsg.NewRow();
            dr[0] = "Suspended (NR)";
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

    #region Message Grid
    protected void gridMsgInfo_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        LinkButton lnkMessageState = e.Row.Cells[0].FindControl("lnkMessageState") as LinkButton;
        //lblMsgCount
        LinkButton lnkMsgCount = e.Row.Cells[1].FindControl("lnkMsgCount") as LinkButton;

        if (lnkMessageState != null && lnkMsgCount != null)
        {
            if ((lnkMessageState.Text.Equals("Suspended (R)") || lnkMessageState.Text.Equals("Suspended (NR)")) && (!lnkMsgCount.Text.Equals("0")))
            {
                lnkMessageState.BackColor = Color.RosyBrown;
                lnkMessageState.ForeColor = Color.White;
                lnkMessageState.Font.Bold = true;
                lnkMessageState.BorderColor = Color.Black;
                lnkMessageState.BorderStyle = BorderStyle.Solid;
                lnkMessageState.BorderWidth = 1;

                lnkMsgCount.BackColor = Color.RosyBrown;
                lnkMsgCount.ForeColor = Color.White;
                lnkMsgCount.Font.Bold = true;
                lnkMsgCount.BorderColor = Color.Black;
                lnkMsgCount.BorderStyle = BorderStyle.Solid;
                lnkMsgCount.BorderWidth = 1;
            }
        }
    }

    public void lnkMsg_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/BAMV.aspx");
    }
    #endregion

    private void PopulateViewServiceDetails()
    {
        try
        {
            StringCollection serviceList = null;

            try
            {
                BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                serviceList = props.ModuleDictionary[BCCUIHelper.Constants.SERVICE_PROFILE_KEY];
            }
            catch (Exception exception)
            {
                DisplayError(exception.Message);
            }

            if (serviceList != null && serviceList.Count > 0)
            {
                DataTable dtService = bccOperator.GetServiceStatus(serviceList);

                dtService.DefaultView.Sort = "ServiceName asc";
                gridServices.DataSource = dtService;
                gridServices.DataBind();
                gridServices.Visible = true;
            }
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message.ToString());
            gridServices.Visible = false;
        }
    }

    protected void gridServices_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        gridServices.PageIndex = e.NewPageIndex;
        PopulateViewServiceDetails();
    }

    protected void lnkService_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/BAWS.aspx");
    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        errorImg.Visible = true;
    }

    private void PopulateDBStatus()
    {
        StringCollection connectionStringList = null;
        DataTable dt = null;

        try
        {
            if (Profile.ControlCenterProfile.IsProfileActive)
            {
                try
                {
                    BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                    connectionStringList = props.ModuleDictionary[BCCUIHelper.Constants.DB_SERVER_LIST_KEY];
                }
                catch
                {
                    // Ignore errors from Profile system
                }
            }

            if (connectionStringList != null)
            {
                dt = dataAccess.DatabaseCheck(connectionStringList);
                gridDBStatus.DataSource = dt;
                gridDBStatus.DataBind();
                gridDBStatus.Visible = true;
            }
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
        }
    }

    protected void gridDB_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        gridDBStatus.PageIndex = e.NewPageIndex;
        PopulateDBStatus();
    }

    private void PopulateGridReceivePort()
    {
        StringCollection applicationList = null;
        DataTable dt = null;

        try
        {
            if (Profile.ControlCenterProfile.IsProfileActive)
            {
                try
                {
                    BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                    applicationList = props.ModuleDictionary["RL"+BCCUIHelper.Constants.APP_LIST_KEY];
                }
                catch
                {
                    // Ignore errors from Profile system
                }
            }

            if (applicationList != null && applicationList.Count > 0)
            {
                dt = dataAccess.RetrieveAllReceivePorts(applicationList);
            }
            else
            {
                dt = dataAccess.RetrieveAllReceivePorts();
            }

            dt.DefaultView.Sort = "Application asc, ReceivePortName asc";
            gridReceivePort.DataSource = dt;
            gridReceivePort.DataBind();
            gridReceivePort.Visible = true;
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            gridReceivePort.Visible = false;
        }
    }

    protected void gridReceivePort_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        gridReceivePort.PageIndex = e.NewPageIndex;
        PopulateGridReceivePort();
    }

    protected void lnkReceivePort_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/BARL.aspx");
    }

    private void PopulateGridSendPort()
    {
        StringCollection applicationList = null;
        DataTable dt = null;
        
        try
        {
            // Check whether the Profile is active and then proceed.
            if (Profile.ControlCenterProfile.IsProfileActive)
            {
                try
                {
                    BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                    applicationList = props.ModuleDictionary["SP" + BCCUIHelper.Constants.APP_LIST_KEY];
                }
                catch
                {
                    // Ignore errors from Profile system
                }
            }

            if (applicationList != null && applicationList.Count > 0)
            {
                dt = dataAccess.RetrieveAllSendPorts(applicationList);
            }
            else
            {
                dt = dataAccess.RetrieveAllSendPorts();
            }

            dt.DefaultView.Sort = "SendPortName asc";
            gridSendPort.DataSource = dt;
            gridSendPort.DataBind();
            gridSendPort.Visible = true;
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            gridSendPort.Visible = true;
        }
    }

    protected void gridSendPort_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        gridSendPort.PageIndex = e.NewPageIndex;
        PopulateGridSendPort();
    }

    protected void lnkSendPort_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/BASP.aspx");
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
	// DO NOT : remove this method. 
    }
}
