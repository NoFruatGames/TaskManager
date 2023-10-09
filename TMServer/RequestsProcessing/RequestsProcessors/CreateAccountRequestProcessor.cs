using TransferDataTypes;
using TransferDataTypes.Messages;

namespace TMServer.RequestsProcessing.RequestsProcessors
{
    internal class CreateAccountRequestProcessor : IRequestProcessor
    {
        public TMMessage Process(TMMessage request)
        {
            //CreateAccountMessage? createAccountM = CreateAccountMessage.TryParse(request);
            Console.WriteLine("create account processing");
            //TMMessage response = new TMMessage();
            return default;
        }
        public async Task<TMMessage> ProcessAsync(TMMessage request)
        {
            //CreateAccountMessage? createAccountM = CreateAccountMessage.TryParse(request);
            Console.WriteLine("create account processing async");
            //TMMessage response = new TMMessage();
            return default;
        }
    }
}
