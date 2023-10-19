// See https://aka.ms/new-console-template for more information
using System;
using TMServerLinker;
using TMServerLinker.ConnectionHandlers;
TMClient server = new(
    new TCPConnectionHandler("192.168.0.129",4980),
    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NoFruatINC", "TaskManager", "Client")
);
try
{
    while (true)
    {
        Console.WriteLine("1) Register account");
        Console.WriteLine("2) Login to account");
        Console.WriteLine("3) Logout");
        int num = int.Parse(Console.ReadLine());
        if (num == 1)
        {
            Console.Write("Login: ");
            string? login = Console.ReadLine();
            Console.Write("Password: ");
            string? password = Console.ReadLine();
            Console.Write("Email: ");
            string? email = Console.ReadLine();
            _ = Task.Run(() => server.RegisterAccount(new Profile() { Username = login, Password = password, Email = email }));
            
        } else if(num == 2)
        {
            Console.Write("Login: ");
            string? login = Console.ReadLine();
            Console.Write("Password: ");
            string? password = Console.ReadLine();
            _ = Task.Run(() => server.LoginToAccount(login, password));
        }
        else if(num == 3)
        {
            _ = Task.Run(async ()=> await server.Logout());
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    server.Dispose();
}
