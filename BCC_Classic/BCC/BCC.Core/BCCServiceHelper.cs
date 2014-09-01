using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text; 

namespace BCC.Core
{
    class BCCServiceHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static bool IsInstalled(string serviceName) 
        { 
            using (ServiceController controller = new ServiceController(serviceName)) 
            { 
                try 
                { 
                    ServiceControllerStatus status = controller.Status; 
                } 
                catch 
                { 
                    return false; 
                } 
                return true; 
            } 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static bool IsRunning(string serviceName) 
        {
            using (ServiceController controller = new ServiceController(serviceName)) 
            {
                if (!IsInstalled(serviceName))
                {
                    return false;
                }
                else
                {
                    return (controller.Status == ServiceControllerStatus.Running);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static AssemblyInstaller GetInstaller(string serviceName)
        {
            //TODO: THis is not working
            AssemblyInstaller installer = new AssemblyInstaller();
            installer.UseNewContext = true;
            return installer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        public static void StartService(string serviceName)
        {
            if (!IsInstalled(serviceName))
            {
                return;
            }

            using (ServiceController controller = new ServiceController(serviceName)) 
            { 
                try 
                { 
                    if (controller.Status != ServiceControllerStatus.Running) 
                    { 
                        controller.Start(); 
                        controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10)); 
                    } 
                } 
                catch 
                { 
                    throw; 
                } 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        public static void StopService(string serviceName) 
        {
            if (!IsInstalled(serviceName))
            {
                return;
            }

            using (ServiceController controller = new ServiceController(serviceName)) 
            { 
                try 
                { 
                    if (controller.Status != ServiceControllerStatus.Stopped) 
                    { 
                        controller.Stop(); 
                        controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10)); 
                    } 
                } 
                catch 
                { 
                    throw; 
                } 
            }
        }
 
        public static void InstallService(string serviceName)     
        {         
            if (IsInstalled(serviceName)) 
            {
                return;
            }

            try
            {
                using (AssemblyInstaller installer = GetInstaller(serviceName))
                {
                    IDictionary state = new Hashtable();

                    try
                    {
                        installer.Install(state);
                        installer.Commit(state);
                    }
                    catch
                    {
                        try
                        {
                            installer.Rollback(state);
                        }
                        catch
                        {
                        }

                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }     
        } 
    }
}
