// See https://aka.ms/new-console-template for more information
using TMServer;

Server gameServer = new Server("192.168.0.129", 4980);
try
{
    gameServer.Start();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    gameServer.Dispose();
}