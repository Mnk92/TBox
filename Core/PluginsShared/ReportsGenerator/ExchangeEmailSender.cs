using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Mnk.Library.Common.Communications;

namespace Mnk.TBox.Core.PluginsShared.ReportsGenerator
{
    public class ExchangeEmailSender : IEmailSender
    {
        private readonly string exchangeServer;
        private readonly string login;
        private readonly string password;

        public ExchangeEmailSender(string exchangeServer, string login, string password)
        {
            this.exchangeServer = exchangeServer;
            this.login = login;
            this.password = password;
        }

        public void Send(string subject, string body, bool isHtml, string[] recipients)
        {
            using var client = new SmtpClient(exchangeServer);
            client.Credentials = new NetworkCredential(login, password);
            using var message = new MailMessage(login, recipients.First(), subject, body);
            message.IsBodyHtml = true;
            foreach (var email in recipients.Skip(1))
            {
                message.ReplyToList.Add(email);
            }
            client.Send(message);
        }
    }
}
