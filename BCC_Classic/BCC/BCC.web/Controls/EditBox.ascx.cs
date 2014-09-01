using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Controls_EditBox : System.Web.UI.UserControl
{
    private bool isPasswordField = false;

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public string DisplayName
    {
        get
        {
            return this.lblDisplayName.Text;
        }
        set
        {
            this.lblDisplayName.Text = value;
        }
    }

    public string LabelName
    {
        get
        {
            return this.lblName.Value;
        }
        set
        {
            this.lblName.Value = value;
        }
    }

    public string TextValue
    {
        get
        {
            return this.tValue.Text;
        }
        set
        {
            this.tValue.Text = value;
        }
    }

    public string ToolTip
    {
        get
        {
            return this.tValue.ToolTip;
        }
        set
        {
            this.tValue.ToolTip = value;
        }
    }

    public bool IsPasswordField
    {
        get
        {
            return this.isPasswordField;
        }
        set
        {
            isPasswordField = value;

            if (isPasswordField)
            {
                this.tValue.TextMode = TextBoxMode.Password;
            }
        }
    }
}
