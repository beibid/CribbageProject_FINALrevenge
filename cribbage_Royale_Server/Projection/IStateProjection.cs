using CribbageRoyale.Contracts.DataContracts;

namespace CribbageRoyale.Server.Projection
{
    public interface IStateProjection
    {
        PresenceChange BuildPresenceChange(int accountId, bool isConnected);
    }
}