using TransferDataTypes;
using TransferDataTypes.Messages;
using TransferDataTypes.Results;
namespace TMServer.RequestsProcessing.RequestsProcessors
{
    internal class LogoutRequestProcessor :IRequestProcessor
    {
        public TMMessage Process(TMMessage request)
        {
            LogoutMessage? message = LogoutMessage.TryParse(request);
            LogoutMessage response = new LogoutMessage()
            {
                IsRequest = false,
                LogoutResult = LogoutResult.TokenNotExist
            };

            if (message is not null && GlobalProperties.sessionManager.ValidateSession(message.SessionToken))
            {
                response.LogoutResult = LogoutResult.Success;
                GlobalProperties.sessionManager.ExpireSession(message.SessionToken);
            }
            return response;
        }
        public async Task<TMMessage> ProcessAsync(TMMessage request)
        {
            return Process(request);
        }
    }
}
