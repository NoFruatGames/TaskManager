using TMServer.ConnectionTools.Connections;
using TMServer.RequestsProcessing.RequestsProcessors;
using TransferDataTypes;
using TransferDataTypes.Messages;
namespace TMServer.RequestsProcessing
{
    internal class RequestExecutor
    {
        private IRequestProcessor? processor = null;
        private TMMessage request;
        public RequestExecutor(TMMessage request, IConnection sender)
        {
            this.request = request;
            if (CreateAccountMessage.IsIdentity(request))
                processor = new CreateAccountRequestProcessor();
            else if(LogoutMessage.IsIdentity(request))
                processor = new LogoutRequestProcessor();
            else if (InitSessionMessage.IsIdentity(request))
                processor = new InitSessionRequestProcessor(sender);
            else if(LoginMessage.IsIdentity(request))
                processor = new LoginRequestProcessor(sender);
        }
        public TMMessage Execute()
        {
            if (processor is null) throw new ArgumentException("unsupported message type");
            try { return processor.Process(request); }
            catch { throw; }
            
        }
        public async Task<TMMessage> ExecuteAsync()
        {
            if (processor is null) throw new ArgumentException("unsupported message type");
            try { return await processor.ProcessAsync(request); }
            catch { throw; }
            
        }

    }
}
