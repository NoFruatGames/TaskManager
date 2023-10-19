using TMServer.ConnectionTools.Connections;
using TMServer.Data;
using TransferDataTypes;

namespace TMServer.RequestsProcessing.SessionSystem.SessionManagers
{
    internal interface ISessionManager
    {
        int TokenLenght { get; }
        UserSession? CreateSession(string username);
        bool ValidateSession(string sessionToken);
        void ExpireSession(string sessionToken);
        Task NotifyAllConnections(TMMessage message, Profile profile);
        void DeleateAllSessionsFromProfile(Profile profile);
        UserSession? this[string sessionToken]
        {
            get;
        }

    }
}
