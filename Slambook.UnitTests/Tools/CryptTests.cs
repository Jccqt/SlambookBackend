using SlambookBackend.Tools;

namespace Slambook.UnitTests.Tools
{
    public class CryptTests
    {
        [Fact]
        public void HashPassword_WithKnownInput_ShouldReturnExpectedSha256Hash()
        {
            const string password = "CorrectHorseBatteryStaple!";
            const string salt = "fixed-test-salt";
            const string expectedHash = "9dc43fe37a18af6d0e002809332bf443e34d3f4f04ed8788e636d70ff5314894";

            var actualHash = Crypt.HashPassword(password, salt);

            Assert.Equal(expectedHash, actualHash);
        }

        [Fact]
        public void HashPassword_ShouldReturnLowercaseSha256Hex()
        {
            var actualHash = Crypt.HashPassword("password", "salt");

            Assert.Matches("^[0-9a-f]{64}$", actualHash);
        }

        [Fact]
        public void HashPassword_WhenPasswordChanges_ShouldChangeHash()
        {
            const string salt = "fixed-test-salt";

            var firstHash = Crypt.HashPassword("first-password", salt);
            var secondHash = Crypt.HashPassword("second-password", salt);

            Assert.NotEqual(firstHash, secondHash);
        }

        [Fact]
        public void HashPassword_WhenSaltChanges_ShouldChangeHash()
        {
            const string password = "fixed-password";

            var firstHash = Crypt.HashPassword(password, "first-salt");
            var secondHash = Crypt.HashPassword(password, "second-salt");

            Assert.NotEqual(firstHash, secondHash);
        }

        [Fact]
        public void GenerateSalt_ShouldReturnSixteenBase64EncodedBytes()
        {
            var salt = Crypt.GenerateSalt();

            var saltBytes = Convert.FromBase64String(salt);
            Assert.Equal(16, saltBytes.Length);
        }
    }
}
