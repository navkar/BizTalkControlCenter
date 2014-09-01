using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using BCC.Core;
using Microsoft.EnterpriseSingleSignOn.Interop;

public partial class SSOConfigView : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCOperator hubOperator = new BCCOperator();
    BCCUIHelper uiHelper = new BCCUIHelper();
    public string searchTerms = "";
    public string testData = "";
    public int count = 0; // This is a global variable
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

    private void InitializeObjects()
    {
        lblError.Visible = false;
        txtSearchKey.Focus();
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

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            GrabQueryString();
            InitializeObjects();

            if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN) || User.IsInRole(BCCUIHelper.Constants.ROLE_ARTIFACT))
            {
                InitializeSearchHistory();
                btnFilter_Click(null, null);
            }
            else
            {
                DisplayError(BCCUIHelper.Constants.ACCESS_DENIED);
            }
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            gridAppView.Visible = false;
        }
    }

    private void InitializeSearchHistory()
    {
        StringCollection ssoAppList = null;
        string ssoConnectionString = string.Empty;
        // Initialize - cannot take more than 10 values.
        searchList = new SelectionBar(9, "BASSO.aspx");

        // Use filters here.
        // Check whether the Profile is active and then proceed.
        if (Profile.ControlCenterProfile.IsProfileActive)
        {
            try
            {
                BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                ssoAppList = props.ModuleDictionary[BCCUIHelper.Constants.APP_LIST_KEY];
                ssoConnectionString = props.ModuleKeys[BCCUIHelper.Constants.SSO_CONNECTION_KEY];
            }
            catch
            {
                // Ignore errors from Profile system
            }
        }

        foreach (string applicationName in SSOConfigHelper.ListApp(ssoConnectionString))
        {
            if (ssoAppList != null && ssoAppList.Count > 0)
            {
                if (ssoAppList.Contains(applicationName))
                {
                    searchList.Add(applicationName);
                }

                // This is for the datalist view on the right side. 
                dlSSOAppList.DataSource = ssoAppList;
                dlSSOAppList.DataBind();
                dlSSOAppList.Visible = true;

            }
            else
            {
                searchList.Add(applicationName);
            }
        }

        if (searchList != null)
        {
            searchTerms = searchList.ToSearchTerms();
        }
        else
        {
            searchTerms = "none";
        }
    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = false;
        errorImg.Visible = true;
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        HtmlForm myMasterForm = (HtmlForm)Master.FindControl("form1");

        if (myMasterForm != null)
        {
            myMasterForm.Controls.Add(gridSSO);
        }

        BCCGridView.Export("ControlCenter_" + System.Environment.MachineName + "_SSODB.xls", this.gridSSO);
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        // DO NOT : remove this method. 
    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        DataTable dt = null;

        if (txtSearchKey.Text != null && txtSearchKey.Text.Length > 0)
        {
            try
            {
                string appUserAcct, appAdminAcct, description, contactInfo;

                HybridDictionary props = SSOConfigManager.GetConfigProperties(txtSearchKey.Text, out description, out contactInfo, out appUserAcct, out appAdminAcct);

                dt = new DataTable();
                DataRow dr = null;

                // define the table's schema
                dt.Columns.Add(new DataColumn("AppUserAccount", typeof(string)));
                dt.Columns.Add(new DataColumn("AppAdminAccount", typeof(string)));
                dt.Columns.Add(new DataColumn("Description", typeof(string)));
                dt.Columns.Add(new DataColumn("ContactInfo", typeof(string)));

                dr = dt.NewRow();
                dr["AppUserAccount"] = appUserAcct;
                dr["AppAdminAccount"] = appAdminAcct;
                dr["Description"] = description;
                dr["ContactInfo"] = contactInfo;
                dt.Rows.Add(dr);

                gridAppView.Visible = true;
                gridAppView.DataSource = dt;
                gridAppView.DataBind();
               
                //-- http://www.codeproject.com/KB/webforms/Editable_GridView.aspx
                // http://msdn.microsoft.com/en-us/library/ms972948.aspx
                dt = new DataTable();
                dt.Columns.Add(new DataColumn("Application", typeof(string)));
                dt.Columns.Add(new DataColumn("KeyName", typeof(string)));
                dt.Columns.Add(new DataColumn("KeyValue", typeof(string)));

                foreach (DictionaryEntry appProp in props)
                {
                    dr = dt.NewRow();
                    dr["Application"] = txtSearchKey.Text;
                    dr["KeyName"] = appProp.Key.ToString();
                    dr["KeyValue"] = appProp.Value.ToString();
                    dt.Rows.Add(dr);
                }

                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    gridSSO.DataSource = dt;
                    gridSSO.DataBind();

                    count = dt.Rows.Count;
                }
                else
                {
                    dr = dt.NewRow();
                    dr["Application"] = txtSearchKey.Text;
                    dr["KeyName"] = "empty";
                    dr["KeyValue"] = "empty";
                    dt.Rows.Add(dr);

                    gridSSO.DataSource = dt;
                    gridSSO.DataBind();
                }

                if (count <= 0)
                {
                    DisplayError("No record(s) found.");
                }

            }
            catch(Exception ex)
            {
                DisplayError(ex.Message);
            }
        }
    }

    private void PopulateGridSSO()
    {
        btnFilter_Click(null, null);
    }


    protected void gridSSO_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gridSSO.EditIndex = -1;
        PopulateGridSSO();
    }

    protected void gridSSO_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gridSSO.EditIndex = e.NewEditIndex;
        PopulateGridSSO();
    }

    protected void gridSSO_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        try
        {
            Label txtKeyName = gridSSO.Rows[e.RowIndex].FindControl("txtKeyName") as Label;
            TextBox txtKeyValue = gridSSO.Rows[e.RowIndex].FindControl("txtKeyValue") as TextBox;

            //Its found in the first cell of the first row.
            string applicationName = gridSSO.Rows[0].Cells[0].Text;

            if ((txtKeyName != null && txtKeyName.Text != string.Empty) && (txtKeyValue != null && txtKeyValue.Text != string.Empty))
            {
                if (applicationName != null && applicationName.Length > 0)
                {
                    string keyName = txtKeyName.Text;
                    string keyValue = txtKeyValue.Text;
                    int rowCount = gridSSO.Rows.Count;

                    string[] propNames = new string[rowCount];
                    string[] propValues = new string[rowCount];

                    Label gridKeyLabel = null;
                    Label gridValueLabel = null;

                    for (int count = 0; count < rowCount; count++)
                    {
                        // Write all the left over values.
                        if (count != e.RowIndex)
                        {
                            gridKeyLabel = gridSSO.Rows[count].FindControl("lblKeyName") as Label;
                            gridValueLabel = gridSSO.Rows[count].FindControl("lblKeyValue") as Label;

                            propNames[count] = gridKeyLabel.Text;
                            propValues[count] = gridValueLabel.Text;
                        }
                        else // write the first one!
                        {
                            propNames[count] = keyName;
                            propValues[count] = keyValue;
                        }
                    }

                    // Finally update every thing.
                    SSOConfigHelper.Write(applicationName, propNames, propValues);
                    new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "updated " + keyName + " in " + applicationName, 211);
                }
            }

            gridSSO.EditIndex = -1;
            PopulateGridSSO();
        }
        catch (Exception exception)
        {
            DisplayError("SSO Update: " + exception.Message);
        }
    }

    protected void gridSSO_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName.Equals("Insert"))
            {
                TextBox txtKeyName = gridSSO.FooterRow.FindControl("txtNewKeyName") as TextBox;
                TextBox txtKeyValue = gridSSO.FooterRow.FindControl("txtNewKeyValue") as TextBox;

                //Its found in the first cell of the first row.
                string applicationName = gridSSO.Rows[0].Cells[0].Text;

                if ((txtKeyName != null && txtKeyName.Text != string.Empty) && (txtKeyValue != null && txtKeyValue.Text != string.Empty))
                {
                    if (applicationName != null && applicationName.Length > 0)
                    {
                        SSOPropBag propBag = new SSOPropBag();
                        int rowCount = gridSSO.Rows.Count;
                        
                        string keyName = txtKeyName.Text;
                        object keyValue = txtKeyValue.Text;

                        propBag.Write(keyName, ref keyValue);

                        string[] propNames = new string[rowCount + 1];
                        object[] propValues = new object[rowCount + 1];

                        Label gridKeyLabel = null;
                        Label gridValueLabel = null;

                        int loopCount = 0;

                        for (loopCount = 0; loopCount < rowCount; loopCount++)
                        {
                            gridKeyLabel = gridSSO.Rows[loopCount].FindControl("lblKeyName") as Label;
                            gridValueLabel = gridSSO.Rows[loopCount].FindControl("lblKeyValue") as Label;

                            propNames[loopCount] = gridKeyLabel.Text;
                            propValues[loopCount] = gridValueLabel.Text;

                            propBag.Write(propNames[loopCount], ref propValues[loopCount]);
                        }

                       // Finally update every thing.
                       SSOConfigManager.DeleteApplication(applicationName);
                       SSOConfigManager.CreateConfigStoreApplication(applicationName, "ControlCenter", "BizTalk Application Users", "BizTalk Server Administrators", propBag, null);
                      
                    }
                }
            }
        }
        catch (Exception exception)
        {
            DisplayError("SSO Insert: " + exception.Message);
        }
    }

    protected void gridSSO_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            Label lblKeyName = gridSSO.Rows[e.RowIndex].FindControl("lblKeyName") as Label;

            //Its found in the first cell of the first row.
            string applicationName = gridSSO.Rows[0].Cells[0].Text;

            if (applicationName != null && applicationName.Length > 0)
            {
                int rowCount = gridSSO.Rows.Count;

                string[] propNames = new string[rowCount];
                string[] propValues = new string[rowCount];

                Label gridKeyLabel = null;
                Label gridValueLabel = null;

                for (int count = 0; count < rowCount; count++)
                {
                    // Write all the left over values.
                    if (count != e.RowIndex)
                    {
                        gridKeyLabel = gridSSO.Rows[count].FindControl("lblKeyName") as Label;
                        gridValueLabel = gridSSO.Rows[count].FindControl("lblKeyValue") as Label;

                        propNames[count] = gridKeyLabel.Text;
                        propValues[count] = gridValueLabel.Text;
                    }
                    else // write the first one!
                    {
                        propNames[count] = lblKeyName.Text + "1";
                        propValues[count] = "";
                    }
                }
                // --  <asp:CommandField CausesValidation="true" HeaderText="Delete" ShowDeleteButton="false" /> 
                // Finally update every thing.
                SSOConfigHelper.Write(applicationName, propNames, propValues);
                new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "deleted " + lblKeyName.Text + " in " + applicationName, 211);
                PopulateGridSSO();
            }
        }
        catch (Exception exception)
        {
            DisplayError(exception.Message);
        }
    }

    protected void gridSSO_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");
        }

        if (BCCUIHelper.Constants.FLAG_TRUE.Equals(e.Row.Cells[1].Text.ToLower()))
        {
            e.Row.Cells[1].ForeColor = Color.Teal;
        }

        if (BCCUIHelper.Constants.FLAG_FALSE.Equals(e.Row.Cells[1].Text.ToLower()))
        {
            e.Row.Cells[1].ForeColor = Color.Brown;
        }
    }
}
