using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BCC.Core;
using BCC.Core.WMI.BizTalk;

namespace BCC.Agent
{
    class EmailHelper
    {
        private string _speedCode = string.Empty;
        private int smtp_email_port = 25;
        private bool smtp_email_ssl = false;
        private bool isWebEmail = false;
        private string smtp_email_host = string.Empty;
        private string smtp_email_username = string.Empty;
        private string smtp_email_userpwd = string.Empty;
        private string smtp_email_subject = string.Empty;
        private string smtp_email_rcpnt = string.Empty;
        private string smtp_email_title = "BizTalk Control Center (BCC) - Alert Notification";

        /// <summary>
        /// Loads the email configuration using speedcode.
        /// </summary>
        /// <param name="speedCode"></param>
        internal EmailHelper(string speedCode)
        {
            _speedCode = speedCode;

            LoadEmailConfiguration();
        }

        /// <summary>
        /// Override configuration 
        /// </summary>
        /// <param name="smtp_email_port"></param>
        /// <param name="smtp_email_ssl"></param>
        /// <param name="smtp_email_host"></param>
        /// <param name="smtp_email_username"></param>
        /// <param name="smtp_email_userpwd"></param>
        /// <param name="smtp_email_rcpnt"></param>
        /// <param name="smtp_email_title"></param>
        internal EmailHelper(int smtp_email_port, bool smtp_email_ssl, string smtp_email_host, 
            string smtp_email_username, string smtp_email_userpwd,
            string smtp_email_rcpnt, string smtp_email_title, bool isWebEmail)
        {
            this.smtp_email_host = smtp_email_host;
            this.smtp_email_port = smtp_email_port;
            this.smtp_email_rcpnt = smtp_email_rcpnt;
            this.smtp_email_ssl = smtp_email_ssl;
            this.smtp_email_title = smtp_email_title;
            this.smtp_email_username = smtp_email_username;
            this.smtp_email_userpwd = smtp_email_userpwd;
            this.isWebEmail = isWebEmail;
        }

        internal string SpeedCode
        {
            get
            {
                return _speedCode;
            }
        }

        /// <summary>
        /// Can be used with default constructor
        /// </summary>
        internal string EmailTitle
        {
            get
            {
                return smtp_email_title;
            }
        }

        /// <summary>
        /// Can be used with default constructor
        /// </summary>
        internal string EmailSubject
        {
            get
            {
                return smtp_email_subject;
            }
        }

        /// <summary>
        /// Can be used with default constructor
        /// </summary>
        internal bool IsWebEmail
        {
            get
            {
                return isWebEmail;
            }
            set
            {
                isWebEmail = value;
            }
        }

        /// <summary>
        /// Can be used with default constructor
        /// </summary>
        internal string EmailRecipient
        {
            get
            {
                return smtp_email_rcpnt;
            }
            set
            {
                smtp_email_rcpnt = value;
            }
        }

        private void LoadEmailConfiguration()
        {
            BCCManageConfigData configData = new BCCManageConfigData();
            configData.Speedcode = SpeedCode;
            configData.Query();

            NameValuePairSet configSet = configData.ConfigurationData;

            foreach (NameValuePair nvPair in configSet)
            {
                if (nvPair.Name.Equals(BCCUIHelper.Constants.SMTP_EMAIL_HOST))
                {
                    smtp_email_host = nvPair.Value;
                }
                else
                    if (nvPair.Name.Equals(BCCUIHelper.Constants.SMTP_EMAIL_PORT))
                    {
                        Int32.TryParse(nvPair.Value, out smtp_email_port);
                    }
                    else
                        if (nvPair.Name.Equals(BCCUIHelper.Constants.SMTP_EMAIL_RCPNT))
                        {
                            smtp_email_rcpnt = nvPair.Value;
                        }
                        else
                            if (nvPair.Name.Equals(BCCUIHelper.Constants.SMTP_EMAIL_SSL))
                            {
                                Boolean.TryParse(nvPair.Value, out smtp_email_ssl);
                            }
                            else
                                if (nvPair.Name.Equals(BCCUIHelper.Constants.SMTP_EMAIL_SUBJECT))
                                {
                                    smtp_email_subject = nvPair.Value;
                                }
                                else
                                    if (nvPair.Name.Equals(BCCUIHelper.Constants.SMTP_EMAIL_TITLE))
                                    {
                                        smtp_email_title = nvPair.Value;
                                    }
                                    else
                                        if (nvPair.Name.Equals(BCCUIHelper.Constants.SMTP_EMAIL_USERNAME))
                                        {
                                            smtp_email_username = nvPair.Value;
                                        }
                                        else
                                            if (nvPair.Name.Equals(BCCUIHelper.Constants.SMTP_EMAIL_USERPWD))
                                            {
                                                smtp_email_userpwd = nvPair.Value;
                                            }
                                            else
                                                if (nvPair.Name.Equals(BCCUIHelper.Constants.IS_WEB_EMAIL))
                                                {
                                                    Boolean.TryParse(nvPair.Value, out isWebEmail);
                                                }
            }
        }

        internal void SendMail(string mailSubject, string mailMessage, bool smtpEmailFlag)
        {
            if (!smtpEmailFlag)
            {
                BCCMailer bccAgent = new BCCMailer(smtp_email_host, smtp_email_port, smtp_email_ssl);
                
                if (IsWebEmail)
                {
                    bccAgent.Password = smtp_email_userpwd;
                }

                bccAgent.SendMail(smtp_email_username, smtp_email_rcpnt, mailSubject, mailMessage);
            }
            else
            {
                BCCMailer bccAgent = new BCCMailer(smtp_email_host);
                bccAgent.SendSMTPMail(smtp_email_username, smtp_email_rcpnt, mailSubject, mailMessage);
            }
        }

    }
}
