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


public partial class OrchDox : System.Web.UI.Page
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
    }

    private void UpdateLabel(int count)
    {

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

    private void BindGridView()
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
        }
        else
        {
            dt = dataAccess.RetrieveAllOrchestrations();
        }

        gridOdx.DataSource = dt;
        int count = gridOdx.Rows.Count;
        gridOdx.DataBind();
    }

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

            dt.DefaultView.Sort = "NAME ASC";
            gridOdx.DataSource = dt;
            gridOdx.DataBind();
            gridOdx.Visible = true;
            int count = gridOdx.Rows.Count;

            if (count == 0)
            {
                masterPanel.Visible = false;
                masterPanel.Enabled = false;

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

    protected void gridOdx_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");

            string orchestrationName = e.Row.Cells[0].Text;
            string assemblyName = e.Row.Cells[1].Text;
            LinkButton lnkODox = e.Row.Cells[4].FindControl("lnkODox") as LinkButton;

            if (orchestrationName != null && assemblyName != null && lnkODox != null)
            {
                lnkODox.Attributes.Add("onclick", "window.open('DSOV-R.aspx?ON=" + orchestrationName + "&AN=" + assemblyName + "','','height=800,width=1200');return false;");
            }
        }
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        // DO NOT : remove this method. 
    }

    private void ShowDetailPanel()
    {
        masterPanel.Visible = false;
    }

    private void ShowMasterPanel()
    {
        masterPanel.Visible = true;
    }

}
