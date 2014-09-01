using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BCC.Core;
using System.Threading;
using System.Drawing;
using System.Collections.Specialized;

public partial class IFrame : System.Web.UI.Page
{
    IFrame.DataHelper data = new IFrame.DataHelper();
    BCCDataAccess dataAccess = new BCCDataAccess();
    public DataTable dt;
    public string chartData = "1,1,1,1,1,1,1,1,1,1";
    public string activeMsgCount = "0", suspendedMsgCount = "0", suspendedNRMsgCount = "0", dehydratedMsgCount = "0";
    public string strMsgCount, strMsgType, strMsgTypeColour;
    public int UpperLimitValue = 700;

    protected void Page_PreInit(object sender, EventArgs e)
    {
        string theme = Session["PAGE_THEME"] as string;

        if (theme != null)
        {
            this.Page.Theme = theme;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string filterExpr = "";

            if (Session["CHART_DATA"] != null)
            {
                data = Session["CHART_DATA"] as IFrame.DataHelper;
            }
            else
            {
                // Initialize the chart at one go! 
                data.Add(0);
                data.Add(0);
                data.Add(0);
                data.Add(0);
                data.Add(0);
                data.Add(0);
                data.Add(0);
                data.Add(0);
                data.Add(0);
                data.Add(0);
                Session["CHART_DATA"] = data;
            }

            string upperLimit = String.Empty;

            try
            {
                BCCModuleProperty props = Profile.ControlCenterProfile.ModuleFilter["102"];
                //upperLimit = props.ModuleKeys[BCCUIHelper.Constants.SC102_UPPER_LIMIT_VALUE];

                if (props != null)
                {
                    StringCollection valueCollection = props.ModuleDictionary[BCCUIHelper.Constants.SC102_UPPER_LIMIT_VALUE];

                    if (valueCollection != null && valueCollection.Count > 0)
                    {
                        upperLimit = valueCollection[0];
                    }
                }

            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.Write(exception.Message, "102");
            }

            if (upperLimit != String.Empty && upperLimit.Length > 0)
            {
                Int32.TryParse(upperLimit, out UpperLimitValue);
            }

            if (!IsPostBack && Convert.ToString(Session["lbStatus"]) == "Refresh on")
            {
                RefreshTimer.Interval = 86400000;
                lbRefresh.Text = "Refresh on";
            }

            dt = dataAccess.RetrieveAllMessages(UpperLimitValue);

            if (dt != null && dt.Rows != null)
            {
                data.Add(dt.Rows.Count);
            }

            chartData = data.ToChart();

            if (dt.Rows.Count > 0)
            {
                filterExpr = "Status = 'Active'";
                activeMsgCount = dt.Select(filterExpr).Length + "";

                filterExpr = "Status = 'Suspended'";
                suspendedMsgCount = dt.Select(filterExpr).Length + "";

                filterExpr = "Status = 'SuspendedNotResumable'";
                suspendedNRMsgCount = dt.Select(filterExpr).Length + "";

                filterExpr = "Status = 'Dehydrated'";
                dehydratedMsgCount = dt.Select(filterExpr).Length + "";
            }

            strMsgCount = (activeMsgCount == "0" ? "" : activeMsgCount + ",") +
                   (suspendedMsgCount == "0" ? "" : suspendedMsgCount + ",") +
                   (suspendedNRMsgCount == "0" ? "" : suspendedNRMsgCount + ",") +
                   (dehydratedMsgCount == "0" ? "" : dehydratedMsgCount);
            if (strMsgCount.EndsWith(","))
            {
                strMsgCount = strMsgCount.Remove(strMsgCount.Length - 1);
            }


            strMsgType = (activeMsgCount == "0" ? "" : "Active-" + activeMsgCount + "|") +
                   (suspendedMsgCount == "0" ? "" : "Suspended(R)-" + suspendedMsgCount + "|") +
                   (suspendedNRMsgCount == "0" ? "" : "Suspended(NR)-" + suspendedNRMsgCount + "|") +
                   (dehydratedMsgCount == "0" ? "" : "Dehydrated-" + dehydratedMsgCount);

            if (strMsgType.EndsWith("|"))
            {
                strMsgType = strMsgType.Remove(strMsgType.Length - 1);
            }

            strMsgTypeColour = (activeMsgCount == "0" ? "" : "00FF00,") +
                   (suspendedMsgCount == "0" ? "" : "FF9900,") +
                   (suspendedNRMsgCount == "0" ? "" : "FF1A00,") +
                   (dehydratedMsgCount == "0" ? "" : "FFFF00");

            if (strMsgTypeColour.EndsWith(","))
            {
                strMsgTypeColour = strMsgTypeColour.Remove(strMsgTypeColour.Length - 1);
            }
        }
        catch
        {
            chartPanel.Visible = false;
            errorPanel.Visible = true;
            lblError.Font.Name = "Verdana";
            lblError.Font.Size = FontUnit.Smaller;
            lblError.Text = "Session has become dirty, restart application pool to restore it back. ";
        }

    }

    protected void ToggleRefresh(object sender, EventArgs e)
    {
        if (Session["lbStatus"] == null)
        {
            Session["lbStatus"] = "Refresh off";
        }

        if (Convert.ToString(Session["lbStatus"]) == "Refresh off")
        {
            //RefreshTimer.Enabled = false;
            RefreshTimer.Interval = 86400000;
            lbRefresh.Text = "Refresh on";
            Session["lbStatus"] = "Refresh on";
        }
        else
        {
            //RefreshTimer.Enabled = true;
            RefreshTimer.Interval = 5000;
            lbRefresh.Text = "Refresh off";
            Session["lbStatus"] = "Refresh off";
        }

    }

    protected void RefreshTimer_Tick(object sender, EventArgs e)
    {
        if (Session["lbStatus"] == null)
        {
            Session["lbStatus"] = "Refresh off";
        }

        if (Convert.ToString(Session["lbStatus"]) == "Refresh off")
        {
            lbRefresh.Text = "Pass";
            UpdatePanel1.Update();
        }
    }

    public class DataHelper
    {
        ArrayList array = null;

        public DataHelper()
        {
            array = new ArrayList();
        }

        public void Add(int data)
        {

            array.Add(data + "");

            if (array.Count > 10)
            {
                array.RemoveAt(0); //
            }
        }

        public string ToChart()
        {
            string chartData = "";

            foreach (string data in array)
            {
                chartData = chartData + data + ",";
            }

            return chartData.Substring(0, chartData.Length - 1);
        }

    }

}

