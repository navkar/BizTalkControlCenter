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
using System.Net;
using System.IO;
using BCC.Core;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml;
using System.Collections.Specialized;

public partial class PartnerWebService : System.Web.UI.Page
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

    protected void DisplayError(string message)
    {
        lblError.Visible = true;
        errorImg.Visible = true;
        lblError.Text = message;
        lblError.Font.Bold = false;
    }

    private void InitializeObjects()
    {
        lblError.Visible = false;
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "Information";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeObjects();

        if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN) || User.IsInRole(BCCUIHelper.Constants.ROLE_ARTIFACT))
        {
            if (!IsPostBack)
            {
                InitializeWSType();
                ActivateGrid();
            }
            else
            {

            }
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

    private void InitializeWSType()
    {
        StringCollection serviceList = null;
        // Use filters here.
        // Check whether the Profile is active and then proceed.
        if (Profile.ControlCenterProfile.IsProfileActive)
        {
            try
            {
                BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                serviceList = props.ModuleDictionary[BCCUIHelper.Constants.SC301_SERVICE_NAME];
            }
            catch
            {
               
            }
        }

        if (serviceList != null && serviceList.Count > 0)
        {
            uiHelper.PopulateDropDown(dListServices, serviceList);
        }

    }

    protected void dListService_SelectedIndexChanged(object sender, EventArgs e)
    {
        string selectedKey = dListServices.SelectedItem.Value.ToString();

        StringCollection serviceList = null;
        // Use filters here.
        // Check whether the Profile is active and then proceed.
        if (Profile.ControlCenterProfile.IsProfileActive)
        {
            try
            {
                BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
                serviceList = props.ModuleDictionary[selectedKey];

                string webRequestTimeout = props.ModuleKeys[BCCUIHelper.Constants.SC301_WEB_REQ_TIMEOUT];

                //Timeout value
                submit.CommandArgument = webRequestTimeout;

                if (serviceList.Count > 1)
                {
                    // URL please
                    endPoint.Text = serviceList[0];
                    inpRequest.Text = serviceList[1];
                }
            }
            catch(Exception exception)
            {
                DisplayError(exception.Message);
            }
        }
    }

    private void SaveWebRequest(string endPoint, string inputRequest)
    {
        try
        {
            string selectedKey = dListServices.SelectedItem.Value.ToString();
            BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter[SiteMap.CurrentNode.Description];
            StringCollection serviceList = props.ModuleDictionary[selectedKey];

            if (serviceList.Count > 1)
            {
                serviceList[0] = endPoint;
                serviceList[1] = inputRequest;
            }
        }
        catch (Exception exception)
        {
            DisplayError(exception.Message);
        }
    }


    protected void btnWebServiceCall_Click(object sender, EventArgs e)
    {
        try
        {
            int timeoutInMilliseconds = 0;

            if (String.IsNullOrEmpty(inpRequest.Text.ToString()))
            {
                Exception userExp = new Exception("Provide a valid request to execute at partners end point.");
                throw userExp;
            }

            LinkButton submitBtn = sender as LinkButton;
            string timeOutValue = submitBtn.CommandArgument;

            Int32.TryParse(timeOutValue, out timeoutInMilliseconds);

            if (timeoutInMilliseconds == 0)
            {
                timeoutInMilliseconds = 30 * 1000;
            }

            SaveWebRequest(endPoint.Text, inpRequest.Text);

            XmlDocument xmlDoc = new XmlDocument();
            // Load the xml doc
            xmlDoc.LoadXml(inpRequest.Text);
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(endPoint.Text);

            // Request properties
            httpReq.Method = "POST";
            httpReq.ContentType = "text/xml";
            httpReq.Timeout = timeoutInMilliseconds;

            // Get the http request stream
            Stream httpReqStream = httpReq.GetRequestStream();
            //write the XML to the stream
            xmlDoc.Save(httpReqStream);
            // Close the stream
            httpReqStream.Close();

            // Get the raw response from the webservice
            HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse();
            Stream httpRespStream = httpResp.GetResponseStream();
            // Read the response
            StreamReader httpRespStreamReader = new StreamReader(httpRespStream, System.Text.Encoding.GetEncoding("utf-8"));
            string reponse = httpRespStreamReader.ReadToEnd();

            // Assign the response to the response window
            outResponse.Text = reponse.ToString();
            new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, " invoked '" + endPoint.Text + "'", 301);
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message.ToString());
        }
    }
    
}
