using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TransferDataTypes;
using TransferDataTypes.Payloads;

namespace TMServer
{
    public class Server :IDisposable
    {
        private ConnectionHandler connectionHandler;
        private Clients clients;
        private Profiles profiles;
        //private RequestProcessor requestProcessor;
        private SessionManager sessionManager;
        private readonly Random random = new Random();
        public Server(string host, int port)
        {
            connectionHandler = new ConnectionHandler(new IPEndPoint(IPAddress.Parse(host), port));
            connectionHandler.ClientConnected += ConnectionHandler_ClientConnected;
            sessionManager = new SessionManager(random);
            clients = new Clients();
            profiles = new Profiles();
            //requestProcessor = new RequestProcessor() 
            //{
            //    Profiles= profiles,
            //    SessionManager = sessionManager
            //};
        }
        public void Start()
        {
            try
            {
                connectionHandler.StartListener();
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Server started at {connectionHandler.EndPoint.Address}:{connectionHandler.EndPoint.Port}");
                Thread listenClientsThread = new Thread(connectionHandler.ListenClients);
                listenClientsThread.Name = "connection listener thread";
                listenClientsThread.Start();
                while (true)
                {
                    Console.Write(">");
                    string? command = Console.ReadLine();
                    if(command == "show clients")
                    {
                        foreach (var c in clients.GetAllClients())
                        {
                            Console.WriteLine(c);
                        }
                        Console.WriteLine("-------------------------------------------------------------------------------");
                    }
                    else if(command == "show profiles")
                    {
                        for(int i = 0; i < profiles.Count; ++i)
                        {
                            Console.WriteLine(profiles.GetById(i));
                        }
                        Console.WriteLine("-------------------------------------------------------------------------------");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Dispose();
            }
        }
        private void ConnectionHandler_ClientConnected(TcpClient obj)
        {
            clients.RegisterClient(obj);
            clients.OnRequestRecived += Clients_OnRequestRecived;
            clients.OnClientDisconnected += Clients_OnClientDisconnected;
            
        }

        private void Clients_OnClientDisconnected(string clientId)
        {
            sessionManager.RemoveClientFromSession(clientId);
        }

        private async Task Clients_OnRequestRecived(Message message, string senderId)
        {
            //TransferDataTypes.Message? response = requestProcessor.Process(message);
            if(message.Action == TransferDataTypes.MessageAction.InitSession) {
                string sessionKey = message.DeserializePayload<string>()??string.Empty;
                if(!sessionManager.RegisterClientForSession(senderId, sessionKey))
                {

                }
            }
            else if(message.Action == TransferDataTypes.MessageAction.LoginToAccount)
            {
                PayloadAccountInfo payloadAccountInfo = message.DeserializePayload<PayloadAccountInfo>();
                Profile? p = profiles.GetByUsername(payloadAccountInfo.Username);
                Message response = new Message()
                {
                    Id = message.Id,
                    Action = MessageAction.LoginToAccount,
                    Type = MessageType.Response,
                };
                sessionManager.RemoveClientFromSession(senderId);
                if (p is not null && p.Password == payloadAccountInfo.Password)
                {
                    string sessionKey = sessionManager.GetSessionKeyByProfile(p.Username);
                    sessionManager.RegisterClientForSession(senderId, sessionKey);
                    response.Payload = new PayloadLoginToAccountResult()
                    {
                        SessionKey = sessionKey,
                        Result = LoginToAccountResult.Success
                    };
                }else if(p is not null && p.Password == payloadAccountInfo.Password)
                {
                    response.Payload = new PayloadLoginToAccountResult()
                    {
                        SessionKey = string.Empty,
                        Result = LoginToAccountResult.WrongPassword
                    };
                }
                else
                {
                    response.Payload = new PayloadLoginToAccountResult()
                    {
                        SessionKey = string.Empty,
                        Result = LoginToAccountResult.WrongUsername
                    };
                }
                ClientHandler? client = clients.GetClientById(senderId);
                if (client is not null)
                    await client.SendMessageAsync(response.Serialize());
            }
            else if (message.Action == TransferDataTypes.MessageAction.RegisterAccount)
            {
                PayloadAccountInfo payloadAccountInfo = message.DeserializePayload<PayloadAccountInfo>();
                CreateAccountResult result = profiles.Add(payloadAccountInfo);
                Message response = new Message()
                {
                    Id = message.Id,
                    Action = MessageAction.LoginToAccount,
                    Type = MessageType.Response,
                };
                sessionManager.RemoveClientFromSession(senderId);
                if (result == CreateAccountResult.Success)
                {
                    string sessionKey = sessionManager.CreateSession(payloadAccountInfo.Username);
                    sessionManager.RegisterClientForSession(senderId, sessionKey);
                    response.Payload = new PayloadCreateAccountResult()
                    {
                        SessionKey = sessionKey,
                        Result = CreateAccountResult.Success
                    };
                }
                else
                {
                    response.Payload = new PayloadCreateAccountResult()
                    {
                        SessionKey = string.Empty,
                        Result = result
                    };
                }
                ClientHandler? client = clients.GetClientById(senderId);
                if (client is not null)
                    await client.SendMessageAsync(response.Serialize());

            }
        }


        public void Dispose()
        {
            try
            {
                connectionHandler.Dispose();
                clients.Dispose();
            }
            catch { }

        }
    }
    
}
