using CribbageRoyale.Rules;
using CribbageRoyale.Server.Matches;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribbageRoyale.Server.Tests.Matches
{
    [TestClass]
    public class MatchRoomRegistryTests
    {
        private const int FirstAccountId = 7;
        private const int SecondAccountId = 8;
        private const int OtherFirstAccountId = 30;
        private const int OtherSecondAccountId = 31;
        private const long UnknownMatchId = 999;

        private MatchRoomRegistry _matchRoomRegistry;

        [TestInitialize]
        public void SetUp()
        {
            _matchRoomRegistry = new MatchRoomRegistry();
        }

        [TestMethod]
        public void StartMatch_TwoPlayers_BothPlayersAreInMatch()
        {
            _matchRoomRegistry.StartMatch(FirstAccountId, SecondAccountId);

            Assert.IsTrue(_matchRoomRegistry.IsInMatch(FirstAccountId));
            Assert.IsTrue(_matchRoomRegistry.IsInMatch(SecondAccountId));
        }

        [TestMethod]
        public void FindMatchOf_EitherPlayer_ReturnsSameMatch()
        {
            CribbageMatch startedMatch = _matchRoomRegistry.StartMatch(FirstAccountId, SecondAccountId);
            CribbageMatch firstPlayerMatch;
            CribbageMatch secondPlayerMatch;

            bool isFirstFound = _matchRoomRegistry.FindMatchOf(FirstAccountId, out firstPlayerMatch);
            bool isSecondFound = _matchRoomRegistry.FindMatchOf(SecondAccountId, out secondPlayerMatch);

            Assert.IsTrue(isFirstFound);
            Assert.IsTrue(isSecondFound);
            Assert.AreSame(startedMatch, firstPlayerMatch);
            Assert.AreSame(startedMatch, secondPlayerMatch);
        }

        [TestMethod]
        public void CloseMatch_ExistingMatch_ReleasesPlayersAndRemovesMatch()
        {
            CribbageMatch startedMatch = _matchRoomRegistry.StartMatch(FirstAccountId, SecondAccountId);
            CribbageMatch registeredMatch;

            _matchRoomRegistry.CloseMatch(startedMatch.MatchId, FirstAccountId, SecondAccountId);

            Assert.IsFalse(_matchRoomRegistry.IsInMatch(FirstAccountId));
            Assert.IsFalse(_matchRoomRegistry.IsInMatch(SecondAccountId));
            Assert.IsFalse(_matchRoomRegistry.TryGetMatch(startedMatch.MatchId, out registeredMatch));
        }

        [TestMethod]
        public void StartMatch_AfterCloseMatch_PlayersJoinNewMatch()
        {
            CribbageMatch closedMatch = _matchRoomRegistry.StartMatch(FirstAccountId, SecondAccountId);
            _matchRoomRegistry.CloseMatch(closedMatch.MatchId, FirstAccountId, SecondAccountId);
            CribbageMatch foundMatch;

            CribbageMatch newMatch = _matchRoomRegistry.StartMatch(FirstAccountId, SecondAccountId);

            Assert.IsTrue(_matchRoomRegistry.FindMatchOf(SecondAccountId, out foundMatch));
            Assert.AreSame(newMatch, foundMatch);
            Assert.AreNotEqual(closedMatch.MatchId, newMatch.MatchId);
        }

        [TestMethod]
        public void CloseMatch_UnknownMatch_DoesNotThrowNorAffectOtherMatches()
        {
            _matchRoomRegistry.StartMatch(FirstAccountId, SecondAccountId);

            _matchRoomRegistry.CloseMatch(UnknownMatchId, OtherFirstAccountId, OtherSecondAccountId);

            Assert.IsTrue(_matchRoomRegistry.IsInMatch(FirstAccountId));
            Assert.IsTrue(_matchRoomRegistry.IsInMatch(SecondAccountId));
        }
    }
}
