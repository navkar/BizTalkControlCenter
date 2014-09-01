using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using BCC.Core;

public partial class UserRoles : System.Web.UI.Page
{
    private BCCDataAccess dataAccess = new BCCDataAccess();
    private BCCUIHelper uiHelper = new BCCUIHelper();

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

        if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN))
        {
            if (!Page.IsPostBack)
            {
                BindUsers();
                ActivateGrid();
                DataBindChart();
                UpdateProfile(this.Context.User.Identity.Name);
            }
        }
        else
        {
            DisablePanels();
            DisplayError(BCCUIHelper.Constants.ACCESS_DENIED);
        }
    }

    private void InitializeObjects()
    {
        tUserName.Focus();
        //updateBtn.Visible = false;
        lblCaption.Text = SiteMap.CurrentNode.ParentNode.Title + " - " + SiteMap.CurrentNode.Title;
        subCaption.Text = "User Information";
    }

    private void UpdateProfile(string userName)
    {
        string profilePath = MapPath("~/App_Data/DefaultProfile.xml");
        XmlDocument profileXmlDoc = new XmlDocument();
        profileXmlDoc.Load(profilePath);

        BCCProfile userProfile = BCCProfileHelper.CreateDefaultProfile(profileXmlDoc);

        Profile.ControlCenterProfile = userProfile;
        Profile.Save();

        new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, string.Format("refreshed user profile successfully.", userName), 603);
    }

    private void ActivateGrid()
    {
        UserActivityGrid.Visible = true;
    }

    private void Debug(string message)
    {
        System.Diagnostics.Debug.WriteLine(message, SiteMap.CurrentNode.Description);
    }

    protected void DisplayError(string message)
    {
        lblStatus.Text = message;
        lblStatus.Font.Bold = false;
        lblStatus.Visible = true;
        errorImg.Visible = true;
        okImg.Visible = false;
    }

    protected void DisplayInformation(string message)
    {
        lblStatus.Text = message;
        lblStatus.Font.Bold = false;
        lblStatus.Visible = true;
        okImg.Visible = true;
        errorImg.Visible = false;
    }

    private void DisablePanels()
    {
        tablePanel.Visible = false;
        tablePanel.Enabled = false;
        rolesPanel.Visible = false;
        rolesPanel.Enabled = false;
    }

    private void HideRolePanel()
    {
        rolesPanel.Visible = false;
    }

    private void DataBindChart()
    {
        // set series members names for the X and Y values 
        userRoleChart.ToolTip = "Displays the number of roles that a user has.";
        userRoleChart.Series["Default"]["DoughnutRadius"] = "30";
        userRoleChart.Series["Default"]["PieLabelStyle"] = "Outside";
        userRoleChart.Series["Default"]["PieDrawingStyle"] = "SoftEdge";
        userRoleChart.Series["Default"].XValueMember = "UserName";
        userRoleChart.Series["Default"].YValueMembers = "RoleCount";
        // Refer - http://blogs.msdn.com/b/alexgor/archive/2008/11/11/microsoft-chart-control-how-to-using-keywords.aspx
        userRoleChart.Series["Default"].Label = "#VALX (#VALY)";

        // data bind to the selected data source
        userRoleChart.DataBind();
    }

    private void PopulateEmailID(string userName)
    {
        MembershipUser user = Membership.GetUser(userName);

        if (user != null)
        {
            tbEmail.Text = user.Email;
        }
    }

    private void CheckIfUserIsLocked(string userName)
    {
        MembershipUser user = Membership.GetUser(userName);

        if (user != null && user.IsLockedOut)
        {
            chkLocked.Enabled = true;
            tbEmail.Text = user.Email;
        }
    }

    private bool PopulateRoleList(string userName)
    {
        bool notifyFlag = false;
        UserRoleList.Items.Clear();

        String[] roleNames = Roles.GetAllRoles();

        foreach (String roleName in roleNames)
        {
            ListItem roleListItem = new ListItem();

            if (roleName.Equals(BCCUIHelper.Constants.ROLE_ADMIN))
            {
                roleListItem.Text = roleName + " - Administrator Role (Complete control)";
            }
            else if (roleName.Equals(BCCUIHelper.Constants.ROLE_ARTIFACT))
            {
                roleListItem.Text = roleName + " - BizTalk Artifact Role (Used to manage BizTalk artifacts)";
            }
            else if (roleName.Equals(BCCUIHelper.Constants.ROLE_DEPLOY))
            {
                roleListItem.Text = roleName + " - Deployment Role (Used for BizTalk deployments only)";
            }
            else if (roleName.Equals(BCCUIHelper.Constants.ROLE_MEMBER))
            {
                roleListItem.Text = roleName + " - Member Role (Default access)";
            }

            roleListItem.Value = roleName;
            roleListItem.Selected = Roles.IsUserInRole(userName, roleName);

            UserRoleList.Items.Add(roleListItem);

            if (roleListItem.Selected)
            {
                notifyFlag = true;
            }
        }

        return notifyFlag;
    }

    private void UpdateRolesFromList()
    {
        string userName = tUserName.Text;

        foreach (ListItem roleListItem in UserRoleList.Items)
        {
            String roleName = roleListItem.Value;
            bool enableRole = roleListItem.Selected;

            if (enableRole && !Roles.IsUserInRole(userName, roleName))
            {
                Roles.AddUserToRole(userName, roleName);

            }
            else if (!enableRole && Roles.IsUserInRole(userName, roleName))
            {
                Roles.RemoveUserFromRole(userName, roleName);
            }
        }

        DisplayInformation("Selected role(s) for the user '<b>" + userName + "</b>' have been updated.");

        bool notifyFlag = PopulateRoleList(tUserName.Text);

        if (!notifyFlag)
        {
            if (DeleteUser(tUserName.Text))
            {
                DisplayInformation("User '<b>" + userName + "</b>' has been removed from the system.");
                new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "Deleted " + userName, 601);
            }
            else
            {
                DisplayError("Unable to delete user '<b>" + userName + "</b>' due to assigned tasks, see speedcode '702'.");
            }
        }
    }

    /// <summary>
    /// You cannot delete a user, if the user has assigned tasks.
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    private bool DeleteUser(string userName)
    {
        bool successFlag = true;

        try
        {
            Membership.DeleteUser(userName, true);
        }
        catch
        {
            successFlag = false;
        }

        return successFlag; 
    }

    private void CreateUser(string userName, string userEmail)
    {
        // Check if user exists
        MembershipUser user = Membership.GetUser(userName);

        if (user == null)
        {
            // Name, Password and email
            Membership.CreateUser(userName, "P@ssw0rd", userEmail);
            // Create a default profile.
            CreateDefaultProfile(userName);
            // Adding default roles
            Roles.AddUserToRole(userName, BCCUIHelper.Constants.ROLE_MEMBER);
            new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "created " + userName, 601);
        }
    }

    private void CreateDefaultProfile(string userName)
    {
        ProfileCommon common = (ProfileCommon) ProfileCommon.Create(userName, true);

        Debug("UserName:" + common.UserName);

        if (common != null)
        {
            string profilePath = MapPath("~/App_Data/DefaultProfile.xml");
            XmlDocument profileXmlDoc = new XmlDocument();
            profileXmlDoc.Load(profilePath);

            Debug(profilePath);

            BCCProfile userProfile = BCCProfileHelper.CreateDefaultProfile(profileXmlDoc);

            common.ControlCenterProfile = userProfile;
            common.Save();
        }
    }

    private void EnableRoleView()
    {
        updateBtn.Visible = true;
        rolesPanel.Visible = true;
    }

    private void BindUsers()
    {
        UserGrid.DataSource = (object)Membership.GetAllUsers();
        UserGrid.DataBind();
    }

    #region Click events
    protected void btnLookup_Click(object sender, EventArgs e)
    {
        try
        {
            if (tUserName.Text != null && tUserName.Text.Length > 0)
            {
                bool notifyFlag = PopulateRoleList(tUserName.Text);

                if (!notifyFlag)
                {
                    DisplayError("User <b>'" + tUserName.Text + "'</b> does NOT exist in the system. You need to create this user by selecting Roles and clicking 'Update user...'");
                    EnableRoleView();
                }
                else
                {
                    EnableRoleView();
                    PopulateEmailID(tUserName.Text);
                    CheckIfUserIsLocked(tUserName.Text);
                }
            }
            else
            {
                DisplayError("Enter a user name (e.g. john) to view the list of roles.");
                tUserName.Text = this.User.Identity.Name;
            }

            new ActivityHelper().RaiseAuditEvent(this, lblCaption.Text, "searched user '" + tUserName.Text + "'", 601);
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
        }
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            // Check if user exists
            MembershipUser user = Membership.GetUser(tUserName.Text);

            if (user != null)
            {
                if (chkLocked.Checked && user.IsLockedOut)
                {
                    user.UnlockUser();
                    Membership.UpdateUser(user);
                }

                user.Email = tbEmail.Text;
                Membership.UpdateUser(user);
            }
            else
            {
                // Create a new user and give it a member role.
                CreateUser(tUserName.Text, tbEmail.Text);
                DisplayInformation("User '" + tUserName.Text + "' has been created.");
            }

            UpdateRolesFromList();
            PopulateRoleList(tUserName.Text);
            HideRolePanel();
            BindUsers();
            DataBindChart();
        }
        catch (Exception ex)
        {
            DisplayError("Problem updating roles, " + ex.Message);
        }
    }
    #endregion 

    #region UserGrid events
    protected void UserGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");

            LinkButton lnkUser = e.Row.Cells[0].FindControl("lnkUser") as LinkButton;
            Label lblLocked = e.Row.Cells[2].FindControl("lblLocked") as Label;

            if (lnkUser != null)
            {
                if (Roles.IsUserInRole(lnkUser.Text, BCCUIHelper.Constants.ROLE_ADMIN))
                {
                    lnkUser.ForeColor = Color.White;
                    lnkUser.BackColor = Color.RosyBrown;
                    lnkUser.ToolTip = "User is an Administrator";
                }
                else
                {
                    lnkUser.ForeColor = Color.White;
                    lnkUser.BackColor = Color.Gray;
                    lnkUser.ToolTip = "Regular user";
                }
            }

            if (lblLocked != null)
            {
                if (lblLocked.Text.Equals("True"))
                {
                    lblLocked.Text = "Yes";
                    lblLocked.BackColor = Color.SandyBrown;
                }
                else
                {
                    lblLocked.Text = "No";
                }
            }
        }
    }

    protected void UserGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        GridViewRow deletedRow = UserGrid.Rows[e.RowIndex] as GridViewRow;

        try
        {
            if (deletedRow != null)
            {
                LinkButton lnkUser = deletedRow.FindControl("lnkUser") as LinkButton;

                if (lnkUser != null)
                {
                    string userName = lnkUser.Text;

                    if (!userName.Equals(HttpContext.Current.User.Identity.Name))
                    {
                        if (DeleteUser(userName))
                        {
                            DisplayInformation(string.Format("User '{0}' has been deleted.", userName));
                        }
                        else
                        {
                            DisplayError("Unable to delete user '<b>" + userName + "</b>' due to assigned tasks, see speedcode '702'.");
                        }

                        BindUsers();
                        DataBindChart();
                    }
                    else
                    {
                        DisplayError(string.Format("Logged in user '{0}' cannot be deleted.", userName));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
        }
    }

    protected void UserGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "Add")
            {
                TextBox tNewUserName = UserGrid.FooterRow.FindControl("tNewUserName") as TextBox;
                TextBox tNewUserEmail = UserGrid.FooterRow.FindControl("tNewUserEmail") as TextBox;

                if (tNewUserName != null && tNewUserEmail != null)
                {
                    CreateUser(tNewUserName.Text, tNewUserEmail.Text);
                    DisplayInformation("User '" + tNewUserName.Text + "' has been created.");
                    BindUsers();
                    DataBindChart();
                }
            }
            else if (e.CommandName == "UpdateUser")
            {
                GridViewRow selectedRow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                LinkButton lnkUser = selectedRow.FindControl("lnkUser") as LinkButton;

                if (lnkUser != null)
                {
                    tUserName.Text = lnkUser.Text;
                    btnLookup_Click(e.CommandSource, null);
                }

            }
        }
        catch (Exception ex)
        {
            DisplayError(ex.Message);
        }
    }

    #endregion
}
