using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace BCC.Controls
{
    //WITGridRow will contain one row of (grid) information.
    //When the grid is build, a RowDataBound event is triggered each time
    //a row is completed. The developer can read and/or change the information
    //in this event. After the event, the row is finally added to the grid
    public class WITGridRow : IDataItemContainer
    {
        public WITGridRow()
        { }

        public WITGridRow(object dataItem, int Index)
        {
            _dataItem = dataItem;
            _dataItemIndex = Index;
        }

        //Container with cells
        private TableRow _container;
        public TableCellCollection Cells
        {
            get
            {
                if (_container == null)
                    _container = new TableRow();
                return _container.Cells;
            }
        }

        //The row's data item
        private object _dataItem;
        private int _dataItemIndex;

        public object DataItem
        {
            get { return _dataItem; }
            set { _dataItem = value; }
        }

        //The data item's index
        public int DataItemIndex
        {
            get { return _dataItemIndex; }
            set { _dataItemIndex = value; }
        }

        public int DisplayIndex
        { get { return 0; } }
    }

    //WITGridRowEventArgs contains the event arguments
    //for the RowDataBound event
    public class WITGridRowEventArgs : EventArgs
    {
        //The WITGridRow
        private WITGridRow _row;
        public WITGridRow Row
        {
            get { if (_row == null) _row = new WITGridRow(); return _row; }
            set { _row = value; }
        }
        //Constructor
        public WITGridRowEventArgs(WITGridRow row)
        {
            _row = row;
        }
    }

    //WITGridRowCommandEventsArgs contains the event arguments for
    //the RowCommand event and contains the selected row, related key and command name
    public class WITGridRowCommandEventsArgs : EventArgs
    {
        private int _selectedRow;
        private string _selectedKey = "";
        private string _commandName = "";

        public WITGridRowCommandEventsArgs(int selectedRow, string selectedKey, string commandName)
        {
            _selectedRow = selectedRow;
            _selectedKey = selectedKey;
            _commandName = commandName;
        }

        public int SelectedRow
        {
            get { return _selectedRow; }
            set { _selectedRow = value; }
        }

        public string SelectedKey
        {
            get { return _selectedKey; }
            set { _selectedKey = value; }
        }

        public string CommandName
        {
            get { return _commandName; }
            set { _commandName = value; }
        }
    }
}
