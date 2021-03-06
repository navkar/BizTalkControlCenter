using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;

namespace BCC.Controls
{
    // WITGridDesigner
    // This class generates HTML code for the web page design mode in VS2005
    public class WITGridDesigner : ControlDesigner
    {
        //In case you encounter the message Unable to cast object of type 'X' to type 'X' 
        //please take a look at http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=167058&SiteID=1
        //This is a known problem: different modules are loaded. Close VS2005 and restart, rebuild


        //GetDesignTimeHtml returns the HTML for design mode
        public override string GetDesignTimeHtml()
        {
            //Returns HTML that shows the grid in design view of your web form
            //There are three options: 
            // - show the gray control box using CreatePlaceHolderDesignHtml
            // - use the grid's own render info
            // - build custom HTML

            try
            {
                //Point to the control to gather information
                WITGrid wg = (WITGrid)base.Component;
                if (wg.GridFields == null)
                {
                    //No columns, return empty control
                    return GetEmptyDesignTimeHtml();
                }
                else
                {
                    //To show the grid using it's own rendered HTML:
                    //wg.DataBind();
                    //return base.GetDesignTimeHtml();

                    //This is not used because it is slow (databind) and because
                    //of all the related HTML-events

                    //Show custom HTML
                    return ShowControlInfo(wg);
                }
            }
            catch (Exception ex)
            {
                //Return HTML for control with error
                return GetErrorDesignTimeHtml(ex);
            }
        }
        protected override string GetEmptyDesignTimeHtml()
        {
            //Returns a gray control box including a custom text.
            //Showed in design view in case the control is empty
            string text = "The control is empty. Please add columns/field items.";
            return CreatePlaceHolderDesignTimeHtml(text);
        }

        protected override string GetErrorDesignTimeHtml(Exception e)
        {
            //Returns the HTML for a gray control box including the error message
            //This is showed when an error occurs when displaying the control
            //in design view.
            string text = "An error occurred. The control cannot be displayed. <br />";
            text += "Exception: " + e.Message;
            return CreatePlaceHolderDesignTimeHtml(text);
        }

        //ShowControlInfo renders a simple version of the grid control
        //with the data field names in the columns
        private string ShowControlInfo(WITGrid wg)
        {
            string s = "";

            s += "<table cellspacing='0' cellpadding='0'";
            if (wg.CssClass != null) s += " class='" + wg.CssClass + "'";
            s += " >";

            //header ------------------------------------------------------------------
            s += "<thead><tr>";
            foreach (GridField gf in wg.GridFields)
            {
                s += "<th scope='col'";
                if (gf.CssClass != null) s += " class='" + gf.CssClass + "'";
                s += ">" + gf.HeaderText + "</th>";
            }

            //Special columns
            if (wg.ShowDeleteColumn)
                s += "<th>Delete</th>";
            if (wg.ShowEditColumn)
                s += "<th>Edit</th>";
            if (wg.ShowOrderColumn)
                s += "<th>Order</th>";

            s += "</thead>";

            //Body, 3 rows-----------------------------------------------------------------------
            s += "<tbody>";
            for (int i = 0; i <= 3; i++)
            {
                s += "<tr";
                if (i == 1) s += " class='gridview-highlight'";
                s += ">";

                foreach (GridField gf in wg.GridFields)
                {
                    s += "<td";
                    if (gf.CssClass != null) s += " class='" + gf.CssClass + "'";
                    s += ">";
                    if (gf is CheckBoxField)
                        s += "[" + ((CheckBoxField)gf).DataField + "]";
                    else if (gf is BoundField)
                        s += "[" + ((BoundField)gf).DataField + "]";
                    s += "</td>";
                }
                //Special columns, I couldn't find a way to include the images
                if (wg.ShowDeleteColumn)
                    s += "<td>[delete]</td>";
                if (wg.ShowEditColumn)
                    s += "<td>[edit]</td>";
                if (wg.ShowOrderColumn)
                    s += "<td>[order]</td>";
                s += "</tr>";
            }

            s += "</tbody>";

            return s;

        }

        //Build the smart tags (see WITGridActionList.cs)
        private DesignerActionListCollection _actionList;
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionList == null)
                {
                    _actionList = new DesignerActionListCollection();
                    _actionList.Add(new WITGridActionList((WITGrid)Component));
                }
                return _actionList;
            }
        }
    }
}
