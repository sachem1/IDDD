using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Coralcode.Framework.ConfigManager;

namespace Coralcode.Framework.Utils
{
    public static class MailUtil
    {
        /// <summary>
        ///  邮件的发送
        /// </summary>
        /// <param name="toMail">收件人地址（可以是多个收件人，程序中是以“;"进行区分的）</param>
        /// <param name="subject">邮件标题</param>
        /// <param name="emailBody">邮件内容（可以以html格式进行设计）</param>
        /// <param name="attachments"></param>
        public static void Send(string toMail, string subject, string emailBody, List<Stream> attachments = null)
        {
            var maillAddress = AppConfig.DllConfigs.Current["Mail"]["MailAddress"];

            var message = new MailMessage
            {
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                Priority = MailPriority.Normal,
                Subject = subject,
                Body = emailBody
            };

            var fromMailAddress = new MailAddress(maillAddress, "中估联信息技术有限公司");
            message.From = fromMailAddress;

            var toMails = toMail.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var toMailAddress in toMails)
            {
                message.To.Add(toMailAddress);
            }

            if (attachments != null && attachments.Count != 0)
            {
                attachments.ForEach(item =>
                {
                    var attachment = new Attachment(item, MediaTypeNames.Application.Octet);
                    //var disposition = attachment.ContentDisposition;
                    //disposition.CreationDate = File.GetCreationTime(path[i]);
                    //disposition.ModificationDate = File.GetLastWriteTime(path[i]);
                    //disposition.ReadDate = File.GetLastAccessTime(path[i]);
                    message.Attachments.Add(attachment);

                });

            }
            var mSmtpClient = new SmtpClient()
            {
                Host = AppConfig.DllConfigs.Current["Mail"]["Host"],
                Port = Convert.ToInt32(AppConfig.DllConfigs.Current["Mail"]["Port"]),
                UseDefaultCredentials = true,
                EnableSsl = Convert.ToBoolean(AppConfig.DllConfigs.Current["Mail"]["EnableSsl"]),
            };
            if (Convert.ToBoolean(AppConfig.DllConfigs.Current["Mail"]["EnableAuthentication"]))
            {
                NetworkCredential nc = new NetworkCredential(AppConfig.DllConfigs.Current["Mail"]["UserName"], AppConfig.DllConfigs.Current["Mail"]["Password"]);
                mSmtpClient.Credentials = nc.GetCredential(mSmtpClient.Host, mSmtpClient.Port, "NTLM");
            }
            else
            {
                mSmtpClient.Credentials = new NetworkCredential(AppConfig.DllConfigs.Current["Mail"]["UserName"], AppConfig.DllConfigs.Current["Mail"]["Password"]);
            }
            mSmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            mSmtpClient.Send(message);

        }
    }
}
