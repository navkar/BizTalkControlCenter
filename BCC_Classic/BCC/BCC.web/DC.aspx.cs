using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using BCC.Core;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.Schema;
using System.Drawing;

public partial class DeploymentConfiguration : System.Web.UI.Page
{
    public string configCategory = string.Empty;
    BCCDataAccess dataAccess = new BCCDataAccess();

    protected void Page_PreInit(object sender, EventArgs e)
    {
        string theme = Session["PAGE_THEME"] as string;

        if (theme != null)
        {
            this.Page.Theme = theme;
        }
    }

    private void InitializeObjects()
    {
        lblError.Visible = false;
        errorImg.Visible = false;
        okImg.Visible = false;
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN) || User.IsInRole(BCCUIHelper.Constants.ROLE_DEPLOY))
            {
                if (Page.IsPostBack)
                {
                    configCategory = lblXmlConfig.Text;
                    upTextArea.Visible = true;
                }

                InitializeObjects();
                InitializeConfigurationTabs();
                InitializeAdapterGrid();
            }
            else
            {
                DisplayError(BCCUIHelper.Constants.ACCESS_DENIED);
            }
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
        }
    }

    private void InitializeAdapterGrid()
    {
        DataTable dt = dataAccess.RetrieveAllAdapters();
        dt.DefaultView.Sort = "ProtocolName ASC";
        adapterList.DataSource = dt;
        adapterList.DataBind();
        adapterList.Visible = true;
    }

    private void InitializeConfigurationTabs()
    {
        StringCollection configOperations = null;
        string ssoConnectionString = string.Empty;
        // Use filters here.
        // Check whether the Profile is active and then proceed.
        if (Profile.ControlCenterProfile.IsProfileActive)
        {
            try
            {
                BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                configOperations = props.ModuleDictionary[BCCUIHelper.Constants.S502_CONFIG_OPERATIONS_KEY];
            }
            catch
            {
                // Ignore errors from Profile system
            }
        }

        if (configOperations != null)
        {
            LinkButton linkButton = null;

            foreach (string configName in configOperations)
            {
                linkButton = new LinkButton();
                linkButton.ID = configName;
                linkButton.Text = configName;
                linkButton.CssClass = "linkConfig";
                linkButton.Click += new EventHandler(linkButton_Click);
                linkButton.ToolTip = "Create " + configName;
                linkButtonGroup.Controls.Add(linkButton);
                linkButtonGroup.Controls.Add(new LiteralControl("&nbsp;"));
            }
        }

    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = false;
        errorImg.Visible = true;
    }

    protected void DisplayOK(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = false;
        okImg.Visible = true;
        errorImg.Visible = false;
    }

    private void LoadConfigPanel(string searchKey)
    {
        BCCModuleProperty props = null;

        if (searchKey != null && searchKey.Length > 0)
        {
            props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];

            if (props != null && props.ModuleDictionary.ContainsKey(searchKey))
            {
                configXmlText.Text = props.ModuleDictionary[searchKey][0];
                configXmlText.Focus();
                pXmlConfig.Visible = true;
                lblXmlConfig.Text = searchKey;
            }
            else
            {
                DisplayError("Configuration with the key '" + searchKey + "' was not found. Try another key.");
            }
        }
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        // DO NOT : remove this method. 
    }

    protected void linkButton_Click(object sender, EventArgs e)
    {
        LinkButton linkBtn = sender as LinkButton;
        LoadConfigPanel(linkBtn.Text);
        upTextArea.Visible = true;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            string configXmlData = configXmlText.Text;
            BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];

            if (props != null && configXmlData.Length > 0)
            {
                if (configCategory.Equals(BCCUIHelper.Constants.S502_HOST_CONFIG_KEY))
                {
                    props.ModuleDictionary[BCCUIHelper.Constants.S502_HOST_CONFIG_KEY][0] = configXmlData;
                    Profile.Save();

                    new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "saved " + BCCUIHelper.Constants.S502_HOST_CONFIG_KEY, 502);
                }
                else if (configCategory.Equals(BCCUIHelper.Constants.S502_SSO_CONFIG_KEY))
                {
                    props.ModuleDictionary[BCCUIHelper.Constants.S502_SSO_CONFIG_KEY][0] = configXmlData;
                    Profile.Save();

                    new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "saved " + BCCUIHelper.Constants.S502_SSO_CONFIG_KEY, 502);
                }
            }

        }
        catch (Exception exception)
        {
            new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "saved " + BCCUIHelper.Constants.S502_HOST_CONFIG_KEY + " with error " + exception.Message, 502);
            DisplayError(exception.Message);
        }
    }

    protected void btnTemplate_Click(object sender, EventArgs e)
    {
        switch (configCategory)
        {
            case BCCUIHelper.Constants.S502_HOST_CONFIG_KEY:
                configXmlText.Text = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/" + BCCUIHelper.Constants.S502_HOST_CONFIG_KEY + "Template.xml"));
                break;
            case BCCUIHelper.Constants.S502_SSO_CONFIG_KEY:
                configXmlText.Text = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/" + BCCUIHelper.Constants.S502_SSO_CONFIG_KEY + "Template.xml"));
                break;
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            // Goals - Save the most recent configuration information
            // Provide status and exception updates
            string configXmlData = configXmlText.Text;

            if (configXmlData != string.Empty)
            {
                ProcessXMLConfigData(configXmlData, configCategory);
            }

            DisplayOK(configCategory + " has been applied successfully.");
        }
        catch (Exception ex)
        {
            new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "applied " + BCCUIHelper.Constants.S502_HOST_CONFIG_KEY + " with error " + ex.Message, 502);
            DisplayError(ex.Message);
        }

    }

    private void ValidationCallBack(object sender, ValidationEventArgs args)
    {
        if (args.Severity == XmlSeverityType.Warning)
        {
            DisplayError("Warning: Matching schema not found.  No validation occurred." + args.Message);
        }
        else if (args.Severity == XmlSeverityType.Error)
        {
            DisplayError("Validation error: " + args.Message);
        }
    }

    private void ProcessXMLConfigData(string configData, string category)
    {
        XmlDocument configXml = null;
        BCCModuleProperty props = null;

        if (configData != string.Empty)
        {
            props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];

            configXml = new XmlDocument();
            configXml.LoadXml(configData);

            //configXml.Schemas.

            //ValidationEventHandler handler = new ValidationEventHandler(ValidationCallBack);
            //configXml.Validate(handler);               

            if (category.Equals(BCCUIHelper.Constants.S502_HOST_CONFIG_KEY))
            {
                // First save user data and then proceed.
                if (props != null)
                {
                    props.ModuleDictionary[BCCUIHelper.Constants.S502_HOST_CONFIG_KEY][0] = configData;
                    Profile.Save();
                }

                BCCWMIHelpers wmi = new BCCWMIHelpers();
                wmi.CreateHosts(configXml);

                //TODO: Dont forget to update user profile information

                new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "applied " + BCCUIHelper.Constants.S502_HOST_CONFIG_KEY, 502);
            }
            else if (category.Equals(BCCUIHelper.Constants.S502_SSO_CONFIG_KEY))
            {
                // First save user data and then proceed.
                if (props != null)
                {
                    props.ModuleDictionary[BCCUIHelper.Constants.S502_SSO_CONFIG_KEY][0] = configData;
                    Profile.Save();
                }

                string applicationName = SSOConfigHelper.LoadSSOConfigXml(configXml);

                // Dont forget to update user profile information
                BCCModuleProperty prop211 = Profile.ControlCenterProfile.ModuleFilter["211"];
                if (prop211 != null)
                {
                    StringCollection appList = prop211.ModuleDictionary[BCCUIHelper.Constants.APP_LIST_KEY];

                    if (appList != null)
                    {
                        appList.Add(applicationName);
                        Profile.Save();
                    }
                }

                new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "applied " + BCCUIHelper.Constants.S502_SSO_CONFIG_KEY, 502);
            }
        }
    }
}
