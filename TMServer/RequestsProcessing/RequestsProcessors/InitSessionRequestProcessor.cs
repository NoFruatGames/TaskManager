using TMServer.ConnectionTools.Connections;
using TransferDataTypes;
using TransferDataTypes.Messages;
using TransferDataTypes.Results;
namespace TMServer.RequestsProcessing.RequestsProcessors
{
    internal class InitSessionRequestProcessor :IRequestProcessor
    {
        IConnection sender;
        public InitSessionRequestProcessor(IConnection sender) {  this.sender = sender; }
        public TMMessage Process(TMMessage request)
        {
            InitSessionMessage? message = InitSessionMessage.TryParse(request);
            InitSessionMessage response = new InitSessionMessage()
            {
                IsRequest = false,
                CheckTokenResult = InitSessionResult.TokenNotExist
            };
            if (message is not null && GlobalProperties.sessionManager.ValidateSession(message.SessionToken))
            {
                var session = GlobalProperties.sessionManager[message.SessionToken];
                if (session is not null)
                {
                    if (session.Connection is null || !session.Connection.IsConnected)
                    {
                        session.Connection = sender;
                        response.CheckTokenResult = InitSessionResult.Success;
                    }
                    else
                    {
                        response.CheckTokenResult = InitSessionResult.TokenAlreadyUsing;
                    }
                }
                

            }
            return response;
        }
        public async Task<TMMessage> ProcessAsync(TMMessage request)
        {
            return Process(request);
        }
    }
}
