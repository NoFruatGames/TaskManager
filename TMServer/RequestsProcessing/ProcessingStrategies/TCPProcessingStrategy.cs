using TransferDataTypes;
using TMServer.ConnectionTools.Connections;
using static TMServer.RequestsProcessing.ProcessingStrategies.IProcessingStrategy;

namespace TMServer.RequestsProcessing.ProcessingStrategies
{
    internal class TCPProcessingStrategy :IProcessingStrategy
    {
        ProcessingDelegate ProcessRequest;
        public TCPProcessingStrategy(ProcessingDelegate processingMethod)
        {
            ProcessRequest = processingMethod;
        }
        public void ExecuteProcessing(IConnection connection)
        {
            TMMessage? request;
            TMMessage response = null!;
            while (connection.IsConnected)
            {
                request = connection.GetMessage();
                if (request is null) continue;
                response = ProcessRequest(request, connection);
                connection.SendMessage(response);
            }
            Console.WriteLine("connection closed");

        }
        public async Task ExecuteProcessingAsync(IConnection connection)
        {
            TMMessage? request;
            TMMessage response = null!;
            while (connection.IsConnected)
            {
                request = await connection.GetMessageAsync();
                if (request is null) continue;
                response = ProcessRequest(request, connection);
                await connection.SendMessageAsync(response);
            }
            Console.WriteLine("connection closed");
        }
    }
}
