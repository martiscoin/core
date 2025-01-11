using Martiscoin.Consensus;
using Martiscoin.Consensus.ScriptInfo;
using Martiscoin.Networks;

namespace Martiscoin.Interfaces
{
    /// <summary>
    /// A reader for extracting an address from a Script
    /// </summary>
    public interface IScriptAddressReader
    {
        /// <summary>
        /// Extracts an address from a given Script, if available. Otherwise returns <see cref="string.Empty"/>
        /// </summary>
        ScriptAddressResult GetAddressFromScriptPubKey(Network network, Script script);
    }
}