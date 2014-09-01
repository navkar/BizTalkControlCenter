using System;
using System.Data;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BCC.Controls
{
    [Designer(typeof(WITGridDesigner))]
    [ParseChildren(true, "GridFields")]
    public class WITGrid : CompositeDataBoundControl, IPostBackEventHandler
    {
        #region private data
        //-----------------------------  Private data --------------------------------------//
        private Collection<GridField> _gridFields;  //Inner elements
        private bool _highLightSelectedRow;     //If the row should be highlighted
        private string _dataKeyName = "";       //The field name containing the key
        private bool _showEditColumn = false;  //Up and down icon
        private bool _showOrderColumn = false;  //Up and down icon
        private bool _showDeleteColumn = false; //Delete icon
        private string _noRecordsText = "No records where found.";  //Text displayed when no records where found
        private string _confirmDeleteText = "This record and all related data will be deleted. Continue?";
        private string _emptyGridCssClass = "GridView-Empty";
        private string _highlightCssClass = "GridView-Highlight";
        private string _checkboxCssClass = "FormCheckbox";
        private string _cssClass = "GridView";
        #endregion

        #region Events
        //-----------------------------  Events --------------------------------------//
        public event EventHandler<WITGridRowCommandEventsArgs> RowCommand;
        public event EventHandler<WITGridRowEventArgs> RowDataBound;
        #endregion

        #region Properties
        //-----------------------------  Properties --------------------------------------//

        //A collection of the inner elements (BoundField, CheckBoxField, etc)
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [ReadOnly(true)]
        public Collection<GridField> GridFields
        {
            get
            {
                if (_gridFields == null)
                    _gridFields = new Collection<GridField>();
                return _gridFields;
            }
        }

        //Highlight the selected row (hovering over it with the mouse pointer)
        [Category("Appearance")]
        [Description("Highlight the row")]
        [DefaultValue("true")]
        public bool HighLightSelectedRow
        {
            get { return _highLightSelectedRow; }
            set { _highLightSelectedRow = value; }
        }

        // The name of the data field containg the key.This key will be returned in the RowCommand-event
        [Category("Data")]
        [Description("The data field containg the key")]
        [DesignOnly(true)]
        public string DataKeyName
        { set { _dataKeyName = value; } }

        //Show the edit column
        [Category("Data")]
        [Description("Show the edit column")]
        [DefaultValue("false")]
        public bool ShowEditColumn
        { get { return _showEditColumn; } set { _showEditColumn = value; } }

        //Show the order column
        [Category("Data")]
        [Description("Show the order column")]
        [DefaultValue("false")]
        public bool ShowOrderColumn
        { get { return _showOrderColumn; } set { _showOrderColumn = value; } }

        //Show the delete column
        [Category("Data")]
        [Description("Show the delete column")]
        [DefaultValue("false")]
        public bool ShowDeleteColumn
        { get { return _showDeleteColumn; } set { _showDeleteColumn = value; } }

        //The text displayed when no records are available
        [Category("Appearance")]
        [Description("The text displayed when no records are available")]
        [DefaultValue("No records where found.")]
        public string NoRecordsText
        { get { return _noRecordsText; } set { _noRecordsText = value; } }

        //The text displayed asking the user for confirmation (delete action)
        [Category("Appearance")]
        [Description("The text displayed when asking the user to confirm the deletion of a record.")]
        public string ConfirmDeleteText
        { get { return _confirmDeleteText; } set { _confirmDeleteText = value; } }

        [Category("Appearance")]
        [Description("The style of the message when no data is available")]
        [DefaultValue("GridView-Empty")]
        public string EmptyGridCssClass
        {
            get { return _emptyGridCssClass; }
            set { _emptyGridCssClass = value; }
        }
        [Category("Appearance")]
        [Description("The style of the highlighted row.")]
        [DefaultValue("GridView-Highlight")]
        public string HighLightCssClass
        {
            get { return _highlightCssClass; }
            set { _highlightCssClass = value; }
        }

        [Category("Appearance")]
        [Description("The style of the grid.")]
        [DefaultValue("GridView")]
        public override string CssClass
        {
            get { return _cssClass; }
            set { _cssClass = value; }
        }
        [Category("Appearance")]
        [Description("The style of the checkbox column.")]
        [DefaultValue("FormCheckbox")]
        public string CheckBoxCssClass
        {
            get { return _checkboxCssClass; }
            set { _checkboxCssClass = value; }
        }
        #endregion

        #region local events
        //-----------------------------  Local Events --------------------------------------//

        //Build the grid
        protected override int CreateChildControls(IEnumerable dataSource, bool dataBinding)
        {
            //The IStateManager is not used, therefor the grid has to be 
            //build again in case of a PostBack (therefor the dataBinding property 
            //is not checked)

            //Disbale viewstate, otherwise the datasource/binding is lost after a 
            //postback and I have not implemented the viewstate (alle element  are lost)
            this.EnableViewState = false;

            int count = 0;  //Count the controls (return value)

            if (dataSource == null)
            {
                //No records found
                Label lb = new Label();
                lb.Text = _noRecordsText;
                if (_emptyGridCssClass != null) lb.CssClass = _emptyGridCssClass;
                Controls.Add(lb);
                count++;
            }
            else
            {
                //Records were found, build the grid
                Table tb = new Table();
                tb.CellPadding = 0;
                tb.CellSpacing = 0;
                if (_cssClass != null) tb.CssClass = CssClass;

                //Build header -----------------------------------------------------------
                TableHeaderRow thr = new TableHeaderRow();
                thr.TableSection = TableRowSection.TableHeader;
                TableHeaderCell thc;

                foreach (GridField gf in _gridFields)
                {
                    //A header cell for each field
                    thc = new TableHeaderCell();
                    if (gf.CssClass != null) thc.CssClass = gf.CssClass;
                    thc.Attributes["scope"] = "col";
                    if (gf.HeaderText != null) thc.Text = gf.HeaderText;
                    thr.Cells.Add(thc);
                }

                //Special columns
                if (_showDeleteColumn)
                {
                    thc = new TableHeaderCell();
                    thc.Text = "Delete";
                    thc.Style["text-align"] = "center";
                    thr.Cells.Add(thc);
                }

                if (_showEditColumn)
                {
                    thc = new TableHeaderCell();
                    thc.Text = "Edit";
                    thc.Style["text-align"] = "center";
                    thr.Cells.Add(thc);
                }

                if (_showOrderColumn)
                {
                    thc = new TableHeaderCell();
                    thc.Text = "Order";
                    thc.Style["text-align"] = "center";
                    thr.Cells.Add(thc);
                }

                tb.Rows.Add(thr);

                //Body -----------------------------------------------------------------------
                TableRow tr;
                TableCell tc;
                Image img;
                string rowData = "";
                string rowCancel = "";  //cancel row action (delete)

                //Build each new row in WITGridRow (instead of directly adding it 
                //to the table's row). Trigger the event RowDataBound to enable
                //the programmer to change or read the information before the grid is build.
                //Then build the grid.

                int row = 0;
                foreach (DataRowView dataitem in dataSource)
                {
                    //New custom row for RowDataBound-event
                    WITGridRow wgr = new WITGridRow(dataitem, row);

                    //Build per (data)row

                    tr = new TableRow();
                    tr.TableSection = TableRowSection.TableBody;
                    if (_highLightSelectedRow)
                    {
                        tr.Attributes["onmouseover"] = "this.className='" + _highlightCssClass + "';";
                        tr.Attributes["onmouseout"] = "this.className=''";
                        rowData = "row_" + row.ToString();
                        if (_dataKeyName != "") rowData += "_" + dataitem[_dataKeyName].ToString();
                        tr.Attributes["onclick"] = "javascript:" + this.Page.ClientScript.GetPostBackEventReference(this, rowData);
                    }

                    //Regular items
                    foreach (GridField gf in _gridFields)
                    {
                        //Build per column (data field)
                        tc = new TableCell();
                        count++;
                        tc.CssClass = gf.CssClass;

                        if (gf is CheckBoxField)  //Put checkbox before bound because checkbox is bound
                        {
                            CheckBoxField cbf = (CheckBoxField)gf;
                            if (cbf.DataField == null) throw (new NullReferenceException("The data field for '" + cbf.ID + "' is not defined."));
                            CheckBox cb = new CheckBox();
                            cb.Enabled = false;
                            cb.Checked = Convert.ToBoolean(((DataRowView)dataitem).Row[cbf.DataField]);
                            cb.CssClass = _checkboxCssClass;
                            cb.ID = cbf.ID;
                            cb.Text = "";
                            tc.Style["text-align"] = "center";
                            tb.EnableViewState = false;
                            tc.Controls.Add(cb);
                        }
                        else if (gf is BoundField)
                        {
                            BoundField bf = (BoundField)gf;
                            if (bf.DataField == null) throw (new NullReferenceException("The data field for '" + bf.ID + "' is not defined."));
                            tc.ID = gf.ID;
                            tc.Text = ((DataRowView)dataitem).Row[bf.DataField].ToString();
                        }
                        wgr.Cells.Add(tc);
                    }

                    //Special columns

                    if (_showDeleteColumn)
                    {
                        tc = new TableCell();
                        count++;
                        tc.Width = 50;
                        tc.Style["text-align"] = "center";
                        img = new Image();
                        img.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "BCC.Controls.CollectionGrid.Resources.delete.gif");
                        rowData = "delete_" + row.ToString();
                        rowCancel = "cancel_" + row.ToString();  //to avoid a row click when the user cancels the delete
                        if (_dataKeyName != "") rowData += "_" + dataitem[_dataKeyName].ToString();
                        img.Attributes["onclick"] = "javascript:if (confirm('" + _confirmDeleteText + "')) { " + this.Page.ClientScript.GetPostBackEventReference(this, rowData) + " } " +
                                                                " else " + this.Page.ClientScript.GetPostBackEventReference(this, rowCancel);
                        tc.Controls.Add(img);
                        wgr.Cells.Add(tc);
                    }

                    if (_showEditColumn)
                    {
                        tc = new TableCell();
                        count++;
                        tc.Width = 30;
                        tc.Style["text-align"] = "center";
                        img = new Image();
                        img.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "BCC.Controls.CollectionGrid.Resources.pencil.gif");
                        rowData = "edit_" + row.ToString();
                        if (_dataKeyName != "") rowData += "_" + dataitem[_dataKeyName].ToString();
                        img.Attributes["onclick"] = "javascript:" + this.Page.ClientScript.GetPostBackEventReference(this, rowData);
                        tc.Controls.Add(img);
                        wgr.Cells.Add(tc);

                    }

                    if (_showOrderColumn)
                    {
                        //Up
                        tc = new TableCell();
                        count++;
                        tc.Width = 60;
                        tc.Style["text-align"] = "center";
                        img = new Image();
                        img.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "BCC.Controls.CollectionGrid.Resources.up.gif");
                        rowData = "up_" + row.ToString();
                        if (_dataKeyName != "") rowData += "_" + dataitem[_dataKeyName].ToString();
                        img.Attributes["onclick"] = "javascript:" + this.Page.ClientScript.GetPostBackEventReference(this, rowData);
                        tc.Controls.Add(img);
                        wgr.Cells.Add(tc);

                        //Down
                        img = new Image();
                        img.ImageUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "BCC.Controls.CollectionGrid.Resources.down.gif");
                        rowData = "down_" + row.ToString();
                        if (_dataKeyName != "") rowData += "_" + dataitem[_dataKeyName].ToString();
                        img.Attributes["onclick"] = "javascript:" + this.Page.ClientScript.GetPostBackEventReference(this, rowData);
                        tc.Controls.Add(img);
                        wgr.Cells.Add(tc);
                    }


                    //Trigger OnRowDataBound
                    if (RowDataBound != null)
                    {
                        //RowDataBound(this, new GridViewRowEventArgs(gvr));
                        RowDataBound(this, new WITGridRowEventArgs(wgr));
                    }

                    //Add the (changed) information to the table.
                    while (wgr.Cells.Count > 0)
                    {
                        //Adding the cell will empty the cell collection
                        tr.Cells.Add(wgr.Cells[0]);
                    }
                    tb.Rows.Add(tr);


                    row++;

                }
                Controls.Add(tb);
            }
            return count;
        }


        public void RaisePostBackEvent(String eventArgument)
        {
            //Handle the raised postback and determine what command (icon) was clicked

            int row = -1;
            string key = "";
            string command = "";

            //Get command
            int pos = eventArgument.IndexOf("_");
            command = eventArgument.Substring(0, pos);

            if (command.ToLower() != "cancel")
            {
                //Get row and key
                string temp = eventArgument.Substring(pos + 1);
                pos = temp.IndexOf("_");
                if (pos > 0)
                {
                    row = Convert.ToInt32(temp.Substring(0, pos));
                    key = temp.Substring(pos + 1);
                }
                else
                    row = Convert.ToInt32(temp);

                EventHandler<WITGridRowCommandEventsArgs> tempEvent = this.RowCommand;
                if (tempEvent != null)
                    tempEvent(this, new WITGridRowCommandEventsArgs(row, key, command));
            }
        }

        #endregion

    }
}