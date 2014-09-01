using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Web.Configuration;

namespace BCC.Core
{
    public class BCCMailer
    {
        private string category = "BCCMailer";
        private string hostName = string.Empty;
        private int portNumber = 25;
        private bool enableSSL = false;
        private string userPwd = string.Empty;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="host"></param>
        public BCCMailer(string host)
        {
            this.hostName = host;
        }

        /// <summary>
        /// Constructor for the class
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="enableSSL"></param>
        public BCCMailer(string host, int port, bool enableSSL)
        {
            this.hostName = host;
            this.portNumber = port;
            this.enableSSL = enableSSL;
        }

        /// <summary>
        /// Set the User password for email.
        /// </summary>
        public string Password
        {
            set
            {
                this.userPwd = value;
            }
        }

        /// <summary>
        /// Send email with Attachments, can be set to NULL, in case of no attachments.
        /// </summary>
        /// <param name="from">User Name</param>
        /// <param name="toList">Comma separated To list</param>
        /// <param name="subject">Email Subject</param>
        /// <param name="body">Email body</param>
        /// <param name="isBodyHtml">Specify if you want to HTML content</param>
        /// <param name="attachmentCollection">Attachments</param>
        public void SendMail(string from, string toUserList, string subject, string body, bool isBodyHtml, AttachmentCollection attachmentCollection)
        {
            try
            {
                // Create mail message
                MailMessage message = new MailMessage();
                // Get the default from address if the from address is null
                message.From = new MailAddress(from);

                if (toUserList != null && toUserList != string.Empty)
                {
                    // Add recipients to from to list
                    foreach (string to in toUserList.Split(','))
                    {
                        message.To.Add(to);
                    }
                }
                else
                {
                    throw new Exception("Calls must specify TO user email id.");
                }

                message.Body = body;
                message.Subject = subject;
                message.IsBodyHtml = isBodyHtml;

                if (attachmentCollection != null)
                {
                    foreach (Attachment fileAttachment in attachmentCollection)
                    {
                        message.Attachments.Add(fileAttachment);
                    }
                }

                // Create smpt client instance
                SmtpClient mailClient = new SmtpClient(this.hostName, this.portNumber);
                mailClient.EnableSsl = this.enableSSL;
                mailClient.Send(message);

                System.Diagnostics.Debug.Write("Email sent to " + toUserList.ToString(), category);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.Write("Exception:" + exception.Message + exception.StackTrace, category);
            }
        }

        /// <summary>
        /// Send email with no attachments, but HTML content only.
        /// It uses user password
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public void SendMail(string from, string toUserList, string subject, string body)
        {
            try
            {
                var client = new SmtpClient(this.hostName, this.portNumber)
                {
                    Credentials = new NetworkCredential(from, this.userPwd),
                    EnableSsl = this.enableSSL
                };

                MailMessage mailMsg = new MailMessage();

                mailMsg.From = new MailAddress(from);

                if (toUserList != null && toUserList != string.Empty)
                {
                    // Add recipients to from to list
                    foreach (string to in toUserList.Split(','))
                    {
                        mailMsg.To.Add(to);
                    }
                }
                else
                {
                    throw new Exception("Calls must specify TO user email id.");
                }

                mailMsg.Subject = subject;
                mailMsg.Body = body;
                mailMsg.IsBodyHtml = true;

                client.Send(mailMsg);

                System.Diagnostics.Debug.Write("Email sent to " + toUserList.ToString(), category);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.Write("Exception:" + exception.Message + exception.StackTrace, category);
            }
        }

        /// <summary>
        /// Send email with no attachments, but HTML content only.
        /// It uses user password
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public void SendSMTPMail(string from, string toUserList, string subject, string body)
        {
            try
            {
                // Create mail message
                MailMessage message = new MailMessage();
                message.From = new MailAddress(from);

                // Add recipients to from to list
                foreach (string to in toUserList.Split(','))
                {
                    message.To.Add(to);
                }

                message.Body = body;
                message.Subject = subject;
                message.IsBodyHtml = true;

                // Create SMTP client instance
                SmtpClient mailClient = new SmtpClient(this.hostName);
                mailClient.Send(message);

                System.Diagnostics.Debug.Write("Email sent to " + toUserList.ToString(), category);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.Write("Exception:" + exception.Message + exception.StackTrace, category);
            }
        }
    }
}
