using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Mnk.Library.Common.Communications;
using Mnk.Library.Common.Log;

namespace Mnk.TBox.Tools.FeedbackService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly ILog log = LogManager.GetLogger<FeedbackController>();
        private readonly string toAddress;
        private readonly SmptEmailSender sender;
        public FeedbackController(IConfiguration configuration)
        {
            var section = configuration.GetSection("Feedback");
            var smtpServer = section.GetValue<string>("SmtpServer");
            var smtpServerPort = section.GetValue<int>("SmtpServerPort");
            var feedbackFromLogin = section.GetValue<string>("FeedbackFromLogin");
            var feedbackFromPassword = section.GetValue<string>("FeedbackFromPassword");
            toAddress = section.GetValue<string>("FeedbackToAddress");
            sender = new SmptEmailSender(smtpServer, smtpServerPort, feedbackFromLogin, feedbackFromPassword);
        }

        public class Message
        {
            public string Subject { get; set; }
            public string Body { get; set; }
        };

        [HttpPost("send")]
        public void Send(Message message)
        {
            log.Write($"Send: '{message.Subject}' # '{message.Body}'");
            try
            {
                sender.Send(message.Subject, message.Body, false, new[] { toAddress });
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't send email");
                throw;
            }
        }
    }
}
