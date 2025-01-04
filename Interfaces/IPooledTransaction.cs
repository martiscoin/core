using System.Threading.Tasks;
using Marscore.Consensus.TransactionInfo;
using Marscore.NBitcoin;

namespace Marscore.Interfaces
{
    public interface IPooledTransaction
    {
        Task<Transaction> GetTransaction(uint256 trxid);
    }
}
