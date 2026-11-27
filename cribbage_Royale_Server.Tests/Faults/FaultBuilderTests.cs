using System.ServiceModel;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Server.Faults;
using CribbageRoyale.Server.Sessions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribbageRoyale.Server.Tests.Faults
{
    [TestClass]
    public class FaultBuilderTests
    {
        [TestMethod]
        public void ChatFaultBuilder_Reason_BuildsFaultWithReason()
        {
            ChatFailure reason = ChatFailure.InvalidMessageText;

            FaultException<ChatFault> faultException = ChatFaultBuilder.Build(reason);

            Assert.AreEqual(reason, faultException.Detail.Reason);
            Assert.AreEqual(reason.ToString(), faultException.Reason.ToString());
        }

        [TestMethod]
        public void FriendshipFaultBuilder_Reason_BuildsFaultWithReason()
        {
            FriendshipFailure reason = FriendshipFailure.RequestAlreadyPending;

            FaultException<FriendshipFault> faultException = FriendshipFaultBuilder.Build(reason);

            Assert.AreEqual(reason, faultException.Detail.Reason);
            Assert.AreEqual(reason.ToString(), faultException.Reason.ToString());
        }

        [TestMethod]
        public void InvitationFaultBuilder_Failure_BuildsFaultWithReason()
        {
            InvitationFailure failure = InvitationFailure.RecipientBusy;

            FaultException<InvitationFault> faultException = InvitationFaultBuilder.Build(failure);

            Assert.AreEqual(failure, faultException.Detail.Reason);
            Assert.AreEqual(failure.ToString(), faultException.Reason.ToString());
        }

        [TestMethod]
        public void SessionFaultBuilder_Failure_BuildsFaultWithFailure()
        {
            SessionFailure failure = SessionFailure.GuestNotAllowed;

            FaultException<SessionFault> faultException = SessionFaultBuilder.Build(failure);

            Assert.AreEqual(failure, faultException.Detail.Failure);
            Assert.AreEqual(failure.ToString(), faultException.Reason.ToString());
        }

        [TestMethod]
        public void ServiceFaultBuilder_Code_BuildsFaultWithCode()
        {
            ServiceFaultCode code = ServiceFaultCode.DataAccessFailure;

            FaultException<ServiceFault> faultException = ServiceFaultBuilder.Build(code);

            Assert.AreEqual(code, faultException.Detail.Code);
            Assert.AreEqual(code.ToString(), faultException.Reason.ToString());
        }

        [TestMethod]
        public void ServiceFaultBuilder_AnyCode_AssignsCorrelationId()
        {
            FaultException<ServiceFault> faultException = ServiceFaultBuilder.Build(ServiceFaultCode.InternalFailure);

            Assert.IsFalse(string.IsNullOrWhiteSpace(faultException.Detail.CorrelationId));
        }

        [TestMethod]
        public void ServiceFaultBuilder_TwoCalls_AssignsDifferentCorrelationIds()
        {
            FaultException<ServiceFault> firstFault = ServiceFaultBuilder.Build(ServiceFaultCode.InternalFailure);
            FaultException<ServiceFault> secondFault = ServiceFaultBuilder.Build(ServiceFaultCode.InternalFailure);

            Assert.AreNotEqual(firstFault.Detail.CorrelationId, secondFault.Detail.CorrelationId);
        }
    }
}
