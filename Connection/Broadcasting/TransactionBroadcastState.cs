namespace Martiscoin.Connection.Broadcasting
{
    public enum TransactionBroadcastState
    {
        FailedBroadcast,
        ReadyToBroadcast,
        Broadcasted,
        Propagated
    }
}