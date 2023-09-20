using System;
using TransferDataTypes.Payloads;
using TransferDataTypes;
using System.Threading.Tasks;

namespace TMServer
{
    internal class RequestProcessor
    {
        public required Profiles profiles { get; init; }
        public Message Process(Message message)
        {

            if (message.Action == MessageAction.RegisterAccount)
            {
                Message response = new Message()
                {
                    Action = message.Action,
                    Type = MessageType.Response,
                    Id = message.Id,
                };
                try
                {
                    PayloadCreateAccountResult result = RegisterAccount(message.DeserializePayload<PayloadAccountInfo>());
                    response.Payload = result;
                }
                catch
                {
                    response.Payload = PayloadCreateAccountResult.ServerError;
                }
                return response;

            }
            return null;
        }
        private PayloadCreateAccountResult RegisterAccount(PayloadAccountInfo payloadAccountInfo)
        {
            return profiles.Add(payloadAccountInfo);
        }
    }
}
