using Blockcore.Consensus;

namespace Blockcore.Networks.X1.Rules
{
    public static class X1ConsensusErrors
    {
        public static ConsensusError OutputNotWhitelisted => new ConsensusError("tx-output-not-whitelisted", "Only P2WPKH, P2WSH and OP_RETURN are allowed outputs.");

        public static ConsensusError MissingWitness => new ConsensusError("tx-input-missing-witness", "All transaction inputs must have a non-empty WitScript.");

        public static ConsensusError ScriptSigNotEmpty => new ConsensusError("scriptsig-not-empty", "The ScriptSig must be empty.");

        public static ConsensusError FeeBelowAbsoluteMinTxFee => new ConsensusError("fee_below_abolute_min_tx_fee", "The fee must not be below the absolute minimum transaction fee.");

        public static ConsensusError BadPosPowRatchetSequence => new ConsensusError("bad-pos-pow-ratchet-sequence", "Blocks at even heights must be PoS and at odd heights must be PoW.");

        public static ConsensusError RewardAddressBalanceNotEnough => new ConsensusError("tx-input-balance-not-enough", "Check the miner reward address must have enough balance.");

        public static ConsensusError NoPeersConnected => new ConsensusError("tx-input-no-peers-connected", "No peers connect the miner must be the dev.");

        public static ConsensusError LotMinedNotAllowed => new ConsensusError("lot-mine-not-allowed", "Check the lot miner reward address.");

        public static ConsensusError DevRewardAddressCheckFailed => new ConsensusError("tx-output-check-dev-address", "Check the dev reward address.");

        public static ConsensusError LotUsersCheckFailed => new ConsensusError("tx-output-check-lot-users", "Check the Lot reward Users.");

    }
}