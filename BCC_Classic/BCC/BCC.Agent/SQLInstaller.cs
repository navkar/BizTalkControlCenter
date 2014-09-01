using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace BCC.Agent
{
    [RunInstaller(true)]
    public class SqlInstaller : System.Configuration.Install.Installer
    {
        public override void Install(IDictionary stateSaver)
        {
            string server = this.Context.Parameters["Server"];
            string password = this.Context.Parameters["Password"];
            string user = this.Context.Parameters["User"];
            string database = this.Context.Parameters["Database"];
            string sqlFile = this.Context.Parameters["SqlFile"];

            // Store these values in the saved state so they will be available to
            // us in the uninstall.
            stateSaver.Add("Server", server);
            stateSaver.Add("Password", password);
            stateSaver.Add("User", user);
            stateSaver.Add("Database", database);

            string commandLine = String.Format("-U {0} -P {1} -S {2} -i \"{3}\"",
                user, password, server, sqlFile);
            LaunchOSql(commandLine);

            base.Install(stateSaver);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            string server = (String)savedState["Server"];
            string password = (String)savedState["Password"];
            string user = (String)savedState["User"];
            string database = (String)savedState["Database"];
            string sqlFile = this.Context.Parameters["SqlFile"];

            string commandLine = String.Format("-U {0} -P {1} -S {2} -i \"{3}\"",
                user, password, server, sqlFile);
            LaunchOSql(commandLine);
        }

        private void LaunchOSql(String commandLine)
        {
            using (Process process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "osql.exe";
                process.StartInfo.Arguments = commandLine;
                process.Start();

                process.WaitForExit();
            }
        }
    }
}
