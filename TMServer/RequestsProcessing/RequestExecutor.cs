using TMServer.RequestsProcessing.RequestsProcessors;
using TransferDataTypes;
using TransferDataTypes.Messages;
namespace TMServer.RequestsProcessing
{
    internal class RequestExecutor
    {
        private IRequestProcessor? processor = null;
        private TMMessage request;
        public RequestExecutor(TMMessage request)
        {
            this.request = request;
            if (CreateAccountMessage.IsIdentity(request))
                processor = new CreateAccountRequestProcessor();
            else if(LogoutMessage.IsIdentity(request))
                processor = new LogoutRequestProcessor();
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
