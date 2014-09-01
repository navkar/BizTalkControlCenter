using BCC.Core;
using System;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.UI.DataVisualization.Charting;
using Microsoft.EnterpriseSingleSignOn.Interop;

public partial class ScalabilityView : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCOperator hubOperator = new BCCOperator();
    BCCUIHelper uiHelper = new BCCUIHelper();
    public string searchTerms = "";
    public string testData = "";
    public int count = 0; // This is a global variable

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
            if (!Page.IsPostBack)
            {
                GrabQueryString();
            }

            InitializeView();
            InitializeObjects();
            btnFilter_Click(null, null);

        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
            gridScaleIndex.Visible = false;
        }
    }

    private void InitializeView()
    {
        // Initialize - cannot take more than 10 values.
        SelectionBar searchList = new SelectionBar(5, "DSI.aspx");
        StringCollection applicationList = null;
        StringCollection appList = null;

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
            appList = dataAccess.RetrieveAllApplications(applicationList);
        }
        else
        {
            appList = dataAccess.RetrieveAllApplications();
        }


        foreach (String applicationName in appList)
        {
            searchList.Add(applicationName);
        }

        if (searchList != null)
        {
            searchTerms = searchList.ToSearchTerms();
        }
        else
        {
            searchTerms = "<empty>";
        }
    }

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        lblError.Text = message;
        errorImg.Visible = true;
    }

    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        HtmlForm myMasterForm = (HtmlForm)Master.FindControl("form1");

        if (myMasterForm != null)
        {
            myMasterForm.Controls.Add(gridScaleIndex);
        }

        BCCGridView.Export("ControlCenter_" + System.Environment.MachineName + "_ScalabilityIndex.xls", this.gridScaleIndex);
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        // DO NOT : remove this method. 
    }

    public void PopulateScaleIndex()
    {
        btnFilter_Click(null, null);
    }

    protected void gridScaleIndex_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");

            if (e.Row.Cells != null)
            {
                string bizTalkArtifactType = e.Row.Cells[0].Text;

                DropDownList hostDropDown = e.Row.FindControl("hostDropDown") as DropDownList;
                Label lblHandler = e.Row.FindControl("lblHandler") as Label;

                if (hostDropDown != null)
                {
                    switch (bizTalkArtifactType)
                    {
                        case BCCUIHelper.Constants.ARTIFACT_RCV:
                            hostDropDown.DataSource = dataAccess.RetrieveArtifactHandlers(BCCUIHelper.Constants.ARTIFACT_RCV);
                            break;

                        case BCCUIHelper.Constants.ARTIFACT_SND:
                            hostDropDown.DataSource = dataAccess.RetrieveArtifactHandlers(BCCUIHelper.Constants.ARTIFACT_SND);
                            break;

                        default:
                            hostDropDown.DataSource = dataAccess.RetrieveArtifactHandlers(BCCUIHelper.Constants.ARTIFACT_ODX);
                            break;
                    }

                    hostDropDown.DataBind();
                    hostDropDown.SelectedValue = gridScaleIndex.DataKeys[e.Row.RowIndex].Values[0].ToString();

                }
            }
        }
    }



    protected void gridScaleIndex_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Edit")
        {
            LinkButton editLink = e.CommandSource as LinkButton;
            GridViewRow selectedRow = (GridViewRow)editLink.NamingContainer;

            DropDownList hostDropDown = editLink.Parent.FindControl("hostDropDown") as DropDownList;

            if (selectedRow != null && hostDropDown != null)
            {
                string bizTalkArtifactType = selectedRow.Cells[0].Text;


                switch (bizTalkArtifactType)
                {
                    case BCCUIHelper.Constants.ARTIFACT_RCV:
                        // ddlArtifactHost.DataSource = dataAccess.RetrieveArtifactHandlers(BCCUIHelper.Constants.ARTIFACT_RCV);
                        break;

                    case BCCUIHelper.Constants.ARTIFACT_SND:
                        //  ddlArtifactHost.DataSource = dataAccess.RetrieveArtifactHandlers(BCCUIHelper.Constants.ARTIFACT_SND);
                        break;

                    default:
                        // ddlArtifactHost.DataSource = dataAccess.RetrieveArtifactHandlers(BCCUIHelper.Constants.ARTIFACT_ODX);
                        break;
                }

                //ddlArtifactHost.DataBind();
            }
        }
    }

    protected void gridScaleIndex_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gridScaleIndex.EditIndex = -1;
        PopulateScaleIndex();
    }

    protected void gridScaleIndex_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gridScaleIndex.EditIndex = e.NewEditIndex;
        PopulateScaleIndex();
    }

    protected void gridScaleIndex_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        PopulateScaleIndex();
    }

    public void DisplayBarChart(string applicationName, string scalabilityRatio, int oaHosts, int saHosts, int raHosts, int oeHosts, int seHosts, int reHosts)
    {
        hostChart.Series["Actual"]["PointWidth"] = "0.6";
        hostChart.Series["Actual"].Points.AddXY("ODX", oaHosts);
        hostChart.Series["Actual"].Points.AddXY("Send", saHosts);
        hostChart.Series["Actual"].Points.AddXY("Receive", raHosts);
        hostChart.Series["Actual"].IsValueShownAsLabel = true;


        hostChart.Series["Expected"]["PointWidth"] = "0.6";
        hostChart.Series["Expected"].Points.AddXY("ODX", oeHosts);
        hostChart.Series["Expected"].Points.AddXY("Send", seHosts);
        hostChart.Series["Expected"].Points.AddXY("Receive", reHosts);
        hostChart.Series["Expected"].IsValueShownAsLabel = true;

        chartHeader.Text = applicationName + " Hosts";
        chartHeader.Visible = true;

        chartFooter.Text = "Scalability ratio = " + scalabilityRatio;
        chartFooter.Visible = true;
        chartPanel.Visible = true;

    }

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        DataTable dt = null;
        int oaHosts, raHosts, saHosts;
        int oeHosts, reHosts, seHosts;
        int oCount, rCount, sCount;

        oaHosts = raHosts = saHosts = 0;
        oCount = rCount = sCount = 0;
        string applicationName = txtSearchKey.Text;

        if (applicationName != null && applicationName.Length > 0)
        {
            try
            {
                dt = dataAccess.EnumerateAllArtifacts(applicationName, out oCount, out rCount, out sCount,
                                                                        out oaHosts, out raHosts, out saHosts);
                ScalabilityIndex index = new ScalabilityIndex();
                double ratio = index.Compute(oCount, oaHosts, out oeHosts, rCount, raHosts, out reHosts, sCount, saHosts, out seHosts);
                string scalabilityRatio = ratio.ToString("F");

                // 1,2,1|3,4,5 - use in the case of emergency only.
                // string chartData = oaHosts + "," + saHosts + "," + raHosts + "|" + oeHosts + "," + seHosts + "," + reHosts;
                // hostChart.Src = "http://chart.apis.google.com/chart?chs=270x200&cht=bvg&chdl=Actual Hosts|Expected Hosts&chco=4D89F9,C6D9FD&chbh=15,4,15&chd=t:" + chartData + "&chds=0,7&chxt=x,y&chxl=0:|ODX|Send|Receive&chxr=1,0,7&&chm=N**,000000,0,-1,11|N**,000000,1,-1,11&chf=bg,lg,0,E9E9E9,0,E9E9E9,1&chtt=" + applicationName.Replace(' ', '+') + "|Scalability+Index+=+" + scalabilityRatio;
                DisplayBarChart(applicationName, scalabilityRatio, oaHosts, saHosts, raHosts, oeHosts, seHosts, reHosts);

                gridScaleIndex.Visible = true;
                gridScaleIndex.DataSource = dt;

                if (dt != null)
                {
                    count = dt.Rows.Count;
                }

                gridScaleIndex.DataBind();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }
    }
}
