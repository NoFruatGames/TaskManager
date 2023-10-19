using TMServer.ConnectionTools.Connections;

namespace TMServer
{
    internal class Connections
    {
        private List<IConnection> connections = new List<IConnection>();
        public void Register(IConnection connection)
        {
            if (!string.IsNullOrEmpty(connection.Id))
            {
                connections.Add(connection);
                connection.ClientDisconnected += Connection_ClientDisconnected;
            }

        }

        private void Connection_ClientDisconnected(IConnection obj)
        {
            connections.Remove(obj);
        }

        public IConnection? this[string id]
        {
            get
            {
                foreach (var connection in connections)
                {
                    if (connection.Id == id)
                        return connection;
                }
                return null;
            }
        }
    }
}
