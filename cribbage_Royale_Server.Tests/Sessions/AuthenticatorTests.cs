using System;
using cribbage_Royale_DataAccess.DataAccess;
using CribbageRoyale.Server.Sessions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CribbageRoyale.Server.Tests.Sessions
{
    [TestClass]
    public class AuthenticatorTests
    {
        private const int AccountId = 7;
        private const int FastWorkFactor = 4;
        private const string UserName = "player_one";

        private Mock<IUserRepository> _userRepositoryMock;
        private Authenticator _authenticator;
        private string _password;
        private UserCredentials _storedCredentials;

        [TestInitialize]
        public void SetUp()
        {
            _password = Guid.NewGuid().ToString("N");
            _storedCredentials = new UserCredentials();
            _storedCredentials.AccountId = AccountId;
            _storedCredentials.PasswordHash = BCrypt.Net.BCrypt.HashPassword(_password, FastWorkFactor);
            _userRepositoryMock = new Mock<IUserRepository>();
            _userRepositoryMock.Setup(repository => repository.FindCredentials(UserName)).Returns(_storedCredentials);
            _authenticator = new Authenticator(_userRepositoryMock.Object);
        }

        [TestMethod]
        public void TryVerifyCredentials_CorrectPassword_ReturnsCredentialsAndRegistersAccess()
        {
            UserCredentials credentials;

            bool isVerified = _authenticator.TryVerifyCredentials(UserName, _password, out credentials);

            Assert.IsTrue(isVerified);
            Assert.AreSame(_storedCredentials, credentials);
            _userRepositoryMock.Verify(repository => repository.RegisterSuccessfulAccess(AccountId), Times.Once);
        }

        [TestMethod]
        public void TryVerifyCredentials_WrongPassword_RegistersFailedAttempt()
        {
            UserCredentials credentials;

            bool isVerified = _authenticator.TryVerifyCredentials(UserName, _password + "x", out credentials);

            Assert.IsFalse(isVerified);
            Assert.IsNull(credentials);
            _userRepositoryMock.Verify(repository => repository.RegisterFailedAttempt(AccountId), Times.Once);
        }

        [TestMethod]
        public void TryVerifyCredentials_UnknownUser_ReturnsFalseWithoutRegisteringAttempt()
        {
            UserCredentials credentials;

            bool isVerified = _authenticator.TryVerifyCredentials("unknown_user", _password, out credentials);

            Assert.IsFalse(isVerified);
            Assert.IsNull(credentials);
            _userRepositoryMock.Verify(repository => repository.RegisterFailedAttempt(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void TryVerifyCredentials_LockedAccount_ReturnsFalseWithoutCheckingPassword()
        {
            _storedCredentials.LockoutEndDate = DateTime.Now.AddHours(1);
            UserCredentials credentials;

            bool isVerified = _authenticator.TryVerifyCredentials(UserName, _password, out credentials);

            Assert.IsFalse(isVerified);
            _userRepositoryMock.Verify(repository => repository.RegisterSuccessfulAccess(AccountId), Times.Never);
            _userRepositoryMock.Verify(repository => repository.RegisterFailedAttempt(AccountId), Times.Never);
        }

        [TestMethod]
        public void TryVerifyCredentials_ExpiredLockout_AcceptsCorrectPassword()
        {
            _storedCredentials.LockoutEndDate = DateTime.Now.AddHours(-1);
            UserCredentials credentials;

            bool isVerified = _authenticator.TryVerifyCredentials(UserName, _password, out credentials);

            Assert.IsTrue(isVerified);
        }

        [TestMethod]
        public void TryVerifyCredentials_EmptyStoredHash_RegistersFailedAttempt()
        {
            _storedCredentials.PasswordHash = string.Empty;
            UserCredentials credentials;

            bool isVerified = _authenticator.TryVerifyCredentials(UserName, _password, out credentials);

            Assert.IsFalse(isVerified);
            _userRepositoryMock.Verify(repository => repository.RegisterFailedAttempt(AccountId), Times.Once);
        }
    }
}
