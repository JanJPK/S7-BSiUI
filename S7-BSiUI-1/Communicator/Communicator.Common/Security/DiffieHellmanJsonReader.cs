using System;
using System.Numerics;

namespace Communicator.Common.Security
{
    /// <summary>
    ///     Loads primes and their roots from json file.
    /// </summary>
    public class DiffieHellmanJsonReader : SettingsJsonReader
    {
        private readonly Random rng;

        public DiffieHellmanJsonReader(string path = @".\keys.json") : base(path)
        {
            rng = new Random();
        }

        public DiffieHellmanPublicKeystore GetRandomKeystore()
        {
            var index = rng.Next(Pairs.Count);
            var pair = Pairs[index];
            return new DiffieHellmanPublicKeystore(BigInteger.Parse(pair.Key), BigInteger.Parse(pair.Value));
        }
    }
}