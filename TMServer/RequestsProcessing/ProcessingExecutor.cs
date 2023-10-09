using TMServer.RequestsProcessing.ProcessingStrategies;
using TMServer.ConnectionTools.Connections;

namespace TMServer.RequestsProcessing
{
    internal class ProcessingExecutor
    {
        private IConnection connection;
        private IProcessingStrategy.ProcessingDelegate processingMethod;
        IProcessingStrategy? strategy = null;
        public ProcessingExecutor(IConnection connection, IProcessingStrategy.ProcessingDelegate processingMethod)
        {
            this.connection = connection;
            this.processingMethod = processingMethod;
            if (connection is TCPConnection)
                strategy = new TCPProcessingStrategy(processingMethod);
        }
        public void Execute()
        {
            if(strategy is null) throw new ArgumentException("Unsupported connection type");
            strategy.ExecuteProcessing(connection);
        }
        public async Task ExecuteAsync()
        {
            if (strategy is null) throw new ArgumentException("Unsupported connection type");
            await strategy.ExecuteProcessingAsync(connection);
        }
    }
}
