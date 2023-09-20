using Newtonsoft.Json;

namespace TransferDataTypes
{
    public class Message
    {
        public required MessageType Type { get; set; } = MessageType.None;
        public required MessageAction Action { get; set; } = MessageAction.None;
        public required int Id { get; set; } = -1;
        
        public object? Payload { get; set; } = null;
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);

        }
        public static Message? Deserialize(string data)
        {
            return JsonConvert.DeserializeObject<Message>(data);
        }
        public T? DeserializePayload<T>()
        {
            try
            {
                if (Payload == null) { return default; }
                T? result = JsonConvert.DeserializeObject<T>(Payload.ToString());
                Payload = result;
                return result;
            }
            catch
            {
                return default;
            }
        }
    }
}