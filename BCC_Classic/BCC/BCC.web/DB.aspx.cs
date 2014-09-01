using System;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using BCC.Core;
using System.Collections.Specialized;

public partial class DeploymentBindings : System.Web.UI.Page
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
            InitializeObjects();

            if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN) || User.IsInRole(BCCUIHelper.Constants.ROLE_DEPLOY))
            {
                if (!Page.IsPostBack)
                {
                    ActivateGrid();
                }
                InitializeApplicationTabs();
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

    private void InitializeObjects()
    {
        bindingFileLink.Visible = false;
        bindingFileLink.Enabled = false;
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";
    }

    private void InitializeApplicationTabs()
    {
        StringCollection appCollection = null;
        // Use filters here.
        // Check whether the Profile is active and then proceed.
        if (Profile.ControlCenterProfile.IsProfileActive)
        {
            try
            {
                BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                appCollection = props.ModuleDictionary[BCCUIHelper.Constants.APP_LIST_KEY];
            }
            catch
            {
                appCollection = dataAccess.RetrieveAllApplications();
            }
        }
        else
        {
            appCollection = dataAccess.RetrieveAllApplications();
        }

        if (appCollection != null)
        {
            LinkButton linkButton = null;
            System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
            img.ImageUrl = @"~\Images\bullet.png";
            img.Visible = true;
            linkButtonGroup.Controls.Add(new LiteralControl("Applications "));
            linkButtonGroup.Controls.Add(img);

            foreach (string applicationName in appCollection)
            {
                linkButton = new LinkButton();
                linkButton.ID = applicationName;
                linkButton.Text = applicationName;
                linkButton.CssClass = "linkConfig";
                linkButton.Click += new EventHandler(linkButton_Click);
                linkButton.ToolTip = "Select " + applicationName;
                linkButtonGroup.Controls.Add(linkButton);
                linkButtonGroup.Controls.Add(new LiteralControl("|"));
            }
        }

        if (appCollection != null && appCollection.Count > 0)
        {
            dlApplication.DataSource = appCollection;
            dlApplication.DataBind();
            dlApplication.Visible = true;
        }
    }

    protected void linkButton_Click(object sender, EventArgs e)
    {
        LinkButton lnkButton = sender as LinkButton;
        
        //bindingsPanel.GroupingText = "BizTalk application '<b>" + lnkButton.Text + "</b>' on machine " + System.Environment.MachineName;
        bindingsPanel.Visible = true;

        lblSection.Text = "Export '<b>" + lnkButton.Text + "</b>' application MSI/bindings";
        lblSection2.Text = "Import '<b>" + lnkButton.Text + "</b>' application bindings";

        btnExportBindings.CommandArgument = lnkButton.Text;
        btnExportMSI.CommandArgument = lnkButton.Text;
        btnImportBinding.CommandArgument = lnkButton.Text;
        
        // Make everything visible
        lblSection.Visible = true;
        linkPanel.Visible = true;
        lblSection2.Visible = true;
        tBindings.Visible = true;
        btnImportBinding.Visible = true;
        bindingsPanel.Visible = true;
        ToggleLinks(true);
    }

    private void ActivateGrid()
    {
        UserActivityGrid.Visible = true;
        bindingsPanel.Visible = false;
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

    protected string FullFileName(string fileName)
    {
        return "DB-D.aspx?name=" + fileName.Replace("\\", "/");
    }

    private void ToggleLinks(bool flag)
    {
        btnExportMSI.Visible = flag;
        btnExportBindings.Visible = flag;
    }

    private void DownloadFile(string filePath)
    {
        try
        {
            FileInfo file = new FileInfo(filePath);

            if (file.Exists)
            {
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.ContentType = "application/octet-stream";
                Response.TransmitFile(file.FullName);
                Response.End();
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
            Response.End();
        }
    }

    protected void btnExportMSI_Click(object sender, EventArgs e)
    {
        LinkButton sourceBtn = sender as LinkButton;
        string applicationName = sourceBtn.CommandArgument;

        ToggleLinks(false);
        BCCOperator bccOperator = new BCCOperator();

        string bindingFilePath = @"C:\Windows\Temp";
        string bindingFileName = applicationName + "_" + System.Environment.MachineName + ".MSI";

        int returnCode = bccOperator.ExportMSIFile(applicationName,
                                            BCCOperator.BizTalkSQLServer(),
                                            BCCOperator.BizTalkMgmtDb(),
                                            bindingFilePath,
                                            bindingFileName);

        if (returnCode != 0)
        {
            DisplayError("Unable to generate a MSI file for the application '" + applicationName + "'.");
        }
        else
        {
            bindingFileLink.Text = "Download MSI";
            bindingFileLink.Visible = true;
            bindingFileLink.Enabled = true;
            bindingFileLink.NavigateUrl = FullFileName(bindingFilePath + "\\" + bindingFileName);
        }

        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "exported " + bindingFileName, 501);
    }


    protected void btnImportBindings_Click(object sender, EventArgs e)
    {
        if (tBindings.Text != string.Empty)
        {
            BCCOperator bccOperator = new BCCOperator();
            LinkButton sourceBtn = sender as LinkButton;
            string applicationName = sourceBtn.CommandArgument;

            string bindingFilePath = @"C:\Windows\Temp";
            string bindingFileName = string.Format("{0}_{1}_{2}_Bindings.xml", applicationName, System.Guid.NewGuid(), System.Environment.MachineName);

            // Read from textArea ----- tBindings.Text
            bccOperator.GenerateBindingFile(tBindings.Text, bindingFilePath + "\\" + bindingFileName);

            int returnCode = bccOperator.ImportBindingFile(applicationName,
                                                BCCOperator.BizTalkSQLServer(),
                                                BCCOperator.BizTalkMgmtDb(),
                                                bindingFilePath,
                                                bindingFileName);

            if (returnCode != 0)
            {
                DisplayError("Unable to import binding file for the application '" + applicationName + "'.");
            }
            else
            {
                new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "imported " + bindingFileName, 501);
                DisplayOK("Bindings were successfully imported into " + applicationName);
            }
        }
        else
        {
            DisplayError("Ensure that the binding file is not empty!");
        }
    }

    protected void btnExportBindings_Click(object sender, EventArgs e)
    {
        LinkButton sourceBtn = sender as LinkButton;
        string applicationName = sourceBtn.CommandArgument;

        ToggleLinks(false);
        BCCOperator bccOperator = new BCCOperator();

        string bindingFilePath = @"C:\Windows\Temp";
        string bindingFileName = applicationName + "_" + System.Environment.MachineName + "_" + "Bindings.XML";

        int returnCode = bccOperator.ExportBindingFile(applicationName,
                                            BCCOperator.BizTalkSQLServer(),
                                            BCCOperator.BizTalkMgmtDb(),
                                            bindingFilePath,
                                            bindingFileName);

        if (returnCode != 0)
        {
            DisplayError("Unable to generate a binding file for the application '" + applicationName + "'.");
        }
        else
        {
            bindingFileLink.Text = "Download bindings";
            bindingFileLink.Visible = true;
            bindingFileLink.Enabled = true;
            bindingFileLink.NavigateUrl = FullFileName(bindingFilePath + "\\" + bindingFileName);
        }

        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "exported " + bindingFileName, 501);

    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        // DO NOT : remove this method. 
    }
}
