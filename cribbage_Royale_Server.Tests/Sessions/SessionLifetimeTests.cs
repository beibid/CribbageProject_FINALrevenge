using System;
using CribbageRoyale.Server.Sessions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribbageRoyale.Server.Tests.Sessions
{
    [TestClass]
    public class SessionLifetimeTests
    {
        private static readonly TimeSpan Duration = TimeSpan.FromHours(2);

        [TestMethod]
        public void CalculateExpiration_Duration_ReturnsCurrentTimePlusDuration()
        {
            SessionLifetime sessionLifetime = new SessionLifetime(Duration);
            DateTime earliestExpiration = DateTime.UtcNow.Add(Duration);

            DateTime expiration = sessionLifetime.CalculateExpiration();

            DateTime latestExpiration = DateTime.UtcNow.Add(Duration);
            Assert.IsTrue(expiration >= earliestExpiration);
            Assert.IsTrue(expiration <= latestExpiration);
        }

        [TestMethod]
        public void CalculateExpiration_AnyDuration_ReturnsUtcTime()
        {
            SessionLifetime sessionLifetime = new SessionLifetime(Duration);

            DateTime expiration = sessionLifetime.CalculateExpiration();

            Assert.AreEqual(DateTimeKind.Utc, expiration.Kind);
        }
    }
}
