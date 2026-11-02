using CribbageRoyale.Contracts.ServiceContracts;

namespace CribbageRoyale.Server.Connections
{
    public abstract class Notification
    {
        public abstract void Deliver(IGameClientCallback callback);
    }
}