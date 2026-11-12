using System;
using CribbageRoyale.Server.Sessions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribbageRoyale.Server.Tests.Sessions
{
    [TestClass]
    public class SessionTokenStoreTests
    {
        private const int AccountId = 7;
        private const int OtherAccountId = 12;
        private const int FirstGuestAccountId = -1;
        private const string GuestName = "Invitado1";

        private static readonly TimeSpan ValidDuration = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan ExpiredDuration = TimeSpan.FromMinutes(-1);

        private SessionTokenStore _tokenStore;

        [TestInitialize]
        public void SetUp()
        {
            _tokenStore = BuildTokenStore(ValidDuration);
        }

        [TestMethod]
        public void TryValidateToken_IssuedToken_ReturnsTrue()
        {
            string token = _tokenStore.IssueToken(AccountId);

            bool isValid = _tokenStore.TryValidateToken(token, out _);

            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void TryValidateToken_IssuedToken_ReturnsAccountId()
        {
            string token = _tokenStore.IssueToken(AccountId);

            _tokenStore.TryValidateToken(token, out SessionIdentity identity);

            Assert.AreEqual(AccountId, identity.AccountId);
        }

        [TestMethod]
        public void TryValidateToken_IssuedToken_ReturnsNotGuest()
        {
            string token = _tokenStore.IssueToken(AccountId);

            _tokenStore.TryValidateToken(token, out SessionIdentity identity);

            Assert.IsFalse(identity.IsGuest);
        }

        [TestMethod]
        public void TryValidateToken_GuestToken_ReturnsGuest()
        {
            string token = _tokenStore.IssueGuestToken(GuestName);

            _tokenStore.TryValidateToken(token, out SessionIdentity identity);

            Assert.IsTrue(identity.IsGuest);
        }

        [TestMethod]
        public void TryValidateToken_FirstGuestToken_ReturnsNegativeAccountId()
        {
            string token = _tokenStore.IssueGuestToken(GuestName);

            _tokenStore.TryValidateToken(token, out SessionIdentity identity);

            Assert.AreEqual(FirstGuestAccountId, identity.AccountId);
        }

        [TestMethod]
        public void TryValidateToken_UnknownToken_ReturnsFalse()
        {
            string unknownToken = "token-inexistente";

            bool isValid = _tokenStore.TryValidateToken(unknownToken, out _);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void TryValidateToken_NullToken_ReturnsFalse()
        {
            string nullToken = null;

            bool isValid = _tokenStore.TryValidateToken(nullToken, out _);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void TryValidateToken_RevokedToken_ReturnsFalse()
        {
            string token = _tokenStore.IssueToken(AccountId);
            _tokenStore.RevokeToken(token);

            bool isValid = _tokenStore.TryValidateToken(token, out _);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void TryValidateToken_ExpiredToken_ReturnsFalse()
        {
            SessionTokenStore expiredTokenStore = BuildTokenStore(ExpiredDuration);
            string token = expiredTokenStore.IssueToken(AccountId);

            bool isValid = expiredTokenStore.TryValidateToken(token, out _);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void RevokeAllTokens_AccountHasTwoTokens_InvalidatesSecondToken()
        {
            _tokenStore.IssueToken(AccountId);
            string secondToken = _tokenStore.IssueToken(AccountId);

            _tokenStore.RevokeAllTokens(AccountId);

            bool isValid = _tokenStore.TryValidateToken(secondToken, out _);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void RevokeAllTokens_OtherAccountHasToken_KeepsOtherToken()
        {
            string otherToken = _tokenStore.IssueToken(OtherAccountId);

            _tokenStore.RevokeAllTokens(AccountId);

            bool isValid = _tokenStore.TryValidateToken(otherToken, out _);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IssueGuestToken_NullDisplayName_ThrowsArgumentNullException()
        {
            string nullDisplayName = null;

            ArgumentNullException exception = Assert.ThrowsException<ArgumentNullException>(
                () => _tokenStore.IssueGuestToken(nullDisplayName));

            Assert.AreEqual("displayName", exception.ParamName);
        }

        private static SessionTokenStore BuildTokenStore(TimeSpan duration)
        {
            return new SessionTokenStore(new GuestAccountIds(), new SessionLifetime(duration));
        }
    }
}