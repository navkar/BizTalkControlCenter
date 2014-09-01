using System;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net;
using System.IO;
using BCC.Core;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml;
using System.Web.Profile;
using System.Collections.Specialized;

public partial class AgentSettings : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCUIHelper uiHelper = new BCCUIHelper();
    protected Controls_EditBox[] editBoxControls = null;

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

    protected string UTCTime
    {
        get
        {
            return DateTime.UtcNow.ToString("HH:mm");
        }
    }

    private void LoadEmailConfiguration()
    {
        try
        {
            BCC.Core.BCCManageConfigData configData = new BCCManageConfigData();
            configData.Speedcode = SiteMap.CurrentNode.Description;
            configData.Query();

            NameValuePairSet configSet = configData.ConfigurationData;

            editBoxControls = new Controls_EditBox[configSet.Count];

            for (int count = 0; count < configSet.Count; count++)
            {
                editBoxControls[count] = (Controls_EditBox)LoadControl("~/Controls/EditBox.ascx");
                editBoxControls[count].ID = "EditBox_" + (count + 1);
                editBoxControls[count].LabelName = configSet[count].Name;
                editBoxControls[count].DisplayName = configSet[count].DisplayName;
                editBoxControls[count].ToolTip = configSet[count].ToolTip;
                
                if (configSet[count].DisplayName.Contains("Password"))
                {
                    editBoxControls[count].IsPasswordField = true;
                }

                editBoxControls[count].TextValue = configSet[count].Value;

                phEmailConfig.Controls.Add(editBoxControls[count]);
            }
        }
        catch (Exception e)
        {
            phEmailConfig.Controls.Add( new LiteralControl("Unable to load email configuration from the BCC database. Error:" + e.Message));
            lnkSaveEmailProp.Enabled = false;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
            subCaption.Text = "Information";

            // TODO: Remove this later
            this.Profile.ControlCenterProfile.IsProfileActive = true;

            if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN))
            {
                ActivateGrid();
                PopulateViewServiceDetails();
                LoadEmailConfiguration();
            }
            else
            {
                DisplayError(BCCUIHelper.Constants.ACCESS_DENIED);
                DisableControls();
            }
        }
        catch(Exception exception)
        {
            DisplayError(exception.Message);
        }
    }

    private void ActivateGrid()
    {
        UserActivityGrid.Visible = true;
    }

    private void DisableControls()
    {
        phEmailConfig.Visible = false;
        lnkSaveEmailProp.Visible = false;
        AgentPanel.Visible = false;
    }


    private void PopulateViewServiceDetails()
    {
        try
        {
            StringCollection serviceList = new StringCollection();
            serviceList.Add(ConfigurationManager.AppSettings["BCCAgentName"].ToString());

            if (serviceList != null && serviceList.Count > 0)
            {
                BCCOperator bccOperator = new BCCOperator();
                DataTable dtService = bccOperator.GetServiceStatus(serviceList);

                gridBCCAgent.DataSource = dtService;
                gridBCCAgent.DataBind();
                gridBCCAgent.Visible = true;
            }
        }
        catch (Exception ex)
        {
            DisplayError("Specify the correct service name in the filters. See 'Administration > System Settings'." + ex.Message);
        }
    }

    protected void DisplayError(string message)
    {
        lblStatus.Text = message;
        lblStatus.Font.Bold = false;
        
        lblStatus.Visible = true;
        errorImg.Visible = true;
    }

    protected void DisplayInformation(string message)
    {
        lblStatus.Text = message;
        lblStatus.Font.Bold = false;
	    lblStatus.ForeColor = Color.Teal;
        lblStatus.Visible = true;
    }

    protected void SaveProperties_Click(object sender, EventArgs e)
    {
        BCC.Core.BCCManageConfigData configData = new BCCManageConfigData();
        configData.Speedcode = SiteMap.CurrentNode.Description;
        configData.Query();

        NameValuePairSet configSet = configData.ConfigurationData;

        for (int count = 0; count < configSet.Count; count++)
        {
            if (editBoxControls[count].LabelName == configSet[count].Name)
            {
                configSet[count].Value = editBoxControls[count].TextValue;
            }
        }

        configData.Update();
        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "updated config data", 604);
    }

    protected void gridFilters_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.Cells[2].Text.Contains("BizTalk"))
        {
            e.Row.Cells[2].ForeColor = Color.Brown;
        }
        else
            if (e.Row.Cells[2].Text.Contains("BTSSvc"))
            {
                e.Row.Cells[2].ForeColor = Color.Brown;
            }
}

    #region Grid operations
    protected void gridBCCAgent_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int position = 3;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");


            if (BCCUIHelper.Constants.STATUS_STOPPED.Equals(e.Row.Cells[position].Text))
            {
                e.Row.Cells[position].ForeColor = Color.Red;
            }
            else
                if (BCCUIHelper.Constants.STATUS_RUNNING.Equals(e.Row.Cells[position].Text))
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
        foreach (GridViewRow row in gridBCCAgent.Rows)
        {
            CheckBox cb = (CheckBox)row.FindControl("chkBoxService");
            if (cb != null)
            {
                cb.Checked = true;
            }
        }
    }

    protected void btnDeSelectAll_Click(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gridBCCAgent.Rows)
        {
            CheckBox cb = (CheckBox)row.FindControl("chkBoxService");
            if (cb != null)
            {
                cb.Checked = false;
            }
        }
    }

    protected void btnStart_Click(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gridBCCAgent.Rows)
        {
            CheckBox cb = (CheckBox)row.FindControl("chkBoxService");

            if (cb != null && cb.Checked)
            {
                try
                {
                    dataAccess.StartService(row.Cells[1].Text);
                    new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "started " + row.Cells[1].Text, 604);
                    PopulateViewServiceDetails();
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                }
            }
        }
    }

    protected void btnStop_Click(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gridBCCAgent.Rows)
        {
            CheckBox cb = (CheckBox)row.FindControl("chkBoxService");

            if (cb != null && cb.Checked)
            {
                try
                {
                    dataAccess.StopService(row.Cells[1].Text);
                    new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "stopped " + row.Cells[1].Text, 604);
                    PopulateViewServiceDetails();
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                }
            }
        }
    }
    #endregion
}
