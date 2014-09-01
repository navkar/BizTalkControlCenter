using System;
using System.Collections.Generic;
using System.Text;

namespace BCC.Core
{
    public class BCCServiceReportStruct
    {
        private string __orginator;
        private string __serviceName;
        private string __dtd;
        private string __wtd;
        private string __mtd;
        private string __ytd;
        private string __ttd;

        public BCCServiceReportStruct(string orginator, String serviceName, String dtd, string wtd, string mtd, string ytd, string ttd)
        {
            __orginator = orginator;
            __serviceName = serviceName;
            __dtd = dtd;
            __wtd = wtd;
            __mtd = mtd;
            __ytd = ytd;
            __ttd = ttd;
        }

        public String Orginator
        {
            get
            {
                return __orginator;
            }

            set
            {
                __orginator = value;
            }

        }
        public String ServiceName
        {
            get
            {
                return __serviceName;
            }

            set
            {
                __serviceName = value;
            }

        }
        public String DTD
        {
            get
            {
                return __dtd;
            }

            set
            {
                __dtd = value;
            }

        }
        public String WTD
        {
            get
            {
                return __wtd;
            }

            set
            {
                __wtd = value;
            }

        }
        public String MTD
        {
            get
            {
                return __mtd;
            }

            set
            {
                __mtd = value;
            }

        }
        public String YTD
        {
            get
            {
                return __ytd;
            }

            set
            {
                __ytd = value;
            }

        }
        public String TTD
        {
            get
            {
                return __ttd;
            }

            set
            {
                __ttd = value;
            }

        }
    }
}
