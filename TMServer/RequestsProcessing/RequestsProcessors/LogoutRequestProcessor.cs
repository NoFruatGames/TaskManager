using TransferDataTypes;
using TransferDataTypes.Messages;
using TransferDataTypes.Results;
namespace TMServer.RequestsProcessing.RequestsProcessors
{
    internal class LogoutRequestProcessor :IRequestProcessor
    {
        public TMMessage Process(TMMessage request)
        {
            LogoutMessage? lRequest = LogoutMessage.TryParse(request);
            LogoutMessage response = new LogoutMessage()
            {
                IsRequest = false,
                LogoutResult = LogoutResult.TokenNotExist
            };
            
            if(lRequest is not null && GlobalProperties.sessionManager.ValidateSession(lRequest.SessionToken))
            {
                response.LogoutResult = LogoutResult.Success;
                GlobalProperties.sessionManager.ExpireSession(lRequest.SessionToken);
            }
            return response;
        }
        public async Task<TMMessage> ProcessAsync(TMMessage request)
        {
            LogoutMessage? lRequest = LogoutMessage.TryParse(request);
            LogoutMessage response = new LogoutMessage()
            {
                IsRequest = false,
                LogoutResult = LogoutResult.TokenNotExist
            };

            if (lRequest is not null && GlobalProperties.sessionManager.ValidateSession(lRequest.SessionToken))
            {
                response.LogoutResult = LogoutResult.Success;
                GlobalProperties.sessionManager.ExpireSession(lRequest.SessionToken);
            }
            return response;
        }
    }
}
