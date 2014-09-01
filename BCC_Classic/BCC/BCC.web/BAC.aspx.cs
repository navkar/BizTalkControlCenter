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
using System.Drawing;

public partial class ConfigFile : System.Web.UI.Page
{
    DataTable dt = null;
    string lastDirection = "DESC";
    string sortExpression = "DateModified";

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
            InitializeConfigurationTabs();
            ActivateGrid();
        }
        else
        {
            DisplayError(BCCUIHelper.Constants.ACCESS_DENIED);
        }
        
        if (Page.IsPostBack)
        {
            lastDirection = Session[SiteMap.CurrentNode.Description + "SortDirection"] as string;
            sortExpression = Session[SiteMap.CurrentNode.Description + "SortExpression"] as string;

            string searchPattern = Session[SiteMap.CurrentNode.Description + "__ID"] as string;

            if (searchPattern != null)
            {
                BuildDataGrid(searchPattern);
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
        lblCaptionToday.Text = DateTime.UtcNow.ToString("MMM-dd-yy hh:mm:ss tt");
        lblCaptionToday.BackColor = Color.NavajoWhite;
    }

    private void InitializeConfigurationTabs()
    {
        StringCollection fileExtnList = null;
        StringCollection configDirList = null;
        // Use filters here.
        // Check whether the Profile is active and then proceed.
        if (Profile.ControlCenterProfile.IsProfileActive)
        {
            try
            {
                BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                fileExtnList = props.ModuleDictionary[BCCUIHelper.Constants.FILE_EXTN_LIST];
                configDirList = props.ModuleDictionary[BCCUIHelper.Constants.CONFIG_DIRECTORY_LIST];
            }
            catch
            {
                // Ignore errors from Profile system
            }
        }

        if (fileExtnList != null)
        {
            LinkButton linkButton = null;
            linkButtonGroup.Controls.Add(new LiteralControl("Select pattern "));
            linkButtonGroup.Controls.Add(BuildImage("bullet.png", "Choose a file pattern"));

            foreach (string fileExtnPattern in fileExtnList)
            {
                linkButton = new LinkButton();
                linkButton.ID = fileExtnPattern;
                linkButton.Text = fileExtnPattern;
                linkButton.CssClass = "linkConfig";
                linkButton.Click += new EventHandler(linkButton_Click);
                linkButton.ToolTip = "View " + fileExtnPattern;
                linkButtonGroup.Controls.Add(linkButton);
                linkButtonGroup.Controls.Add(new LiteralControl("|"));
            }
        }

        if (configDirList != null && configDirList.Count > 0)
        {
            dlFileDirList.DataSource = configDirList;
            dlFileDirList.DataBind();
            dlFileDirList.Visible = true;
        }

    }

    private System.Web.UI.WebControls.Image BuildImage(string fileName, string toolTip)
    {
        System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
        img.ImageUrl = @"~\Images\" + fileName;
        img.Visible = true;
        img.ToolTip = toolTip;
        return img;
    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = false;
        errorImg.Visible = true;
    }

    private void InitializeObjects()
    {
        lblError.Visible = false;
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";
    }

    private void BuildDataGrid(string searchPattern)
    {
        configFileListPanel.GroupingText = "Pattern: " + searchPattern;

        dt = new DataTable();

        // define the table's schema
        dt.Columns.Add(new DataColumn("DirectoryName", typeof(string)));
        dt.Columns.Add(new DataColumn("Name", typeof(string)));
        dt.Columns.Add(new DataColumn("CreationTime", typeof(DateTime)));
        dt.Columns.Add(new DataColumn("DateModified", typeof(DateTime)));
        dt.Columns.Add(new DataColumn("FullName", typeof(string)));

        try
        {
            DirectoryInfo di;
            FileInfo[] files;
            StringCollection configDirList = null;
            // Check whether the Profile is active and then proceed.
            if (Profile.ControlCenterProfile.IsProfileActive)
            {
                try
                {
                    BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                    configDirList = props.ModuleDictionary[BCCUIHelper.Constants.CONFIG_DIRECTORY_LIST];
                }
                catch
                {
                    // Ignore errors from Profile system
                }
            }

            if (configDirList != null && configDirList.Count > 0)
            {
                foreach (string configDirectory in configDirList)
                {
                    di = new DirectoryInfo(configDirectory);
                    files = di.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);

                    DataRow dr = null;

                    foreach (FileInfo fileInfo in files)
                    {
                        // create the rows
                        dr = dt.NewRow();
                        dr[0] = fileInfo.DirectoryName;
                        dr[1] = fileInfo.Name;
                        dr[2] = fileInfo.CreationTime;
                        dr[3] = fileInfo.LastWriteTime;
                        dr[4] = fileInfo.FullName;
                        dt.Rows.Add(dr);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, ex.Message, 210);
        }

        if (dt.Rows.Count > 0)
        {
            dt.DefaultView.Sort = sortExpression + " " + lastDirection;
            configGrid.DataSource = dt;
            configGrid.DataBind();
            configGrid.Visible = true;
            configFileListPanel.Visible = true;
        }
    }

    protected void linkButton_Click(object sender, EventArgs e)
    {
        LinkButton lnkButton = sender as LinkButton;

        if (lnkButton != null)
        {
            BuildDataGrid(lnkButton.Text);
            Session[SiteMap.CurrentNode.Description + "__ID"] = lnkButton.Text;
        }
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        HtmlForm myMasterForm = (HtmlForm)Master.FindControl("form1");

        if (myMasterForm != null)
        {
            myMasterForm.Controls.Add(configGrid);
        }

        BCCGridView.Export("ControlCenter_" + System.Environment.MachineName + "_Configs.xls", this.configGrid);
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        // DO NOT : remove this method. 
    }

    protected void configGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");

            Label lblCreatedOn = e.Row.Cells[3].FindControl("lblCreatedOn") as Label;
            Label lblModifiedOn = e.Row.Cells[4].FindControl("lblModifiedOn") as Label;

            if (lblCreatedOn != null && lblModifiedOn != null)
            {
                // Created On Today
                int createdOnToday = System.DateTime.Compare(DateTime.Parse(lblCreatedOn.Text), System.DateTime.Today);
                // Modified On Today
                int modifedOnToday = System.DateTime.Compare(DateTime.Parse(lblModifiedOn.Text), System.DateTime.Today);

                if (createdOnToday > 0)
                {
                    lblCreatedOn.BackColor = Color.NavajoWhite;
                }

                if (modifedOnToday > 0)
                {
                    lblModifiedOn.BackColor = Color.NavajoWhite;
                }
            }

        }
    }

    protected void configGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        configGrid.PageIndex = e.NewPageIndex;
        dt.DefaultView.Sort = sortExpression + " " + lastDirection;
        configGrid.DataBind();
    }

    protected void configGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        String sortDirection = "ASC";
        sortExpression = e.SortExpression;

        // If the last state is Ascending sort Descending
        if ((lastDirection != null) && (lastDirection.Equals("ASC")))
        {
            sortDirection = "DESC";
        }
        else
        {
            sortDirection = "ASC";
        }

        Session[SiteMap.CurrentNode.Description + "SortDirection"] = sortDirection;
        Session[SiteMap.CurrentNode.Description + "SortExpression"] = sortExpression;
        dt.DefaultView.Sort = sortExpression + " " + sortDirection;
        configGrid.DataBind();
    }
}
