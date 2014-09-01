using System;
using System.ComponentModel;
using System.Web.UI;
using System.Collections;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;

namespace BCC.Controls
{
    public class CheckBoxField : BoundField
    {
        #region " Properties "

        #endregion

        #region " Constructors "
        public CheckBoxField() { }
        public CheckBoxField(string id, string headerText, string dataField)
        {
            ID = id;
            HeaderText = headerText;
            DataField = dataField;
        }
        #endregion
    }
}