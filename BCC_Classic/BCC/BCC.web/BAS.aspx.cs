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
using BCC.Core;
using System.Drawing;
using System.Collections.Specialized;

public partial class Schemas : System.Web.UI.Page
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
        InitializeObjects();

        if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN) || User.IsInRole(BCCUIHelper.Constants.ROLE_ARTIFACT))
        {
            PopulateGrid(string.Empty);
            new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "viewed", 205);
        }
        else
        {
            DisplayError(BCCUIHelper.Constants.ACCESS_DENIED);
        }
    }

    private void InitializeObjects()
    {
        gridSchemas.Visible = false;
        lblError.Visible = false;
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";
        this.search.SearchClick += new EventHandler(this.btnFilter_Click);
    }

    private void UpdateLabel(int count)
    {
        search.KeywordLabel = "(keywords: 'Property', 'BizTalk.System', 'Edi' ...) [" + count + "]";
    }

    private void PopulateGrid(string searchKey)
    {
        try
        {
            StringCollection applicationList = null;
            DataTable dt = null;

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
                dt = dataAccess.RetrieveAllSchemas(applicationList);

                // This is for the datalist view on the right side. 
                dlSAppList.DataSource = applicationList;
                dlSAppList.DataBind();
                dlSAppList.Visible = true;
            }
            else
            {
                dt = dataAccess.RetrieveAllSchemas();
            }

            if (searchKey != null && searchKey.Length > 0)
            {
                dt.DefaultView.RowFilter = "MessageType LIKE '%" + searchKey + "%' or SchemaName LIKE '%" + searchKey + "%' or SchemaType LIKE '%" + searchKey + "%' or Application LIKE '%" + searchKey + "%'";
            }

            if (dt.Columns.Count > 0)
            {
                dt.DefaultView.Sort = "SchemaName ASC";
            }

            gridSchemas.DataSource = dt;
            gridSchemas.DataBind();
            gridSchemas.Visible = true;

            int count = gridSchemas.Rows.Count;

            if (count == 0)
            {
                schemaPanel.Visible = false;
                schemaPanel.Enabled = false;
                // Enable empty panel
                emptyPanel.Visible = true;
            }

            UpdateLabel(count);
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            gridSchemas.Visible = false;
        }
    }
    
    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        errorImg.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = false;
    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        SearchUserControl search = sender as SearchUserControl;
        PopulateGrid(search.SearchKeyword);
    }

    protected void gridSchemas_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        gridSchemas.PageIndex = e.NewPageIndex;
        //dt.DefaultView.Sort = sortExpression + " " + lastDirection;
        gridSchemas.DataBind();
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        HtmlForm myMasterForm = (HtmlForm)Master.FindControl("form1");

        if (myMasterForm != null)
        {
            myMasterForm.Controls.Add(gridSchemas);
        }

        BCCGridView.Export("ControlCenter_" + System.Environment.MachineName + "_Schemas.xls", this.gridSchemas);
    }
}
