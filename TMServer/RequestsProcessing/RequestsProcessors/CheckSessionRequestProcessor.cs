using TransferDataTypes;
using TransferDataTypes.Messages;
using TransferDataTypes.Results;
namespace TMServer.RequestsProcessing.RequestsProcessors
{
    internal class CheckSessionRequestProcessor :IRequestProcessor
    {
        public TMMessage Process(TMMessage request)
        {
            CheckSessionMessage? message = CheckSessionMessage.TryParse(request);
            CheckSessionMessage response = new CheckSessionMessage()
            {
                IsRequest = false,
                CheckTokenResult = CheckSessionResult.TokenNotExist
            };
            if (message is not null && GlobalProperties.sessionManager.ValidateSession(message.SessionToken))
            {
                response.CheckTokenResult = CheckSessionResult.Success;
            }
            return response;
        }
        public async Task<TMMessage> ProcessAsync(TMMessage request)
        {
            CheckSessionMessage? message = CheckSessionMessage.TryParse(request);
            CheckSessionMessage response = new CheckSessionMessage()
            {
                IsRequest = false,
                CheckTokenResult = CheckSessionResult.TokenNotExist
            };
            if (message is not null && GlobalProperties.sessionManager.ValidateSession(message.SessionToken))
            {
                response.CheckTokenResult = CheckSessionResult.Success;
            }
            return response;
        }
    }
}
