using Communicator.Common.Security.Encoder;
using Xunit;

namespace Communicator.Tests
{
    public class XorEncoderTests
    {
        [Fact]
        public void Should_Encode_String()
        {
            // Arrange
            var xe = new XorEncoder(0x89);
            var message = "test_string";
            var expected = "/ez6/db6/fvg5+4=";

            // Act
            var encodedMessage = xe.Encode(message);

            // Assert
            Assert.True(string.Equals(encodedMessage, expected),
                        $"Expected: {expected}; Actual: {encodedMessage}");
        }

        [Fact]
        public void Should_Decode_String()
        {
            // Arrange
            var xe = new XorEncoder(0x89);
            var message = "/ez6/db6/fvg5+4=";
            var expected = "test_string";

            // Act
            var decodedMessage = xe.Decode(message);

            // Assert
            Assert.True(string.Equals(decodedMessage, expected),
                        $"Expected: {expected}; Actual: {decodedMessage}");
        }

        [Fact]
        public void Should_Encode_And_Decode_String()
        {
            // Arrange
            var xe = new XorEncoder(0x13);
            var message = "encrypt_and_decrypt";

            // Act
            var encodedMessage = xe.Encode(message);
            var decodedMessage = xe.Decode(encodedMessage);

            // Assert
            Assert.True(string.Equals(decodedMessage, message),
                        $"Expected: {message}; Actual: {decodedMessage}");
        }
    }
}