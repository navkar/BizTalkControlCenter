using System;
using System.Web;
using System.IO;
using BCC.Core;

public partial class DeploymentBindingsDownload : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN) || User.IsInRole(BCCUIHelper.Constants.ROLE_DEPLOY))
        {
            string fileName = Request.QueryString.Get("name");
            FileInfo fInfo = new FileInfo(fileName);
            WriteFile(fileName);
        }
    }

    protected void WriteFile(string fileName)
    {
        try
        {
            FileInfo file = new FileInfo(fileName);
            string fileExtension = file.Extension.ToLower();

            if ((fileExtension.Equals(".msi") || fileExtension.Equals(".xml")) && file.Exists)
            {
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.ContentType = "application/octet-stream";
                Response.TransmitFile(file.FullName);
                Response.End();
            }
        }
        catch
        {
        }
    }
}
