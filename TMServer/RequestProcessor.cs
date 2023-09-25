using System;
using TransferDataTypes.Payloads;
using TransferDataTypes;
using System.Threading.Tasks;

namespace TMServer
{
    //internal class RequestProcessor
    //{
    //    public required Profiles Profiles { get; init; }
    //    public required SessionManager SessionManager { get; init; }
    //    public Message? Process(Message message)
    //    {

    //        if (message.Action == MessageAction.RegisterAccount)
    //        {
    //            Message response = new Message()
    //            {
    //                Action = message.Action,
    //                Type = MessageType.Response,
    //                Id = message.Id,
    //            };
    //            try
    //            {
    //                PayloadCreateAccountResult result = RegisterAccount(message.DeserializePayload<PayloadAccountInfo>());
    //                response.Payload = result;
    //            }
    //            catch
    //            {
    //                response.Payload = CreateAccountResult.;
    //            }
    //            return response;

    //        }
    //        else if(message.Action == MessageAction.InitSession)
    //        {

    //        }
    //        return null;
    //    }
    //    private PayloadCreateAccountResult RegisterAccount(PayloadAccountInfo payloadAccountInfo)
    //    {
    //        return Profiles.Add(payloadAccountInfo);
    //    }
    //}
}
