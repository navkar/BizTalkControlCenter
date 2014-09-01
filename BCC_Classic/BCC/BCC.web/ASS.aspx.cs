using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

using BCC.Core;
using BCC.Controls;
using System.Collections.Generic;

public partial class SystemSettings : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCUIHelper uiHelper = new BCCUIHelper();

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
        try
        {
            lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
            subCaption.Text = "Information";

            if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN))
            {
                if (!Page.IsPostBack)
                {
                    ActivateGrid();
                    DisplayProfile();
                    BuildSpeedCodeList();
                }
            }
            else
            {
                DisplayError(BCCUIHelper.Constants.ACCESS_DENIED);
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
        themeDiv.Visible = true;
        this.Profile.ControlCenterProfile.IsProfileActive = true;
    }

    private void DisplayProfile()
    {
        DataTable dt = BCCProfileHelper.RetrieveUserProfile(Profile.ControlCenterProfile);

        gridUserProfile.DataSource = dt;
        gridUserProfile.DataBind();
        gridUserProfile.Visible = true;
    }

    private void BuildSpeedCodeList()
    {
        SiteMapNode root = SiteMap.RootNode;
        SiteMapNodeCollection collection = root.GetAllNodes();
        string naviUrl = string.Empty;

        foreach (SiteMapNode node in collection)
        {
            if (node.Description != string.Empty)
            {
                ddlModule.Items.Add(new ListItem("[" + node.Description + "] " + node.ParentNode.Title + " > " + node.Title, node.Description));
            }
        }
    }

    protected void DisplayError(string message)
    {
        lblStatus.Text = message;
        lblStatus.Font.Bold = false;
        
        lblStatus.Visible = true;
        errorImg.Visible = true;
        okImg.Visible = false;
    }

    public string UTCNow
    {
        get
        {
            return DateTime.UtcNow.ToShortTimeString();
        }
    }


    protected void DisplayInformation(string message)
    {
        lblStatus.Text = message;
        lblStatus.Font.Bold = true;
	    lblStatus.ForeColor = Color.Teal;
        lblStatus.Visible = true;
        okImg.Visible = true;
        errorImg.Visible = false;
    }

    protected void btnMood_Click(object sender, EventArgs e)
    {
        LinkButton source = sender as LinkButton;

        if (source != null)
        {
            Profile.ControlCenterProfile.UserTheme = this.Page.Theme;
            Profile.ControlCenterProfile.LastUpdated = System.DateTime.Now;
            new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "changed theme to '" + source.Text.Trim() + "'", 603);
        }

        if (source != null && source.ID == "btnMood")
        {
            lblTheme.Text = "^^^^^";
            lblTheme.Font.Bold = true;
            lblTheme.Visible = true;
            lblTheme2.Visible = false;
            lblTheme3.Visible = false;
        }
        else if (source != null && source.ID == "btnMood2")
        {
            lblTheme2.Text = "^^^^^";
            lblTheme2.Font.Bold = true;
            lblTheme2.Visible = true;
            lblTheme.Visible = false;
            lblTheme3.Visible = false;
        }
        else if (source != null && source.ID == "btnMood3")
        {
            lblTheme3.Text = "^^^^^";
            lblTheme3.Font.Bold = true;
            lblTheme3.Visible = true;
            lblTheme.Visible = false;
            lblTheme2.Visible = false;
        }
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

    protected void ddlModule_SelectedIndexChanged(object sender, EventArgs e)
    {
        string speedcode = ddlModule.SelectedValue;
        BCCModuleProperty props = null;

        if (!(speedcode.Equals("000")))
        {
            try
            {
                props = Profile.ControlCenterProfile.ModuleFilter[speedcode];
            }
            catch
            {

            }
        }

        ddlKey.Items.Clear();
        ddlKey.Items.Add(new ListItem("Select a key", "000"));

        if (props != null)
        {
            Dictionary<string, StringCollection>.KeyCollection keyCollection = props.ModuleDictionary.Keys;

            foreach (string key in keyCollection)
            {
                ddlKey.Items.Add(new ListItem(key, key));
            }
        }
    }

    protected void btnViewConfig_Click(object sender, EventArgs e)
    {
        // Get the speedcode 
        string speedCode = ddlModule.SelectedValue;
        string key = ddlKey.SelectedValue;

        BindModuleConfiguration(speedCode, key);
    }

    protected void btnRefreshProfile_Click(object sender, EventArgs e)
    {
        UpdateProfile(this.Context.User.Identity.Name);
    }

    private void UpdateProfile(string userName)
    {
        string profilePath = MapPath("~/App_Data/DefaultProfile.xml");
        XmlDocument profileXmlDoc = new XmlDocument();
        profileXmlDoc.Load(profilePath);

        BCCProfile userProfile = BCCProfileHelper.CreateDefaultProfile(profileXmlDoc);

        Profile.ControlCenterProfile = userProfile;
        Profile.Save();
        DisplayInformation("Your profile was updated.");

        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, string.Format("refreshed user profile successfully.", userName), 603);
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
                    moduleConfiguration.KeyName = keyName;
                    moduleConfiguration.ControlDataSource = stringCollection;
                    moduleConfiguration.DataBind();
                    moduleConfiguration.Visible = true;
                }
            }
        }
        catch
        {
            moduleConfiguration.Visible = false;
        }
    }

    /// <summary>
    /// Event handler for the Datagrid custom control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void moduleConfiguration_CollectionViewEvent(object sender, CollectionViewEventArgs e)
    {
        string speedCode = ddlModule.SelectedValue;
        string keyName = e.KeyName;

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
                BindModuleConfiguration(speedCode, keyName);

                new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, string.Format("added items to key '{0}' for speedcode {1}", keyName, speedCode), 603);
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
                BindModuleConfiguration(speedCode, keyName);

                new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, string.Format("removed items from key '{0}' for speedcode {1}", keyName, speedCode), 603);
            }
        }
    }
}
