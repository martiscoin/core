using System.Collections.Generic;
using Martiscoin.Consensus.TransactionInfo;
using Martiscoin.NBitcoin.BitcoinCore;
using Martiscoin.Networks;

namespace Martiscoin.Consensus.ScriptInfo
{
    public interface IStandardScriptsRegistry
    {
        void RegisterStandardScriptTemplate(ScriptTemplate scriptTemplate);

        bool IsStandardTransaction(Transaction tx, Network network);

        bool AreOutputsStandard(Network network, Transaction tx);

        ScriptTemplate GetTemplateFromScriptPubKey(Script script);

        bool IsStandardScriptPubKey(Network network, Script scriptPubKey);

        bool AreInputsStandard(Network network, Transaction tx, CoinsView coinsView);

        List<ScriptTemplate> GetScriptTemplates { get; }
    }
}