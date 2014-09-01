using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BCC.WebUserControls
{
    public partial class InfoPanel : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string ErrorText
        {
            set
            {
                this.lblMessage.Text = value;
                lblMessage.Visible = true;
                errorImg.Visible = true;
            }
        }

        public string InfoText
        {
            set
            {
                this.lblMessage.Text = value;
                lblMessage.Visible = true;
                infoImg.Visible = true;
            }
        }
    }
}