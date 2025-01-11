using Martiscoin.EventBus;
using Martiscoin.Interfaces;
using Newtonsoft.Json;

namespace Martiscoin.Connection.Broadcasting
{
    /// <summary>
    /// Event that is executed when a transaction is broadcasted.
    /// </summary>
    public class TransactionBroadcastEvent : EventBase
    {
        public BroadcastTransactionStateChanedEntry BroadcastEntry { get; }

        [JsonIgnore]
        public IBroadcasterManager BroadcasterManager { get; }

        public TransactionBroadcastEvent(IBroadcasterManager broadcasterManager, BroadcastTransactionStateChanedEntry broadcastEntry)
        {
            this.BroadcasterManager = broadcasterManager;
            this.BroadcastEntry = broadcastEntry;
        }
    }
}