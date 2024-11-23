using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockcore.Consensus.BlockInfo;
using Blockcore.Consensus.Rules;
using Blockcore.Consensus.TransactionInfo;
using Blockcore.Features.BlockStore.AddressIndexing;
using Blockcore.Features.MemoryPool.Fee;
using Blockcore.Networks.X1.Consensus;
using Blockcore.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blockcore.Networks.X1.Rules
{
    /// <summary>
    /// Check the miner reward address must have enough balance.
    /// </summary>
    public class X1CheckLotMinedRule : PartialValidationConsensusRule
    {
        public override Task RunAsync(RuleContext context)
        {
            //X1Main x1 = (X1Main)this.Parent.Network;
            //var node = x1.Parent as FullNode;

            //Block block = context.ValidationContext.BlockToValidate;
            //if (block != null && block.Transactions.Count > 0)
            //{
            //    Transaction find = block.Transactions.Find(a => a.IsCoinBase == true);
            //    var address = find.Outputs[0].ScriptPubKey.GetDestinationAddress(this.Parent.Network).ToString();
            //    List<string> lotAddresses = new List<string>();
            //    List<uint> lotNonces = new List<uint>();

            //    foreach (X1Transaction tx in block.Transactions)
            //    {
            //        block.Header.Nonce = tx.LotNonce;
            //        if (tx.LotNonce > 0 && ((X1BlockHeader)block.Header).CheckLotProofOfWork(tx.LotHeaderBytes, tx.LotNonce))
            //        {
            //            Blockcore.Consensus.ScriptInfo.Script script = tx.Outputs.OrderBy(op => op.Value).First().ScriptPubKey;
            //            var lotAddress = script.GetDestinationAddress(this.Parent.Network).ToString();
            //            if (lotAddress.Equals(address))
            //            {
            //                this.Logger.LogTrace($"(-)[FAIL_{nameof(X1CheckLotMinedRule)}] Mining Address [" + lotAddress + "] Not Allowed".ToUpperInvariant());
            //                X1ConsensusErrors.LotMinedNotAllowed.Throw();
            //            }
            //            if (lotAddresses.Count(t => t == address) > 0)
            //            {
            //                this.Logger.LogTrace($"(-)[FAIL_{nameof(X1CheckLotMinedRule)}] Lot Address [" + lotAddress + "] Are Existed".ToUpperInvariant());
            //                X1ConsensusErrors.LotMinedNotAllowed.Throw();
            //            }
            //            if (lotNonces.Count(n => n == tx.LotNonce) > 0)
            //            {
            //                this.Logger.LogTrace($"(-)[FAIL_{nameof(X1CheckLotMinedRule)}] Lot Nonce [" + tx.LotNonce + "] Are Existed".ToUpperInvariant());
            //                X1ConsensusErrors.LotMinedNotAllowed.Throw();
            //            }
            //            lotAddresses.Add(address);
            //            lotNonces.Add(tx.LotNonce);
            //        }
            //    }

            //    var txtDevAddress = block.Transactions[0].Outputs[0].ScriptPubKey.GetDestinationAddress(this.Parent.Network).ToString();
            //    if (!x1.DevAddress.Equals(txtDevAddress))
            //    {
            //        this.Logger.LogTrace($"(-)[FAIL_{nameof(X1CheckLotMinedRule)}] Dev Reward Address Must be [" + x1.DevAddress + "]".ToUpperInvariant());
            //        X1ConsensusErrors.DevRewardAddressCheckFailed.Throw();
            //    }
            //    if (!(lotAddresses.Count == block.Transactions[0].Outputs.Count - 2))
            //    {
            //        this.Logger.LogTrace($"(-)[FAIL_{nameof(X1CheckLotMinedRule)}] Lot Miner Inconsistent Number Of Users".ToUpperInvariant());
            //        X1ConsensusErrors.LotUsersCheckFailed.Throw();
            //    }
            //}

            return Task.CompletedTask;
        }
    }
}