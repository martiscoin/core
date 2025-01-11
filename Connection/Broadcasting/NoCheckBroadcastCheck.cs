using System.Linq;
using System.Threading.Tasks;
using Martiscoin.Consensus.TransactionInfo;
using Martiscoin.Interfaces;
using Martiscoin.Utilities;

namespace Martiscoin.Connection.Broadcasting
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