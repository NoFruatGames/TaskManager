using TMServer.Data;
using TransferDataTypes;
using TransferDataTypes.Messages;
using TransferDataTypes.Results;
namespace TMServer.RequestsProcessing.RequestsProcessors
{
    internal class CreateAccountRequestProcessor : IRequestProcessor
    {
        public TMMessage Process(TMMessage request)
        {
            CreateAccountMessage? message = CreateAccountMessage.TryParse(request);
            if (message is null) return default;
            CreateAccountMessage response = new CreateAccountMessage() { IsRequest = false };
            CanAddProfile canAdd = GlobalProperties.profiles.Add(new Profile(message));
            if (canAdd == CanAddProfile.Success)
            {
                response.CreateResult = CreateAccountResult.Success;
            }
            else
            {
                if (canAdd == CanAddProfile.UsernameExist)
                    response.CreateResult = CreateAccountResult.UsernameExist;
            }
            return response;
        }
        public async Task<TMMessage> ProcessAsync(TMMessage request)
        {
            return Process(request);
        }
    }
}
