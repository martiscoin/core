using System.Linq;
using System.Threading.Tasks;
using Marscore.Consensus.TransactionInfo;
using Marscore.Interfaces;
using Marscore.Utilities;

namespace Marscore.Connection.Broadcasting
{
    /// <summary>
    /// Broadcast that makes not checks.
    /// </summary>
    public class NoCheckBroadcastCheck : IBroadcastCheck
    {
        public NoCheckBroadcastCheck()
        {
        }

        public Task<string> CheckTransaction(Transaction transaction)
        {
            return Task.FromResult(string.Empty);
        }
    }
}