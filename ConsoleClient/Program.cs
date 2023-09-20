// See https://aka.ms/new-console-template for more information
using System;
using TMServerLinker;
using TMClient server = new TMClient();
try
{
    while (true)
    {
        Console.WriteLine("1) Register account");
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
