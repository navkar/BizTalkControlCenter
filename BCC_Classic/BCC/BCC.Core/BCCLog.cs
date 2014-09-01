using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core
{
    public class BCCLog
    {
        private string __logId;
        private string __severity;
        private string __timeStamp;
        private string __machineName;
        private string __title;
        private string __appDomainName;
        private string __logMessage;

        public BCCLog(string logId, string severity, string timeStamp, 
            string machineName,string title, string appDomainName, string logMessage)
        {
        __logId = logId;
        __severity = severity;
        __timeStamp = timeStamp;
        __machineName = machineName;
        __title = title;
        __appDomainName = appDomainName;
        __logMessage = logMessage;
        }

        public String LogId
        {
            get
            {
                return __logId;
            }

            set
            {
                __logId = value;
            }

        }

        public String Severity
        {
            get
            {
                return __severity;
            }

            set
            {
                __severity = value;
            }

        }

        public String LogTimestamp
        {
            get
            {
                return __timeStamp;
            }

            set
            {
                __timeStamp = value;
            }

        }

        public String Title
        {
            get
            {
                return __title;
            }

            set
            {
                __title = value;
            }

        }

        public String MachineName
        {
            get
            {
                return __machineName;
            }

            set
            {
               __machineName = value;
            }

        }
        public String AppDomainName
        {
            get
            {
                return __appDomainName;
            }

            set
            {
                __appDomainName = value;
            }
        }

        public String LogMessage
        {
            get
            {
                return __logMessage;
            }

            set
            {
                __logMessage = value;
            }
        }

    }
}
