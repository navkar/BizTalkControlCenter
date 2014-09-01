using System;
using System.ComponentModel;
using System.Web.UI;
using System.Collections;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;

namespace BCC.Controls
{
    //Gridfield
    //This class is used as a basis for BoundField, CheckBoxField, etc.
    public class GridField
    {
        #region " Properties "

        private string _id;
        [Category("Data")]
        [Description("The header text")]
        public string ID
        { get { return _id; } set { _id = value; } }

        private string _headerText;
        [Category("Data")]
        [Description("The header text")]
        public string HeaderText
        {
            get { return _headerText; }
            set { _headerText = value; }
        }

        private string _cssClass;
        [Category("Appearance")]
        [Description("The column or field style")]
        public string CssClass
        {
            get { return _cssClass; }
            set { _cssClass = value; }
        }
        #endregion

        #region " Constructors "
        public GridField() { }
        public GridField(string id, string headerText)
        {
            _id = id;
            _headerText = headerText;
        }
        #endregion
    }
}
