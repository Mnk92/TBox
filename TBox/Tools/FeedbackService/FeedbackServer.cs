using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Mnk.Library.Common.Communications;
using Mnk.Library.Common.Log;

namespace Mnk.TBox.Tools.FeedbackService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = false)]
    class FeedbackServer : IFeedbackServer
    {
        private readonly ILog log = LogManager.GetLogger<FeedbacksService>();
        private readonly string toAddress;
        private readonly SmptEmailSender sender;
        public FeedbackServer(string smtpServer, int port, string login, string password, string toAddress)
        {
            this.toAddress = toAddress;
            sender = new SmptEmailSender(smtpServer, port, login, password);
        }

        public void Send(string subject, string body)
        {
            log.Write("Send: '{0}' # '{1}'", subject, body);
            try
            {
                sender.Send(subject, body, false, new[] { toAddress });
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't send email");
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
            }
        }
    }
}
