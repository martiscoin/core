using System;
using Martiscoin.NBitcoin.BouncyCastle.math;

namespace Martiscoin.NBitcoin.BouncyCastle.crypto.parameters
{
    internal class ECPrivateKeyParameters
        : ECKeyParameters
    {
        private readonly BigInteger d;

        public ECPrivateKeyParameters(
            BigInteger d,
            ECDomainParameters parameters)
            : this("EC", d, parameters)
        {
        }

        public ECPrivateKeyParameters(
            string algorithm,
            BigInteger d,
            ECDomainParameters parameters)
            : base(algorithm, true, parameters)
        {
            if(d == null)
                throw new ArgumentNullException("d");

            this.d = d;
        }

        public BigInteger D
        {
            get
            {
                return this.d;
            }
        }

        public override bool Equals(
            object obj)
        {
            if(obj == this)
                return true;

            var other = obj as ECPrivateKeyParameters;

            if(other == null)
                return false;

            return Equals(other);
        }

        protected bool Equals(
            ECPrivateKeyParameters other)
        {
            return this.d.Equals(other.d) && base.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.d.GetHashCode() ^ base.GetHashCode();
        }
    }
}
