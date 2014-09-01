using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core
{
    public class BCCReportScheduleStruct
    {
        private string __hubRepSchdId;
        private string __reportName;
        private string __scheduleType;
        private string __year;
        private string __month;
        private string __day;
        private string __hh;
        private string __mi;
        private string __status;

        public BCCReportScheduleStruct(string hubRepSchdId, String reportName, String scheduleType, string year, string month, string day, string hh, string mi, string status)
        {
            __hubRepSchdId = hubRepSchdId;
            __reportName = reportName;
            __scheduleType = scheduleType;
            __year = year;
            __month = month;
            __day = day;
            __hh = hh;
            __mi = mi;
            __status = status;
        }

        public String HubRepSchdId
        {
            get
            {
                return __hubRepSchdId;
            }

            set
            {
                __hubRepSchdId = value;
            }

        }
        public String ReportName
        {
            get
            {
                return __reportName;
            }

            set
            {
                __reportName = value;
            }

        }
        public String ScheduleType
        {
            get
            {
                return __scheduleType;
            }

            set
            {
                __scheduleType = value;
            }

        }
        public String Year
        {
            get
            {
                return __year;
            }

            set
            {
                __year = value;
            }

        }
        public String Month
        {
            get
            {
                return __month;
            }

            set
            {
                __month = value;
            }

        }
        public String Day
        {
            get
            {
                return __day;
            }

            set
            {
                __day = value;
            }

        }
        public String HH
        {
            get
            {
                return __hh;
            }

            set
            {
                __hh = value;
            }

        }
        public String MI
        {
            get
            {
                return __mi;
            }

            set
            {
                __mi = value;
            }

        }

        public String Status
        {
            get
            {
                return __status;
            }

            set
            {
                __status = value;
            }

        }
    }
}
