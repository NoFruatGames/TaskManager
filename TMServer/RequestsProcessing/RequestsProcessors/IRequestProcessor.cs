using TransferDataTypes;

namespace TMServer.RequestsProcessing.RequestsProcessors
{
    internal interface IRequestProcessor
    {
        TMMessage Process(TMMessage request);
        Task<TMMessage> ProcessAsync(TMMessage request);
    }
}
