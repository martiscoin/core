using System.Threading.Tasks;
using Martiscoin.Consensus.TransactionInfo;
using Martiscoin.NBitcoin;

namespace Martiscoin.Interfaces
{
    public interface IPooledTransaction
    {
        Task<Transaction> GetTransaction(uint256 trxid);
    }
}
