using TMServer.ConnectionTools.Connections;
using TransferDataTypes;

namespace TMServer.RequestsProcessing.ProcessingStrategies
{
    internal interface IProcessingStrategy
    {
        delegate TMMessage ProcessingDelegate (TMMessage request);
        void ExecuteProcessing(IConnection connection);
        Task ExecuteProcessingAsync(IConnection connection);
    }
}
