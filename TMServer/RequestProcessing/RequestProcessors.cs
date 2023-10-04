using System;
using System.Text;
using TransferDataTypes;
using TransferDataTypes.Payloads;

namespace TMServer.RequestProcessing
{
    internal partial class RequestProcessors
    {
        List<RequestProcessor> processors = new List<RequestProcessor>();
        public void InitProcessor(string sessionKey, int ProfileId)
        {
            RequestProcessor processor = new RequestProcessor(sessionKey, ProfileId);
        }

        public delegate R UniversalDWithTwoParam<R, FP, SP>(FP firstP, SP secondP);
        public delegate R UniversalDWithOneParam<R,P>(P firstP);
        public UniversalDWithTwoParam<bool, string, string>? AddSenderToSession { get; init; }
        public UniversalDWithTwoParam<abc, PayloadAccountInfo, string>? CreateAccount { get; init; }

        //changes a message variable 
        private void AddClientToReadySession(Message message, string senderId)
        {
            if (AddSenderToSession is null) return;
            string? sessionKey = message.DeserializePayload<string>();
            if (string.IsNullOrEmpty(sessionKey)) return;
            bool res = AddSenderToSession(sessionKey, senderId);
            message.Action = MessageAction.InitSession;
            message.Type = MessageType.Response;
            message.Payload = res;
        }





        public Message Process(Message message, string senderId)
        {
            Message response = new Message()
            {
                Action = MessageAction.None,
                Type = MessageType.None,
                Id = message.Id,
                Payload = null
            };
            if (message.Action == MessageAction.InitSession)
            {
                AddClientToReadySession(response, senderId);
            }
            else if(message.Action == MessageAction.RegisterAccount)
            {
                if (CreateAccount is null) return response;
                PayloadAccountInfo accountInfo = message.DeserializePayload<PayloadAccountInfo>();
                abc result = CreateAccount(accountInfo, senderId);
                response.Action = MessageAction.RegisterAccount;
                response.Type = MessageType.Response;
                response.Payload = result;
            }

            return response;
        }
    }
    internal readonly struct abc
    {
        public string SessionKey { get; }
        public int AccountId { get; }
        public PayloadCreateAccountResult payload { get; }

    }
}
