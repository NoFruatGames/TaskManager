using TMServer.ConnectionTools.Connections;

namespace TMServer.RequestsProcessing.SessionSystem
{
    internal class UserSession
    {
        public IConnection? Connection { get; set; } = null;
        public string SessionToken { get;} = string.Empty;
        public static int TokenLenght { get; } = 12;
        public DateTime CreatedTime { get; }
        public UserSession(IConnection? Connection=null)
        {
            SessionToken = GenerateSessionToken();
            CreatedTime = DateTime.Now;
            if (Connection is null)
            {
                this.Connection = null;
            }
            else
            {
                this.Connection = Connection;
                Connection.ClientDisconnected += Connection_ClientDisconnected;
            } 
        }

        private void Connection_ClientDisconnected(IConnection obj)
        {
            Connection = null;
            obj.ClientDisconnected -= Connection_ClientDisconnected;
        }

        private string GenerateSessionToken()
        {
            string token = string.Empty;
            for (int i = 0; i < TokenLenght; ++i)
            {
                token += GlobalProperties.random.Next(1, 9).ToString();
            }
            return token;
        }
    }
}
