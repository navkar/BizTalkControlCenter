using System;
using System.Web.Management;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCC.Core
{
    /// <summary>
    /// Web Audit Event
    /// </summary>
    public class BCCWebAuditEvent : WebAuditEvent   
    {
        private ActivityData activityData = null;

        /// <summary>
        /// Invoked in case of events identified only by their event code.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="eventSource"></param>
        /// <param name="eventCode"></param>
        /// <param name="activityData"></param>
        public BCCWebAuditEvent(string msg, object eventSource, int eventCode, ActivityData activityData): base(msg, eventSource, eventCode)
        {
            this.activityData = activityData;
        }

        public ActivityData CustomActivityData
        {
            get
            {
                return activityData;
            }

            set
            {
                activityData = value;
            }
        }

        /// <summary>
        /// Raise Event
        /// </summary>
        public override void Raise()
        {
            WebAuditEvent.Raise(this);
        }

        /// <summary>
        /// Obtains the current thread information.
        /// </summary>
        /// <returns></returns>
        public WebRequestInformation GetRequestInformation()
        {
            // Obtain the Web request information.
            // No customization is allowed here.
            return RequestInformation;
        }

     }
}
