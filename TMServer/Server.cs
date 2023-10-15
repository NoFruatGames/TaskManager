using TMServer.ConnectionTools.ConnectionListeners;
using TMServer.ConnectionTools.Connections;
using TMServer.RequestsProcessing;
using TransferDataTypes;
using TransferDataTypes.Messages;
namespace TMServer
{
    public class Server : IDisposable
    {
        private readonly Random random = new Random();
        private Dictionary<ConnectionType, IConnectionListener> listeners = new Dictionary<ConnectionType, IConnectionListener>();
        private List<ProcessingExecutor> processingExecutors = new List<ProcessingExecutor>();
        public Server()
        {

        }
        public void RegisterListener(ConnectionType type, string host, int port)
        {
            if (type == ConnectionType.TCP) listeners.Add(ConnectionType.TCP, new TCPConnectionListener(host, port));
        }

        public void Start()
        {
            StartListeners();
            Console.ReadLine();

        }
        private void StartListeners()
        {
            try
            {
                foreach (var listener in listeners)
                {
                    listener.Value.StartListener();
                    listener.Value.ConnectionOpened += Value_ConnectionOpened;
                    Console.WriteLine($"{listener.Key.ToString()} listener started at {listener.Value.GetListenerHost()}:{listener.Value.GetListenerPort()}");
                    Thread listenThread = new Thread(listener.Value.ListenConnections);
                    listenThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"listener launch error: {ex.Message}");
            }
        }

        private void Value_ConnectionOpened(IConnection obj)
        {
            Console.WriteLine($"Connection opened with {obj.GetRemoteHost()}:{obj.GetRemotePort()}");
            ProcessingExecutor executor = new ProcessingExecutor(obj, new(processRequest));
            processingExecutors.Add(executor);
            Thread executeThread= new Thread(executor.Execute);
            executeThread.Start();
        }
        private TMMessage processRequest(TMMessage request)
        {
            Console.WriteLine(request);
            TMMessage response = null!;
            try
            {
                RequestExecutor executor = new RequestExecutor(request);
                response = executor.Execute();
            }
            catch
            {
                response = new DefaultMessage() { IsRequest=false};
            }
            return response;
        }
        public void Dispose()
        {
            foreach (var listener in listeners)
            {
                listener.Value.Dispose();
            }
        }
    }


    public enum ConnectionType
    {
        None, TCP
    }

}
