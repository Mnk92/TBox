namespace Mnk.TBox.Plugins.TeamManager.Code.Settings
{
    public sealed class Email
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string ServerUrl { get; set; }
        public bool IsSmtp { get; set; }
        public int Port { get; set; }

        public Email()
        {
            IsSmtp = false;
            Port = 25;
            ServerUrl = "http://smtp.sample.com";
        }

        public Email Clone()
        {
            return new Email
                {
                    IsSmtp = IsSmtp,
                    Login = Login,
                    Password = Password,
                    Port = Port,
                    ServerUrl = ServerUrl
                };
        }
    }
}
