using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

public partial class Controls_CollectionView : System.Web.UI.UserControl
{
    private StringCollection source = null;
    private string keyName = string.Empty;

    public delegate void OnCollectionViewEvent(object sender, CollectionViewEventArgs e);
    public event OnCollectionViewEvent CollectionViewEvent;

    public StringCollection Source
    {
        get
        {
            return this.source;
        }
        set
        {
            this.source = value;
        }
    }

    public object ControlDataSource
    {
        get
        {
            return collectionView.DataSource;
        }
        set
        {
            collectionView.DataSource = value;
        }
    }

    public string KeyName
    {
        get
        {
            return ViewState["Controls_CollectionView.KeyName"] as string;
        }
        set
        {
            this.keyName = value;
            ViewState["Controls_CollectionView.KeyName"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            collectionView.DataBind();
        }
    }

    protected void collectionView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        // Set CurrentPageIndex to the page the user clicked.
        collectionView.PageIndex = e.NewPageIndex;
    }

    protected void collectionView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "HighlightON(this);");
            e.Row.Attributes.Add("onmouseout", "HighlightOFF(this);");

            collectionView.HeaderRow.Cells[0].Text = KeyName;
        }
    }

    private void WriteToEventLog(string message)
    {
        string category = "User Control";

        System.Diagnostics.EventLog.WriteEntry(category, message);
        System.Diagnostics.Debug.Write(message, category);
    }

    protected void collectionView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Add")
        {
           TextBox tbCollectionItem = collectionView.FooterRow.FindControl("tbCollectionItem") as TextBox;

           if (tbCollectionItem != null)
           {
               // Generate an event with 
               // ItemName, KeyName, OperationCode (Add, Remove)
               if (CollectionViewEvent != null)
               {
                   CollectionViewEventArgs args = new CollectionViewEventArgs(KeyName, tbCollectionItem.Text, "Add");
                   CollectionViewEvent(this, args);
               }
           }
        }
        else if (e.CommandName == "Remove")
        {
            GridViewRow selectedRow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);

            if (selectedRow != null)
            {
                Label lblCollectionItem = selectedRow.FindControl("lblCollectionItem") as Label;

                if (lblCollectionItem != null)
                {
                    CollectionViewEventArgs args = new CollectionViewEventArgs(KeyName, lblCollectionItem.Text, "Remove");
                    CollectionViewEvent(this, args);
                }
            }
        }
        else if (e.CommandName == "AddOnEmpty")
        {
            TextBox tbEmptyInsert = collectionView.Controls[0].Controls[0].FindControl("tbEmptyInsert") as TextBox;

            if (tbEmptyInsert != null)
            {
                if (CollectionViewEvent != null)
                {
                    CollectionViewEventArgs args = new CollectionViewEventArgs(KeyName, tbEmptyInsert.Text, "AddOnEmpty");
                    CollectionViewEvent(this, args);
                }
            }
        }
    }

}

public class CollectionViewEventArgs : EventArgs
{
    private string keyName = string.Empty;
    private string itemName = string.Empty;
    private string operationCode = string.Empty;

    public CollectionViewEventArgs(string keyName, string itemName, string operationCode)
    {
        this.keyName = keyName;
        this.itemName = itemName;
        this.operationCode = operationCode;
    }

    public string KeyName
    {
        get
        {
            return this.keyName;
        }

        set
        {
            this.keyName = value;
        }
    }

    public string ItemName
    {
        get
        {
            return this.itemName;
        }

        set
        {
            this.itemName = value;
        }
    }

    public string OperationCode
    {
        get
        {
            return this.operationCode;
        }

        set
        {
            this.operationCode = value;
        }
    }
}