﻿using Martiscoin.NBitcoin.BouncyCastle.math;
using Martiscoin.NBitcoin.BouncyCastle.security;

namespace Martiscoin.NBitcoin.BouncyCastle.crypto.signers
{
    /**
     * Interface define calculators of K values for DSA/ECDSA.
     */
    internal interface IDsaKCalculator
    {
        /**
         * Return true if this calculator is deterministic, false otherwise.
         *
         * @return true if deterministic, otherwise false.
         */
        bool IsDeterministic
        {
            get;
        }

        /**
         * Non-deterministic initialiser.
         *
         * @param n the order of the DSA group.
         * @param random a source of randomness.
         */
        void Init(BigInteger n, SecureRandom random);

        /**
         * Deterministic initialiser.
         *
         * @param n the order of the DSA group.
         * @param d the DSA private value.
         * @param message the message being signed.
         */
        void Init(BigInteger n, BigInteger d, byte[] message);

        /**
         * Return the next valid value of K.
         *
         * @return a K value.
         */
        BigInteger NextK();
    }
}
