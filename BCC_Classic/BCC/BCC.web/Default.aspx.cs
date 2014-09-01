using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;

using BCC.Core;

public partial class Default : System.Web.UI.Page
{
    BCCDataAccess dataAccess = new BCCDataAccess();
    BCCOperator hubOperator = new BCCOperator();
    BCCUIHelper uiHelper = new BCCUIHelper();

    protected void Page_PreInit(object sender, EventArgs e)
    {
        try
        {
            string defaultTheme = "day";
            // It doesnt make any sense to use Profile in this page, since the user has not logged in yet. :) 

            // Use sessions - will help during logging out.
            if (Session["PAGE_THEME"] != null)
            {
                this.Page.Theme = Session["PAGE_THEME"] as String;
            }
            else if (Request.Cookies["theme"] != null)
            {
                this.Page.Theme = Request.Cookies["theme"].Value;
            }
            else
            {
                this.Page.Theme = defaultTheme;
            }
        }
        catch
        {

        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            int memberOnlineCount = Membership.GetNumberOfUsersOnline();

            myLogin.Focus();
            changePwdPanel.Visible = false;

            if (Request.Cookies["userInfo"] != null)
            {
                DisplayMsg("Welcome " +
                    Server.HtmlEncode(Request.Cookies["userInfo"]["userName"]) + ", your last login was on " +
                    Server.HtmlEncode(Request.Cookies["userInfo"]["lastVisit"]) + ". There are " +
                Membership.GetNumberOfUsersOnline().ToString() + " user(s) currently online, since last " + Membership.UserIsOnlineTimeWindow + " minutes.");
            }
            else
            {
                if (memberOnlineCount > 0)
                {
                    DisplayMsg("There are " + memberOnlineCount.ToString() + " user(s) currently online.");
                }
                else
                {
                    DisplayMsg("There are no users currently online.");
                }
            }

            DisplayActivity(ActivityHelper.LastKnownActivity(HttpContext.Current));
            CheckAgentServices();
        }
        catch (Exception exception)
        {
            DisplayError(exception.Message);
        }
    }

    private void CheckAgentServices()
    {
        try
        {
            StringCollection serviceList = new StringCollection();
            serviceList.Add(ConfigurationManager.AppSettings["BCCAgentName"].ToString());

            if (serviceList != null && serviceList.Count > 0)
            {
                BCCOperator bccOperator = new BCCOperator();
                DataTable dtService = bccOperator.GetServiceStatus(serviceList);

                foreach (DataRow serviceRecord in dtService.Rows)
                {
                    if (serviceRecord[1].ToString().Equals("Not Installed"))
                    {
                        AnnounceError("Alert: " + serviceRecord[0] + " is not installed, monitoring will not work.");
                        break;
                    }
                    else if (serviceRecord[1].ToString().Equals("Stopped"))
                    {
                        AnnounceError("Alert: " + serviceRecord[0] + " is currently stopped, monitoring will not work.");
                        break;
                    }
                }

            }
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
        }
    }

    protected void AnnounceError(string message)
    {
        Master.DisplayError(message);
    }
    
    protected void DisplayError(string message)
    {
        lblStatus.Text = message;
        lblStatus.Font.Bold = false;
	    
        lblStatus.Visible = true;
    }

    protected void DisplayInformation(string message)
    {
        lblStatus.Text = message;
        lblStatus.Font.Bold = false;
        lblStatus.Visible = true;
    }

    protected void DisplayMsg(string message)
    {
        lblInfo.Text = message;
        lblInfo.Font.Bold = false;
        lblInfo.Visible = true;
    }

    protected void DisplayActivity(string message)
    {
        lblLastActivity.Text = message;
        lblLastActivity.Font.Bold = false;
        lblLastActivity.Visible = true;
    }

    private void SetDefaultProfileForAdmin(string userName)
    {
        // Create a default profile for Admin account
        if (userName.ToLower().Equals("admin"))
        {
            ProfileCommon common = (ProfileCommon)ProfileCommon.Create(userName, true);

            if (common != null)
            {
                common.ControlCenterProfile.UserTheme = "Day";
                common.ControlCenterProfile.IsProfileActive = true;
                common.ControlCenterProfile.IsFilterEnabled = true;
                common.Save();
            }
        }
    }

    public void Login_OnLoggingIn(object sender, EventArgs args)
    {
        if (Membership.ValidateUser(myLogin.UserName, myLogin.Password))
        {
            FormsAuthentication.SetAuthCookie(myLogin.UserName, true);

            new ActivityHelper().RaiseAuditEvent(this, "Login", myLogin.UserName + " login success", 100);

            Response.Cookies["userInfo"]["userName"] = myLogin.UserName;
            Response.Cookies["userInfo"]["lastVisit"] = DateTime.Now.ToString();
            Response.Cookies["userInfo"].Expires = DateTime.Now.AddDays(5);

            // Admin user must be redirect to users page.
            if (myLogin.UserName.ToLower().Equals("admin"))
            {
                if (!Profile.ControlCenterProfile.IsProfileActive)
                {
                    SetDefaultProfileForAdmin(myLogin.UserName);
                }

                Response.Redirect(ConfigurationManager.AppSettings["AdminUserStartPage"]);
            }
            else // Everyone else 
            {
                Response.Redirect(FormsAuthentication.GetRedirectUrl(myLogin.UserName, true));
            }
        }

    }

    public void Login_OnLoginError(object sender, EventArgs args)
    {
        new ActivityHelper().RaiseAuditEvent(this, "Login", myLogin.UserName + " login failure", 100);
    }

    protected void BtnChangePwd_Click(object sender, EventArgs args)
    {
        changePwdPanel.Visible = true;
        ChPwd.Focus();
    }

    protected void CancelButton_Click(object sender, EventArgs args)
    {
        changePwdPanel.Visible = false;
    }

    protected void ChangePassword_Click(object sender, EventArgs e)
    {
        changePwdPanel.Visible = true;

        try
        {
            MembershipUser mu = Membership.GetUser(ChPwd.UserName);

            if (mu != null)
            {
                mu.ChangePassword(ChPwd.CurrentPassword, ChPwd.NewPassword);
                DisplayInformation("OK");
                changePwdPanel.Visible = false;
                new ActivityHelper().RaiseAuditEvent(this, "Login", "changed password", 100);
            }
            else
            {
                DisplayError(ChPwd.UserName + " was not found in the system.");
                new ActivityHelper().RaiseAuditEvent(this, "Login", "not found", 100);
            }
        }
        catch (Exception ex)
        {
            DisplayError("For user " + ChPwd.UserName + " exception is " + ex.Message);
        }

    }
 }