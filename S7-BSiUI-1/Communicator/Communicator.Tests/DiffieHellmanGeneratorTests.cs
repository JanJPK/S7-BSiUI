using Communicator.Common;
using Communicator.Common.Security;
using Xunit;

namespace Communicator.Tests
{
    public class DiffieHellmanGeneratorTests
    {
        [Fact]
        public void Should_Generate_Transport_1()
        {
            // Arrange
            var dhg = new DiffieHellmanGenerator();
            var publicKeys = new DiffieHellmanPublicKeystore(23, 5);
            var secret = 4;
            int expected = 4;

            // Act
            var transport = dhg.GenerateTransport(secret, publicKeys);

            // Assert
            Assert.True(transport == expected, $"Expected: {expected}; Actual: {transport}");
        }

        [Fact]
        public void Should_Generate_Transport_2()
        {
            // Arrange
            var dhg = new DiffieHellmanGenerator();
            var publicKeys = new DiffieHellmanPublicKeystore(23, 5);
            var secret = 3;
            int expected = 10;

            // Act
            var transport = dhg.GenerateTransport(secret, publicKeys);

            // Assert
            Assert.True(transport == expected, $"Expected: {expected}; Actual: {transport}");
        }

        [Fact]
        public void Should_Generate_Shared_Secret()
        {
            // Arrange
            var dhg = new DiffieHellmanGenerator();
            var publicKeys = new DiffieHellmanPublicKeystore(23, 5);
            int secret = 4;
            int transport = 10;
            int expected = 18;

            // Act
            var sharedSecret = dhg.GenerateSharedSecret(secret, transport, publicKeys);

            // Assert
            Assert.True(sharedSecret == expected, $"Expected: {expected}; Actual: {sharedSecret}");
        }

        [Fact]
        public void Should_Exchange_Keys_1()
        {
            // Arrange
            var dhg = new DiffieHellmanGenerator();
            var publicKeys = new DiffieHellmanPublicKeystore(23, 5);
            var secretA = dhg.GenerateSecret();
            var secretB = dhg.GenerateSecret();
            var transportA = dhg.GenerateTransport(secretA, publicKeys);
            var transportB = dhg.GenerateTransport(secretB, publicKeys);

            // Act
            var sharedSecretA = dhg.GenerateSharedSecret(secretA, transportB, publicKeys);
            var sharedSecretB = dhg.GenerateSharedSecret(secretB, transportA, publicKeys);

            // Assert
            Assert.True(sharedSecretA > 0 && sharedSecretB > 0,
                        "Shared secret numbers are lesser than 0, possible math exception");
            Assert.True(sharedSecretA == sharedSecretB,
                        $"Shared secret numbers are not equal: a = {sharedSecretA}, b = {sharedSecretB}");
        }

        [Fact]
        public void Should_Exchange_Keys_2()
        {
            // Arrange
            var dhg = new DiffieHellmanGenerator();
            var publicKeys = new DiffieHellmanPublicKeystore(23, 5);
            int secretA = 5;
            int secretB = 3;
            var transportA = dhg.GenerateTransport(secretA, publicKeys);
            var transportB = dhg.GenerateTransport(secretB, publicKeys);

            // Act
            var sharedSecretA = dhg.GenerateSharedSecret(secretA, transportB, publicKeys);
            var sharedSecretB = dhg.GenerateSharedSecret(secretB, transportA, publicKeys);

            // Assert
            Assert.True(sharedSecretA == sharedSecretB,
                        $"Shared secret numbers are not equal: a = {sharedSecretA}, b = {sharedSecretB}");
        }

        [Fact]
        public void Should_Exchange_Keys_3()
        {
            // Arrange
            var dhg = new DiffieHellmanGenerator();
            var primesReader = new DiffieHellmanJsonReader(@".\primes.json");
            var publicKeys = primesReader.GetRandomKeystore();
            int secretA = 5;
            int secretB = 3;
            var transportA = dhg.GenerateTransport(secretA, publicKeys);
            var transportB = dhg.GenerateTransport(secretB, publicKeys);

            // Act
            var sharedSecretA = dhg.GenerateSharedSecret(secretA, transportB, publicKeys);
            var sharedSecretB = dhg.GenerateSharedSecret(secretB, transportA, publicKeys);

            // Assert
            Assert.True(sharedSecretA == sharedSecretB,
                        $"Shared secret numbers are not equal: a = {sharedSecretA}, b = {sharedSecretB}");
        }
    }
}