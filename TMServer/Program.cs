using TMServer;
Server server = new Server();
server.RegisterListener(ConnectionType.TCP, "192.168.0.129", 4980);
try
{
    server.Start();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    server.Dispose();
}