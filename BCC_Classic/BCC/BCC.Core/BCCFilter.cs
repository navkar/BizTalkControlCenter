using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for BCCFilter
/// </summary>
namespace BCC.Core
{
    public class BCCFilter
    {
        private string __flag;
        private string __name;
        private string __value;
        private string __description;
        private DateTime __createdOn;
        private string __createdBy;

        public BCCFilter(string flag, string name, string value, string description, DateTime createdOn, string createdBy)
        {
            __flag = flag;
            __name = name;
            __value = value;
            __description = description;
            __createdOn = createdOn;
            __createdBy = createdBy;
        }

        public BCCFilter(string name, string value)
        {
            __flag = null;
            __name = name;
            __value = value;
            __description = null;
            //__createdOn = new DateTime;
            __createdBy = null;
        }
        public DateTime CreatedOn
        {
            get
            {
                return __createdOn;
            }

            set
            {
                __createdOn = value;
            }
        }
        public string CreatedBy
        {
            get
            {
                return __createdBy;
            }

            set
            {
                __createdBy = value;
            }
        }

        public string Description
        {
            get
            {
                return __description;
            }

            set
            {
                __description = value;
            }
        }
        public string value
        {
            get
            {
                return __value;
            }

            set
            {
                __value = value;
            }
        }

        public string name
        {
            get
            {
                return __name;
            }

            set
            {
                __name = value;
            }
        }
        public string Flag
        {
            get
            {
                return __flag;
            }

            set
            {
                __flag = value;
            }
        }


    }
}

