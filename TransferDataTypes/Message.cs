namespace TransferDataTypes
{
    public class Message
    {
        public MessageType Type { get; set; }
        public object? Payload { get; set; }
    }
}