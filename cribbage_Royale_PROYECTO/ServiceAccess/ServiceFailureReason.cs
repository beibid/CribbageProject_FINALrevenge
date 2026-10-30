namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public enum ServiceFailureReason
    {
        None,
        ServerUnavailable,
        Timeout,
        ServerError,
        SessionRejected,
        RequestRejected
    }
}