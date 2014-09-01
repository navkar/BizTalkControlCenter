using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;

namespace BCC.Controls
{
    public class WITGridActionList : DesignerActionList
    {
        //Linked grid control
        private WITGrid _linkedControl;

        //Constructor
        public WITGridActionList(WITGrid ctrl) : base(ctrl)
        {
            _linkedControl = ctrl;
        }

        //A property cannot be set directly because other VS2005 functions
        //have to be aware of its changes. Therefor GetPropertyName is created
        private PropertyDescriptor GetPropertyByName(string propName)
        {
            PropertyDescriptor prop;
            prop = TypeDescriptor.GetProperties(_linkedControl)[propName];

            if (prop == null)
                throw new ArgumentException("Property not found", propName);
            else
                return prop;
        }

        //Each property is a smart tag
        public string DataSourceID
        {
            get { return _linkedControl.DataSourceID; }
            set { GetPropertyByName("DataSourceID").SetValue(_linkedControl, value); }
        }

        public bool ShowDeleteColumn
        {
            get { return _linkedControl.ShowDeleteColumn; }
            set { GetPropertyByName("ShowDeleteColumn").SetValue(_linkedControl, value); }
        }

        public bool ShowEditColumn
        {
            get { return _linkedControl.ShowEditColumn; }
            set { GetPropertyByName("ShowEditColumn").SetValue(_linkedControl, value); }
        }

        public bool ShowOrderColumn
        {
            get { return _linkedControl.ShowOrderColumn; }
            set { GetPropertyByName("ShowOrderColumn").SetValue(_linkedControl, value); }
        }

        //This function adds the smart tags and headers to the smart tag list
        public override DesignerActionItemCollection GetSortedActionItems()
        {

            DesignerActionItemCollection items = new DesignerActionItemCollection();

            //Header
            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionHeaderItem("Data"));

            //Items
            items.Add(new DesignerActionPropertyItem("ShowDeleteColumn", "Show delete column.", "Appearance", "Show delete column"));
            items.Add(new DesignerActionPropertyItem("ShowEditColumn", "Show Edit column.", "Appearance", "Show Edit column"));
            items.Add(new DesignerActionPropertyItem("ShowOrderColumn", "Show Order column.", "Appearance", "Show Order column"));
            items.Add(new DesignerActionPropertyItem("DataSourceID", "DataSourceID", "Data", "Link to the data control"));

            return items;
        }
    }

}