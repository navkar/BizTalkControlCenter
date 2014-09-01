namespace BCC.Agent
{
    partial class AgentInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BCCAgentProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.BizTalkControlCenterAgentInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // BCCAgentProcessInstaller
            // 
            this.BCCAgentProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.BCCAgentProcessInstaller.Password = null;
            this.BCCAgentProcessInstaller.Username = null;
            // 
            // BizTalkControlCenterAgentInstaller
            // 
            this.BizTalkControlCenterAgentInstaller.Description = "BizTalk Control Center (BCC) Agent is a service which is used to monitor BizTalk " +
                "Artifacts.";
            this.BizTalkControlCenterAgentInstaller.DisplayName = "BizTalk Control Center Agent";
            this.BizTalkControlCenterAgentInstaller.ServiceName = "BizTalkControlCenterAgent";
            this.BizTalkControlCenterAgentInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            this.BizTalkControlCenterAgentInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.BizTalkControlCenterAgentInstaller_AfterInstall);
            // 
            // AgentInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.BCCAgentProcessInstaller,
            this.BizTalkControlCenterAgentInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller BCCAgentProcessInstaller;
        private System.ServiceProcess.ServiceInstaller BizTalkControlCenterAgentInstaller;
    }
}