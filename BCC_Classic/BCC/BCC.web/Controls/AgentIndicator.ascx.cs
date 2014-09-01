using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BCC.Core;

public partial class Controls_AgentIndicator : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        BCCAgentIndicator();
    }

    private void BCCAgentIndicator()
    {
        try
        {
            StringCollection serviceList = new StringCollection();
            serviceList.Add(ConfigurationManager.AppSettings["BCCAgentName"].ToString());

            if (serviceList != null && serviceList.Count > 0)
            {
                BCCOperator bccOperator = new BCCOperator();
                DataTable dtService = bccOperator.GetServiceStatus(serviceList);

                if (dtService != null && dtService.Rows != null && dtService.Rows.Count > 0)
                {
                    string agent = "BCC Agent";
                    agentName.Text = agent;
                    agentStatus.Status = dtService.Rows[0][1].ToString();
                    agentStatus.ToolTip = dtService.Rows[0][1].ToString();
                    agentName.ToolTip = agent + " - " + dtService.Rows[0][1].ToString();
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.Write(ex.Message + ex.StackTrace, "Controls_AgentIndicator");
        }
    }
}
