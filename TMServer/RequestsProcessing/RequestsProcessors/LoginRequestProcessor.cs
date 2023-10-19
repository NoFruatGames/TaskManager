using TMServer.ConnectionTools.Connections;
using TMServer.Data;
using TMServer.RequestsProcessing.SessionSystem;
using TransferDataTypes;
using TransferDataTypes.Messages;
using TransferDataTypes.Results;
namespace TMServer.RequestsProcessing.RequestsProcessors
{
    internal class LoginRequestProcessor : IRequestProcessor
    {
        IConnection sender;
        public LoginRequestProcessor(IConnection sender) { this.sender = sender; }

        public TMMessage Process(TMMessage request)
        {
            LoginMessage? message = LoginMessage.TryParse(request);
            if (message is null) return default;
            LoginMessage response = new LoginMessage() { IsRequest = false, SessionToken="none" };
            Profile? profile = GlobalProperties.profiles[message.Username];
            if (profile is null)
            { 
                response.LoginResult = LoginResult.WrongUsername;
                return response;
            }
            if(profile.Password != message.Password)
            {
                response.LoginResult = LoginResult.WrongPassword;
                return response;
            }
            UserSession? session = GlobalProperties.sessionManager.CreateSession(message.Username);
            if(session is null)
            {
                response.LoginResult = LoginResult.CreateSessionServerError;
                return response;
            }
            session.Connection = sender;
            response.SessionToken = session.SessionToken;
            return response;
        }
        public async Task<TMMessage> ProcessAsync(TMMessage request)
        {
            return Process(request);
        }
    }
}
