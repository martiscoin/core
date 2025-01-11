using System.Threading.Tasks;
using Martiscoin.Consensus.TransactionInfo;
using Martiscoin.Utilities;

namespace Martiscoin.Interfaces
{
    /// <summary>
    /// An interface used to retrieve unspent transactions
    /// </summary>
    public interface IGetUnspentTransaction
    {
        /// <summary>
        /// Returns the unspent output for a specific transaction.
        /// </summary>
        /// <param name="outPoint">Hash of the transaction to query.</param>
        /// <returns>Unspent Output</returns>
        Task<UnspentOutput> GetUnspentTransactionAsync(OutPoint outPoint);
    }
}
