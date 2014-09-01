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

public partial class BTSearch : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCOperator hubOperator = new BCCOperator();
    BCCUIHelper uiHelper = new BCCUIHelper();
    public string searchTerms = "";
    public int count = 0;
    SelectionBar searchList = null;

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
            if (!Page.IsPostBack)
            {
                GrabQueryString();
            }

            InitializeSearchHistory();
            InitializeObjects();
            btnFilter_Click(null, null);
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message + ex.StackTrace);
            gvSearch.Visible = false;
        }
    }

    protected void gvSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        if (gvSearch.Rows.Count > 0)
        {
            // Set CurrentPageIndex to the page the user clicked.
            gvSearch.PageIndex = e.NewPageIndex;
            gvSearch.DataBind();
        }
    }

    private void InitializeSearchHistory()
    {
        bool exceptionFlag = false;

        if (Application["SEARCH_HISTORY"] != null)
        {
            searchList = Application["SEARCH_HISTORY"] as SelectionBar;
        }
        else
        {
            // Initialize 
            searchList = new SelectionBar(10, "SBA.aspx");

            // Get the list from the Profile system.
            StringCollection searchHistory = null;

            try
            {
                if (Profile.ControlCenterProfile.IsProfileActive)
                {
                    BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];

                    if (props != null)
                    {
                        searchHistory = props.ModuleDictionary[BCCUIHelper.Constants.SEARCH_HISTORY_KEY];
                        
                        foreach (string searchKeyword in searchHistory)
                        {
                            searchList.Add(searchKeyword);
                        }
                    }
                    else
                    {
                        exceptionFlag = true;
                    }
                }
            }
            catch
            {
                exceptionFlag = true;
            }

            if (exceptionFlag || !(searchList.Count > 0))
            {
                searchList.Add("bound");
                searchList.Add("assembly:");
            }

            Application["SEARCH_HISTORY"] = searchList;
        }

        if (searchList != null)
        {
            searchTerms = searchList.ToSearchTerms();
        }
        else
        {
            searchTerms = "disabled";
        }
    }

    private void InitializeObjects()
    {
        lblError.Visible = false;
        txtSearchKey.Focus();
        UserActivityGrid.Visible = true;
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";
    }

    private void GrabQueryString()
    {
	    string searchTerm = Request.QueryString["searchTerm"] as String;

        if (searchTerm != null && searchTerm.Length > 0)
        {
            txtSearchKey.Text = searchTerm;
        }
    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = false;
        errorImg.Visible = true;
        //lblError.ForeColor = Color.Black;
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        HtmlForm myMasterForm = (HtmlForm)Master.FindControl("form1");

        if (myMasterForm != null)
        {
            myMasterForm.Controls.Add(gvSearch);
        }

        BCCGridView.Export("ControlCenter_" + System.Environment.MachineName + "_Search.xls", this.gvSearch);
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
	    // DO NOT : remove this method. 
    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        DataTable dt = dataAccess.EnumerateAllArtifacts();
        StringCollection searchHistory = null;

        if (txtSearchKey.Text != null && txtSearchKey.Text.Length > 0)
        {
            dt.DefaultView.RowFilter = "Name LIKE '%" + txtSearchKey.Text + "%' OR Data LIKE '%" + txtSearchKey.Text + "%' OR Status LIKE '%" + txtSearchKey.Text + "%'";

            gvSearch.Visible = true;
            gvSearch.DataSource = dt;
            gvSearch.DataBind();
            count = gvSearch.Rows.Count;

            if (searchList != null)
            {
                if (count > 0) // There are some results for this search term.
                {
                    searchList.Add(txtSearchKey.Text);

                    #region Profile
                    //TODO: Check if Profile is 'Active', if yes add it to the list.
                    if (Profile.ControlCenterProfile.IsProfileActive)
                    {
                        BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];

                        if (props != null)
                        {
                            searchHistory = props.ModuleDictionary[BCCUIHelper.Constants.SEARCH_HISTORY_KEY];

                            if (searchHistory != null)
                            {
                                searchHistory.Add(txtSearchKey.Text);
                            }
                        }
                    }
                    #endregion 
                }
                searchTerms = searchList.ToSearchTerms();
            }

            new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "searched for '" + txtSearchKey.Text + "'", 402);
        }

        if (count <= 0 && txtSearchKey.Text.Length > 0)
        {
            DisplayError("No records were found for the search term '"  + txtSearchKey.Text + "', try another search.");
        }
    }

    protected void gvSearch_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (BCCUIHelper.Constants.STATUS_UNENLISTED.Equals(e.Row.Cells[1].Text))
            {
                e.Row.Cells[1].ForeColor = Color.Blue;
            }
            else
            if (BCCUIHelper.Constants.STATUS_ENLISTED.Equals(e.Row.Cells[1].Text))
            {
                e.Row.Cells[1].ForeColor = Color.Blue;
            }
            else
            if (BCCUIHelper.Constants.STATUS_BOUND.Equals(e.Row.Cells[1].Text))
            {
                e.Row.Cells[1].ForeColor = Color.Blue;
            }
            else
            if (BCCUIHelper.Constants.STATUS_STARTED.Equals(e.Row.Cells[1].Text))
            {
                e.Row.Cells[1].ForeColor = Color.Green;
            }
            else
            if (BCCUIHelper.Constants.STATUS_ENABLED.Equals(e.Row.Cells[1].Text))
            {
                e.Row.Cells[1].ForeColor = Color.Green;
            }
            else
            if (BCCUIHelper.Constants.STATUS_RUNNING.Equals(e.Row.Cells[1].Text))
            {
                e.Row.Cells[1].ForeColor = Color.Green;
            }
            else
            if (BCCUIHelper.Constants.STATUS_STOPPED.Equals(e.Row.Cells[1].Text))
            {
                e.Row.Cells[1].ForeColor = Color.Red;
            }
            else
            if (BCCUIHelper.Constants.STATUS_DISABLED.Equals(e.Row.Cells[1].Text))
            {
                e.Row.Cells[1].ForeColor = Color.Red;
            }
        }
    }
}
