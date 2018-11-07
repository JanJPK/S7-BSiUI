using Communicator.Common.Security.Encoder;
using Xunit;

namespace Communicator.Tests
{
    public class CaesarEncoderTests
    {
        [Fact]
        public void Should_Encode_String()
        {
            // Arrange
            var ce = new CaesarEncoder(1234);
            var message = "test_string";
            var expected = "RjdFRjFFRkQ7QDk=";


            // Act
            var encodedMessage = ce.Encode(message);

            // Assert
            Assert.True(string.Equals(encodedMessage, expected),
                        $"Expected: {expected}; Actual: {encodedMessage}");
        }

        [Fact]
        public void Should_Decode_String()
        {
            // Arrange
            var ce = new CaesarEncoder(1234);
            var message = "RjdFRjFFRkQ7QDk=";
            var expected = "test_string";

            // Act
            var decodedMessage = ce.Decode(message);

            // Assert
            Assert.True(string.Equals(decodedMessage, expected),
                        $"Expected: {expected}; Actual: {decodedMessage}");
        }

        [Fact]
        public void Should_Encode_And_Decode_String()
        {
            // Arrange
            var ce = new CaesarEncoder(1234);
            var message = "encrypt_and_decrypt";

            // Act
            var encodedMessage = ce.Encode(message);
            var decodedMessage = ce.Decode(encodedMessage);

            // Assert
            Assert.True(string.Equals(decodedMessage, message),
                        $"Expected: {message}; Actual: {decodedMessage}");
        }
    }
}