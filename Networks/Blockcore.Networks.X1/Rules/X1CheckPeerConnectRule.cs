using System.Threading.Tasks;
using Blockcore.Consensus.Rules;
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
    public class X1CheckPeerConnectRule : PartialValidationConsensusRule
    {
        public override Task RunAsync(RuleContext context)
        {
            X1Main x1 = (X1Main)this.Parent.Network;
            var node = x1.Parent as FullNode;
            if (node.ChainBehaviorState.BestPeerTip == null)
            {
                var block = context.ValidationContext.BlockToValidate;
                if (block != null && block.Transactions.Count > 0)
                {
                    var find = block.Transactions.Find(a => a.IsCoinBase == true);
                    if (find != null && find.Outputs.Count > 0)
                    {
                        var address = find.Outputs[0].ScriptPubKey.GetDestinationAddress(this.Parent.Network).ToString();
                        if (!x1.DevAddress.ToLower().Equals(address.ToLower()))
                        {
                            this.Logger.LogTrace($"(-)[FAIL_{nameof(X1CheckPeerConnectRule)}] Mining Address [" + address + "]".ToUpperInvariant());
                            X1ConsensusErrors.NoPeersConnected.Throw();
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}