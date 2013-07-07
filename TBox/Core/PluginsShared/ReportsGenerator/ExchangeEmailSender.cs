using System;
using Mnk.Library.Common.Communications;
using Microsoft.Exchange.WebServices.Data;

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
            var message = new EmailMessage(
                new ExchangeService(ExchangeVersion.Exchange2010)
                {
                    Url = new Uri(exchangeServer),
                    Credentials = new WebCredentials(login, password)
                });
            foreach (var email in recipients)
            {
                message.ToRecipients.Add(email);
            }
			message.Subject = subject;
			message.Body = body;
            if(isHtml)message.Body.BodyType = BodyType.HTML;
            message.SendAndSaveCopy();
		}
	}
}
