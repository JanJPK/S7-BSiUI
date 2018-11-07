using System;
using System.Numerics;

namespace Communicator.Common.Security
{
    /// <summary>
    ///     Generates variables used in Diffie-Hellman key exchange.
    /// </summary>
    public class DiffieHellmanGenerator
    {
        private readonly Random rng;

        public DiffieHellmanGenerator()
        {
            rng = new Random();
        }

        /// <summary>
        ///     Step 1: Generate secret integer a      
        /// </summary>        
        /// <param name="max">maximum a</param>
        /// <returns></returns>
        public int GenerateSecret(int max = 100)
        {                    
            return rng.Next(1, max);
        }        

        /// <summary>
        ///     Step 2: Generate transport integer a where:
        ///         a = (g ^ secret) mod p
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="diffieHellmanPublicKeystore">p and g</param>
        /// <returns></returns>
        public BigInteger GenerateTransport(int secret, DiffieHellmanPublicKeystore diffieHellmanPublicKeystore)
        {            
            var raised = BigInteger.Pow(diffieHellmanPublicKeystore.G, secret);
            var result = raised % diffieHellmanPublicKeystore.P;            
            return result;
        }

        /// <summary>
        ///     Step 3: Generate shared secret 
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="receivedTransport"></param>
        /// <param name="diffieHellmanPublicKeystore"></param>
        /// <returns></returns>
        public BigInteger GenerateSharedSecret(int secret, BigInteger receivedTransport, DiffieHellmanPublicKeystore diffieHellmanPublicKeystore)
        {
            var raised = BigInteger.Pow(receivedTransport, secret);
            var result = raised % diffieHellmanPublicKeystore.P;
            return result;            
        }
    }
}