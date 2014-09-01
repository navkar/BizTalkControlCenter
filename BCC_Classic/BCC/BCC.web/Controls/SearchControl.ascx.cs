using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

public partial class SearchUserControl : System.Web.UI.UserControl
{
    private string searchKeyword = string.Empty;
    private string keywordLabel = string.Empty;
    public event EventHandler SearchClick;

    protected void OnSearchClick(EventArgs e)
    {
        if (SearchClick != null)
        {
            SearchClick(this, e);
        }
    }

    public string KeywordLabel
    {
        get
        {
            return keywordLabel;
        }

        set
        {
            keywordLabel = value;
        }
    }

    public string SearchKeyword
    {
        get
        {
            return searchKeyword;
        }

        set
        {
            searchKeyword = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            txtSearch.Text = SearchKeyword;
        }

        lblSearchKeywords.Font.Bold = false;
        lblSearchKeywords.Text = keywordLabel;
    }

    override protected void OnInit(EventArgs e)
    {
        InitializeComponent();
        base.OnInit(e);
    }

    private void InitializeComponent()
    {
        this.lnkSearch.Click += new System.EventHandler(this.lnkSearch_Click);
        this.Load += new System.EventHandler(this.Page_Load);
    }

    protected void lnkSearch_Click(object sender, EventArgs e)
    {
        SearchKeyword = txtSearch.Text;
        OnSearchClick(e);
    }
}
