using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BCC.Core;
using BCC.Core.WMI.BizTalk;

namespace SampleEvents
{
    public partial class _Default : System.Web.UI.Page
    {
        private BCCMonitoring x = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnDemo_Click(object sender, EventArgs e)
        {
            x = new BCCMonitoring(ArtifactType.HostInstance, "IHS_Receive", 10);
            //BCCMonitoring x = new BCCMonitoring(ArtifactType.ServiceInstance, "BizTalk Application 1", 10);
            x.EnableMonitoring();
            x.ArtifactStatusChanged += new ArtifactMonitoringEventHandler(x_PortStatusChanged);
        }

        protected void x_PortStatusChanged(object sender, ArtifactMonitoringEventArgs e)
        {
            lblInfo.Text = "Artifact Name: " + e.ArtifactName + " Status:" + e.ArtifactStatus;
            System.Diagnostics.Debug.Write(lblInfo.Text, "ASP.NET");
        }

    }
}
