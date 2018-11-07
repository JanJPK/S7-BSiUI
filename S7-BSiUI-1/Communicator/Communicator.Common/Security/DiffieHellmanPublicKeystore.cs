using System.Collections.Generic;
using System.Numerics;

namespace Communicator.Common.Security
{
    /// <summary>
    ///     Stores public keys generated server-side: P and G.
    /// </summary>
    public class DiffieHellmanPublicKeystore
    {
        public BigInteger P { get; }
        public BigInteger G { get; }

        public DiffieHellmanPublicKeystore(BigInteger p, BigInteger g)
        {
            P = p;
            G = g;
        }

        public DiffieHellmanPublicKeystore(Dictionary<string, string> keys)
        {
            P = BigInteger.Parse(keys["p"]);
            G = BigInteger.Parse(keys["g"]);
        }

        public Dictionary<string, string> GetJson()
        {
            return new Dictionary<string, string>
            {
                {"p", P.ToString()},
                {"g", G.ToString()}
            };
        }

        public override string ToString()
        {
            return $"p: {P} g: {G}"; 
        }
    }
}