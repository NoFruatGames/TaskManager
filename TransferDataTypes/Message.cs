namespace TransferDataTypes
{
    public class Message
    {
        public MessageType Type { get; set; } = MessageType.None;
        public MessageAction Action { get; set; } = MessageAction.None;
        public object? Payload { get; set; } = null;
    }
}