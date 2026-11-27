using CribbageRoyale.Server.Sessions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribbageRoyale.Server.Tests.Sessions
{
    [TestClass]
    public class GuestNamesTests
    {
        [DataTestMethod]
        [DataRow("Ana")]
        [DataRow("Player123")]
        [DataRow("abcdefghijklmnop")]
        public void IsValid_AlphanumericNameWithValidLength_ReturnsTrue(string displayName)
        {
            bool isValid = GuestNames.IsValid(displayName);

            Assert.IsTrue(isValid);
        }

        [DataTestMethod]
        [DataRow("Al")]
        [DataRow("abcdefghijklmnopq")]
        [DataRow("Ana Maria")]
        [DataRow("Ana_1")]
        [DataRow("")]
        [DataRow("   ")]
        public void IsValid_InvalidName_ReturnsFalse(string displayName)
        {
            bool isValid = GuestNames.IsValid(displayName);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void IsValid_NullName_ReturnsFalse()
        {
            string displayName = null;

            bool isValid = GuestNames.IsValid(displayName);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void Build_DisplayName_AddsReservedPrefix()
        {
            string displayName = "Ana";

            string guestName = GuestNames.Build(displayName);

            Assert.AreEqual("Guest-Ana", guestName);
        }
    }
}
