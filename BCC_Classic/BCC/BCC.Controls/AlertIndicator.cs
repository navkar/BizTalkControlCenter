using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BCC.Controls
{
    [DefaultProperty("Tooltip")]
    [ToolboxData("<{0}:AlertIndicator runat=server></{0}:AlertIndicator>")]
    public class AlertIndicator : WebControl
    {
        private string toolTip = string.Empty;
        private string color = "#FF0000";
        private string pixelWidth = "12";
        private string pixelHeight = "1";
        private string imagePath = "images/indicator/ball_redS.gif";

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("12")]
        [Localizable(true)]
        public string PixelWidth
        {
            get
            {
                return pixelWidth;
            }

            set
            {
                pixelWidth = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("1")]
        [Localizable(true)]
        public string PixelHeight
        {
            get
            {
                return pixelHeight;
            }

            set
            {
                pixelHeight = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Status
        {
            get
            {
                return toolTip;
            }

            set
            {
                toolTip = value;

                if (toolTip.Equals("Enabled") || toolTip.Equals("Started") || toolTip.Equals("Running"))
                {
                    color = "#99CC32";
                    imagePath = "images/indicator/ball_greenS.gif";
                }
                else if (toolTip.Equals("Bound") || toolTip.Equals("Enlisted") || toolTip.Equals("Unenlisted"))
                {
                    color = "#CECECE";
                    imagePath = "images/indicator/ball_blueS.gif";
                }
                else if (toolTip.Equals("Disabled") || toolTip.Equals("Unknown"))
                {
                    color = "#CECECE";
                    imagePath = "images/indicator/ball_greyS.gif";
                }
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("#FF0000")]
        [Localizable(true)]
        public string BackgroundColor
        {
            get
            {
                return this.color;
            }

            set
            {
                color = value;
            }
        }
        
        protected override void RenderContents(HtmlTextWriter output)
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"" + pixelWidth + "\" height=\"" + pixelHeight + "\" style=\"border-style: outset; border-width:1px; border-color: inherit;\">");
            //sb.Append("<tr><td bgcolor=\"" + color + "\" title=\"" + toolTip + "\">&nbsp;</td></tr></table>");
            //sb.Append("<tr><td><img src=\"" + imagePath + "\" alt=\"" + toolTip + "\"/></td></tr></table>");
            sb.Append("<img src=\"" + imagePath + "\" title=\"" + toolTip + "\" alt=\"" + toolTip + "\"></img>");
            // <tr><td><img src="" alt= /></td></tr></table>
            output.Write(sb.ToString());
            sb = null;
            //output.Close();
        }
    }
}
