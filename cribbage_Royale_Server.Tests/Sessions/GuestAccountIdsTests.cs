using CribbageRoyale.Server.Sessions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribbageRoyale.Server.Tests.Sessions
{
    [TestClass]
    public class GuestAccountIdsTests
    {
        [TestMethod]
        public void Next_FirstCall_ReturnsMinusOne()
        {
            GuestAccountIds guestAccountIds = new GuestAccountIds();

            int accountId = guestAccountIds.Next();

            Assert.AreEqual(-1, accountId);
        }

        [TestMethod]
        public void Next_ConsecutiveCalls_DecreaseByOne()
        {
            GuestAccountIds guestAccountIds = new GuestAccountIds();

            int firstAccountId = guestAccountIds.Next();
            int secondAccountId = guestAccountIds.Next();
            int thirdAccountId = guestAccountIds.Next();

            Assert.AreEqual(-1, firstAccountId);
            Assert.AreEqual(-2, secondAccountId);
            Assert.AreEqual(-3, thirdAccountId);
        }
    }
}
