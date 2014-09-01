using System;
using System.ComponentModel;
using System.Web.UI;
using System.Collections;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;

namespace BCC.Controls
{
    public class BoundField : GridField, IParserAccessor
    {
        #region " Properties "

        private string _dataField;
        [Category("Data")]
        [Description("The data field in the data source")]
        public string DataField
        {
            get { return _dataField; }
            set { _dataField = value; }
        }
        #endregion

        #region " Constructors "
        public BoundField() { }
        public BoundField(string id, string headerText, string dataField)
        {
            ID = id;
            HeaderText = headerText;
            _dataField = dataField;
        }
        #endregion

        #region IParserAccessor Members

        public void AddParsedSubObject(object obj)
        {
            if (obj is LiteralControl)
                this.DataField = ((LiteralControl)obj).Text;
            else if (obj is DataBoundLiteralControl)
                this.DataField = ((DataBoundLiteralControl)obj).Text;
            else
                throw new Exception("Error parsing inner text '" + this.ID + "'");
        }

        #endregion
    }
}
