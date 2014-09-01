using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Configuration;


namespace BCC.Agent
{
    [RunInstaller(true)]
    public partial class AgentInstaller : Installer
    {
        private string serviceUserName = string.Empty;
        private string servicePassword = string.Empty;
        
        public AgentInstaller(): base()
        {
            InitializeComponent();
        }

         /// <summary>
        /// Automatically start the BCC Agent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BizTalkControlCenterAgentInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            using (var serviceController =
                new ServiceController(this.BizTalkControlCenterAgentInstaller.ServiceName, Environment.MachineName))
            {
                try
                {
                    serviceController.Start();
                    Context.LogMessage("Starting the service controller.");
                }
                catch (Exception exception)
                {
                    System.Diagnostics.EventLog.WriteEntry("BCC-AgentInstaller", exception.Message + exception.StackTrace);
                }
            }
        }

        //private void AddConnectionString(string targetDirectory, string databaseName, string serverName)
        //{
        //    // Open Application's App.Config
        //    Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(targetDirectory + @"BCCAgentShared\BCC.Agent.exe");
        //    string connectionString = "Integrated Security=SSPI;database=" + databaseName + ";server=" + serverName;

        //    // Add new connection string setting for App.config
        //    ConnectionStringSettings appDatabase = new ConnectionStringSettings();
        //    appDatabase.Name = "authStore";
        //    appDatabase.ConnectionString = connectionString;
        //    config.ConnectionStrings.ConnectionStrings.Clear();
        //    config.ConnectionStrings.ConnectionStrings.Add(appDatabase);

        //    // Persist App.config settings
        //    config.Save();
        //}

    }
}
