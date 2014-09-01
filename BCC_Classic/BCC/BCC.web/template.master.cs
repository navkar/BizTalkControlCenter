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
using System.Collections.Specialized;
using BCC.Core;

public partial class template : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            AddEventHandlers();
        }

        SetTitles();
        ProcessSpeedCode(speedCode.Text);
        speedCode.Focus();
    }

    private void SetTitles()
    {
        lblInfo.Text = "BizTalk Control Center [" + System.Environment.MachineName + "]";
        Page.Title = "BCC - [" + System.Environment.MachineName + "]";
        footerLabel.Text = "Copyright © 2011 Naveen Karamchetti. All rights reserved. (biztalkcontrolcenter@gmail.com)";
        footerLabel.Font.Bold = false;
        footerLabel.Font.Size = FontUnit.XXSmall;
    }

    private void ProcessSpeedCode(string speedCode)
    {
        lblSpeedCodeTooltip.Text = SiteMap.CurrentNode.Description;

        if (speedCode != null && speedCode.Length > 0 && speedCode.Length == 3)
        {
            SiteMapNode root = SiteMap.RootNode;
            SiteMapNodeCollection collection = root.GetAllNodes();
            string naviUrl = string.Empty;
            bool foundFlag = false;

            foreach (SiteMapNode node in collection)
            {
                if (node.Description == speedCode)
                {
                    naviUrl = node.Url;
                    foundFlag = true;
                    break;
                }
            }

            if (foundFlag)
            {
                Response.Redirect(naviUrl);
            }
        }
    }

    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static string GetDynamicContent(string contextKey)
    {
        return default(string);
    }
    
    private void AddEventHandlers()
    {
         SiteMapPath.ItemCreated += new SiteMapNodeItemEventHandler(SiteMapPath_ItemCreated);
    }

    protected void SiteMapPath_ItemCreated(object sender, SiteMapNodeItemEventArgs e)
    {
        if (e.Item.ItemType == SiteMapNodeItemType.Root)
        {
            SiteMapPath.PathSeparator = " ";
        }
        else
        {
            SiteMapPath.PathSeparator = " > ";
        }
    }

    //http://msdn.microsoft.com/en-us/library/system.web.sitemap.sitemapresolve.aspx
    //solve > SiteMapNode is readonly, property Title cannot be modified. 

    SiteMapNode SiteMap_SiteMapResolve(object sender, SiteMapResolveEventArgs e)
    {
        if (SiteMap.CurrentNode != null)
        {
            SiteMapNode currentNode = SiteMap.CurrentNode.Clone(true);
            SiteMapNode tempNode = currentNode;
            tempNode = ReplaceNodeText(tempNode);

            return currentNode;
        }

        return null;
    }

    //remove <u></u> tag recursively
    internal SiteMapNode ReplaceNodeText(SiteMapNode smn)
    {
        //current node
        if (smn != null && smn.Title.Contains("<u>"))
        {
            smn.Title = smn.Title.Replace("<u>", "").Replace("</u>", "");
        }

        //parent nd
        if (smn.ParentNode != null)
        {
            if (smn.ParentNode.Title.Contains("<u>"))
            {
                SiteMapNode gpn = smn.ParentNode;
                smn.ParentNode.Title = smn.ParentNode.Title.Replace("<u>", "").Replace("</u>", "");
                smn = ReplaceNodeText(gpn);
            }
        }
        return smn;
    }


}
