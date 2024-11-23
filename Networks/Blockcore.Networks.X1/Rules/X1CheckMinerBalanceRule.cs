using System;
using System.Threading.Tasks;
using Blockcore.Consensus.Rules;
using Blockcore.Features.BlockStore.AddressIndexing;
using Blockcore.Features.MemoryPool.Fee;
using Blockcore.NBitcoin;
using Blockcore.Networks.X1.Consensus;
using Blockcore.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blockcore.Networks.X1.Rules
{
    /// <summary>
    /// Check the miner reward address must have enough balance.
    /// </summary>
    public class X1CheckMinerBalanceRule : PartialValidationConsensusRule
    {
        public override Task RunAsync(RuleContext context)
        {
            var node = ((X1Main)this.Parent.Network).Parent as FullNode;
            if (node.ChainBehaviorState.BestPeerTip != null)
            {
                var index = ((X1Main)this.Parent.Network).Parent.Services.ServiceProvider.GetRequiredService(typeof(IAddressIndexer)) as AddressIndexer;
                if (index != null && index.IndexerTip != null)
                {
                    var bestHeight = node.ChainBehaviorState.BestPeerTip.Height;
                    var consensusHeight = node.ChainBehaviorState.ConsensusTip.Height;
                    if (bestHeight <= (consensusHeight + 1) && consensusHeight > 0)
                    {
                        var indexHeight = index.IndexerTip.Height;
                        if (consensusHeight == indexHeight)
                        {
                            var block = context.ValidationContext.BlockToValidate;
                            if (block != null && block.Transactions.Count > 0)
                            {
                                var ctime = DateTime.UtcNow;
                                var find = block.Transactions.Find(a => a.IsCoinBase == true);
                                if (find != null && find.Outputs.Count > 0)
                                {
                                    var address = find.Outputs[0].ScriptPubKey.GetDestinationAddress(this.Parent.Network).ToString();
                                    var balance = index.GetAddressBalances(new string[] { address });
                                    if (balance.Balances.Count <= 0)
                                    {
                                        this.Logger.LogTrace($"(-)[FAIL_{nameof(X1CheckMinerBalanceRule)}] Mining Address [" + address + "]".ToUpperInvariant());
                                        X1ConsensusErrors.RewardAddressBalanceNotEnough.Throw();
                                    }
                                    var mBalance = balance.Balances[0].Balance;
                                    if (mBalance <= 0)
                                    {
                                        this.Logger.LogTrace($"(-)[FAIL_{nameof(X1CheckMinerBalanceRule)}] Mining Address [" + address + "]".ToUpperInvariant());
                                        X1ConsensusErrors.RewardAddressBalanceNotEnough.Throw();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}