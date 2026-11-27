using CribbageRoyale.Server.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribbageRoyale.Server.Tests.Services
{
    [TestClass]
    public class ChatCensorTests
    {
        [TestMethod]
        public void Censor_CleanText_ReturnsSameText()
        {
            string text = "Buena partida, amigo";

            string censoredText = ChatCensor.Censor(text);

            Assert.AreEqual(text, censoredText);
        }

        [TestMethod]
        public void Censor_BlockedWord_MasksWord()
        {
            string text = "que mierda de mano";

            string censoredText = ChatCensor.Censor(text);

            Assert.AreEqual("que ****** de mano", censoredText);
        }

        [TestMethod]
        public void Censor_BlockedWordWithPunctuation_MasksWholeWord()
        {
            string text = "mierda!";

            string censoredText = ChatCensor.Censor(text);

            Assert.AreEqual("*******", censoredText);
        }

        [TestMethod]
        public void Censor_BlockedWordInUpperCase_MasksWord()
        {
            string text = "SHIT happens";

            string censoredText = ChatCensor.Censor(text);

            Assert.AreEqual("**** happens", censoredText);
        }

        [TestMethod]
        public void Censor_WordContainingBlockedWord_KeepsWord()
        {
            string text = "shitty luck";

            string censoredText = ChatCensor.Censor(text);

            Assert.AreEqual(text, censoredText);
        }
    }
}
