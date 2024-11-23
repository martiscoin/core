using System.Threading.Tasks;
using Blockcore.Consensus.Rules;
using Blockcore.Features.BlockStore.AddressIndexing;
using Blockcore.Features.MemoryPool.Fee;
using Blockcore.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blockcore.Networks.X1.Rules
{
    /// <summary>
    /// Checks if all transaction in the block have witness.
    /// </summary>
    public class X1RequireWitnessRule : PartialValidationConsensusRule
    {
        public override Task RunAsync(RuleContext context)
        {
            var block = context.ValidationContext.BlockToValidate;

            foreach (var tx in block.Transactions)
            {
                if (!tx.HasWitness)
                {
                    this.Logger.LogTrace($"(-)[FAIL_{nameof(X1RequireWitnessRule)}]".ToUpperInvariant());
                    X1ConsensusErrors.MissingWitness.Throw();
                }
            }

            return Task.CompletedTask;
        }
    }
}