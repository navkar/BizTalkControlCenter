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
using System.Xml;
using System.Web.Configuration;
using BCC.Core;

public partial class BACD : System.Web.UI.Page
{
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
        if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN) || User.IsInRole(BCCUIHelper.Constants.ROLE_ARTIFACT))
        {
            string fileName = Request.QueryString.Get("name");
            FileInfo fInfo = new FileInfo(fileName);

            lblCaption.Text = "Configuration File View [" + fileName + "]";

            UserActivityGrid.Visible = true;
            displayArea.Text = ReadDataFromFile();
            displayArea.ToolTip = "viewing " + fileName;

            new ActivityHelper().RaiseAuditEvent(this, "BizTalk Artifacts - Configuration", "viewed " + fInfo.Name, 210);
        }
        else
        {
            displayArea.Text = "Access Denied. Check with your administrator.";
        }
    }

    protected string GetBaseFileName(string name)
    {
        return name.Substring(name.LastIndexOf("\\") + 1);
    }

    private string ReadDataFromFile()
    {
        string textData = string.Empty;
        string fileName = string.Empty;
        int i = 0;

        try
        {
            fileName = Request.QueryString.Get("name");
            FileInfo file = new FileInfo(fileName);
            string fileExtension = file.Extension.ToLower();

            if ( (fileExtension.Equals(".txt") 
                || fileExtension.Equals(".log") 
                || fileExtension.Equals(".config") 
                || fileExtension.Equals(".xml")) && file.Exists)
            {
                // Replace Search 
                textData = File.ReadAllText(file.FullName);

                //Encrypt Passwords
                while ((i = textData.ToUpper().IndexOf("PASSWORD", i)) != -1)
                {
                    if (textData.ToUpper().Substring(i, 10) == "PASSWORD=\"")
                    {
                        //To encrypt passwords in name value pairs which are in between two double quote characters.
                        textData = textData.Remove(i + 10, textData.IndexOf("\"", i + 10) - (i + 10));
                        textData = textData.Insert(i + 10, "********");
                    }
                    else if (textData.ToUpper().Substring(i, 17) == "PASSWORD\" VALUE=\"")
                    {
                        //To encrypt passwords in key value pairs format.
                        textData = textData.Remove(i + 17, textData.IndexOf("\"", i + 17) - (i + 17));
                        textData = textData.Insert(i + 17, "********");
                    }
                    else if (textData.IndexOf(";", i + 9) >= 0 && (textData.IndexOf(";", i + 9) < textData.IndexOf("\"", i + 9)))
                    {
                        //To encrypt passwords in connection strings which are ended with a semicolon character.
                        textData = textData.Remove(i + 9, textData.IndexOf(";", i + 9) - (i + 9));
                        textData = textData.Insert(i + 9, "********");
                    }
                    else
                    {
                        //To encrypt passwords in connection strings which are ended with a double quote character.
                        textData = textData.Remove(i + 9, textData.IndexOf("\"", i + 9) - (i + 9));
                        textData = textData.Insert(i + 9, "********");
                    }
                    i = i + 1;
                }
            }
            else
            {
                if (file.Exists)
                {
                    textData = string.Format("Cannot read a non text file '{0}' with file extension '{1}'.", fileName, fileExtension);
                }
                else
                {
                    textData = string.Format("File '{0}' does not exist.", fileName);
                }
            }
        }
        catch (Exception exception)
        {
            textData = string.Format("Exception: {0}. \n Details: {1}", exception.Message, exception.StackTrace);
        }

        return textData;
    }
}
