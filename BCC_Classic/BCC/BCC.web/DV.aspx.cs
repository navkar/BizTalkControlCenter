using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using BCC.Core;

public partial class DeploymentVerification : System.Web.UI.Page
{
    //BCCUIHelper uiHelper = new BCCUIHelper();
   
    public string identifier = "\u25BA";
    public string messageID = "0";
    private String m_strSortExp;
    private SortDirection m_SortDirection = SortDirection.Ascending;

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

        if (!Page.IsPostBack) // Not a postback
        {
            lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
            subCaption.Text = "Steps to verify?";

            PopulateGridOrchestration();
            PopulateGACGrid(txtGACSearchKey.Text);
            ActivateGrid();

            new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "verified GAC installation", 504);
        }
        else
        {
            if (null != ViewState["_SortExp_"])
            {
                m_strSortExp = ViewState["_SortExp_"] as String;
            }
            if (null != ViewState["_Direction_"])
            {
                m_SortDirection = (SortDirection)ViewState["_Direction_"];
            }
        }
    }

    private void InitializeObjects()
    {
        lblError.Visible = false;
        errorImg.Visible = false;

        if (!Page.IsPostBack)
        {
            FillSearchTermFromProfile();
        }

        txtGACSearchKey.Focus();
    }

    private void ActivateGrid()
    {
        UserActivityGrid.Visible = true;
    }

    private void PopulateGridOrchestration()
    {
        try
        {
            StringCollection applicationList = null;
            DataTable dt = null;
            BCCDataAccess dataAccess = new BCCDataAccess();

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
                dlAppList.DataSource = applicationList;
                dlAppList.DataBind();
                dlAppList.Visible = true;
            }
            else
            {
                dt = dataAccess.RetrieveAllOrchestrations();
            }


            if (dt.Rows.Count > 0)
            {
                dt.DefaultView.Sort = "AssemblyName" + " ASC";
            }
            else
            {
                DataRow blankRow = dt.NewRow();
                dt.Rows.Add(blankRow);
            }

            gridOdx.DataSource = dt;
            gridOdx.DataBind();
            gridOdx.Visible = true;
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
        }
    }

    private void FillSearchTermFromProfile()
    {
        string searchTerm = string.Empty;

        // Check whether the Profile is active and then proceed.
        if (Profile.ControlCenterProfile.IsProfileActive)
        {
            try
            {
                BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                searchTerm = props.ModuleKeys[BCCUIHelper.Constants.SEARCH_TERM_KEY] as string;
            }
            catch
            {
                // Ignore errors from Profile system
            }
        }

        if (searchTerm != string.Empty)
        {
            txtGACSearchKey.Text = searchTerm;
        }
    }

    private void PopulateGACGrid(string searchTerm)
    {
        try
        {
            if (searchTerm.Length > 0)
            {
                DataTable dt = BuildGACData(searchTerm);

                if (dt.Rows.Count > 0)
                {
                    dt.DefaultView.Sort = "DateModified DESC";
                }
                
                gridGAC.DataSource = dt;
                gridGAC.DataBind();
                gridGAC.Visible = true;
            }

        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            gridGAC.Visible = false;
        }
    }

    private DataTable BuildGACData(string searchTerm)
    {
        System.Data.DataTable dt = null;

        try
        {
            DataColumn dcGAC = new DataColumn("GACAssemblies", typeof(System.String));
            DataColumn dcVersion = new DataColumn("Version", typeof(System.String));
            DataColumn dcFlag = new DataColumn("Flag", typeof(System.String));
            DataColumn dcDate = new DataColumn("DateModified", typeof(System.DateTime));

            dt = new System.Data.DataTable();
            dt.Columns.Add(dcFlag);
            dt.Columns.Add(dcGAC);
            dt.Columns.Add(dcVersion);
            dt.Columns.Add(dcDate);

            string path = Path.Combine(@"c:\windows\assembly", "GAC_MSIL");
            string fileName = "";
            string version = "";
            int counter = 0;
            FileInfo fileInfo = null;
            string[] filePath = null;

            if (Directory.Exists(path))
            {
                string[] assemblyFolders = Directory.GetDirectories(path);
                
                foreach (string assemblyFolder in assemblyFolders)
                {
                    int index = assemblyFolder.IndexOf(searchTerm);
                    counter = 0; // reset

                    if (index != -1)
                    {
                        int endPoint = assemblyFolder.LastIndexOf("\\");
                        
                        if (endPoint != -1)
                        {
                            fileName = assemblyFolder.Substring(endPoint + 1);
                        }

                        string[] assemblySubFolders = Directory.GetDirectories(assemblyFolder);

                        foreach (string assemblySubFolder in assemblySubFolders)
                        {
                            counter++;

                            int vIndex = assemblySubFolder.IndexOf("__");

                            if (vIndex != -1)
                            {
                                version = assemblySubFolder.Substring(vIndex - 7, 7);
                            }

                            filePath = Directory.GetFiles(assemblySubFolder);
                            fileInfo = new FileInfo(filePath[0]);

                            if (counter > 1)
                            {
                                dt.Rows.Add(identifier, fileName, version, fileInfo.LastWriteTime);
                            }
                            else
                            {
                                dt.Rows.Add(string.Empty, fileName, version, fileInfo.LastWriteTime);
                            }

                        }

                        if (dt.Rows.Count >= 20)
                        {
                            break;
                        }
                   }
                }
            }
            else
            {
                DisplayError(path + " path not found.");
            }

            if (dt.Rows.Count > 20)
            {
                DisplayError("The search term <b>'" + searchTerm + "'</b> resulted in too many results, refine your search.");
            }

        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
        }

        return dt;
    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        errorImg.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = true;
    }

    protected void btnGACFilter_Click(object sender, EventArgs e)
    {
        PopulateGridOrchestration();
        PopulateGACGrid(txtGACSearchKey.Text);
    }

    protected void btnShowOdx_Click(object sender, EventArgs e)
    {
        orchestrationPanel.Visible = true;
        txtGACSearchKey.Focus();
    }
    
    protected void gridOdx_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells.Count > 1)
            {
                if (e.Row.Cells[1].Text.Contains("1.0.0.0"))
                {
                    e.Row.Cells[0].ForeColor = Color.Teal;
                    e.Row.Cells[1].ForeColor = Color.Teal;
                }
            }
        }
    }

    protected void gridGAC_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells.Count > 3)
            {
                // Last modified on is in Column - 3, index starts at 0.
                int compare = System.DateTime.Compare(DateTime.Parse(e.Row.Cells[2].Text), System.DateTime.Today);

                if (compare > 0)
                {
                    System.Web.UI.WebControls.Image tickImage = new System.Web.UI.WebControls.Image();
                    tickImage.ImageUrl = "~/Images/tick-circle-frame-icon.png";
                    tickImage.ToolTip = "This DLL was installed into GAC today.";

                    e.Row.Cells[0].Controls.Add(tickImage);
                    e.Row.Cells[1].ForeColor = Color.Teal;
                    e.Row.Cells[2].ForeColor = Color.Teal;
                    e.Row.Cells[3].ForeColor = Color.Teal;
                }
            }

            if (e.Row.Cells.Count > 0)
            {
                if (e.Row.Cells[0].Text.Contains(identifier))
                {
                    e.Row.Cells[0].ForeColor = Color.Red;
                    e.Row.Cells[1].ForeColor = Color.Red;
                    e.Row.Cells[2].ForeColor = Color.Red;
                    e.Row.Cells[3].ForeColor = Color.Red;
                }
            }
        }
    }

    protected void OnSort(object sender, GridViewSortEventArgs e)
    {
        // There seems to be a bug in GridView sorting implementation. Value of
        // SortDirection is always set to "Ascending". Now we will have to play
        // little trick here to switch the direction ourselves.
        if (String.Empty != m_strSortExp)
        {
            if (String.Compare(e.SortExpression, m_strSortExp, true) == 0)
            {
                m_SortDirection =
                    (m_SortDirection == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
            }
        }
        ViewState["_Direction_"] = m_SortDirection;
        ViewState["_SortExp_"] = m_strSortExp = e.SortExpression;

        BindGridView(txtGACSearchKey.Text);
        PopulateGridOrchestration();
    }

    private void BindGridView(string searchTerm)
    {
        String strSort = String.Empty;

        if (null != m_strSortExp && String.Empty != m_strSortExp)
        {
            strSort = String.Format("{0} {1}", m_strSortExp, (m_SortDirection == SortDirection.Descending) ? "DESC" : "ASC");
        }

        DataTable dt = BuildGACData(searchTerm);
        DataView dv = new DataView(dt, String.Empty, strSort, DataViewRowState.CurrentRows);
        gridGAC.DataSource = dv;
        gridGAC.DataBind();
    }

    protected void btnExportToExcel2_Click(object sender, EventArgs e)
    {
        HtmlForm myMasterForm = (HtmlForm)Master.FindControl("form1");

        if (myMasterForm != null)
        {
            myMasterForm.Controls.Add(gridGAC);
        }

        BCCGridView.Export("ControlCenter_" + System.Environment.MachineName + "_GACList.xls", this.gridGAC);
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        // DO NOT : remove this method. 
    }
}
