using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using GoogleVoiceEmailHandlerUnitTests.Mocks;
using System.Collections.Generic;
using GoogleVoiceEmailHandler;
using GoogleVoiceEmailHandlerUnitTests.Helpers;

namespace GoogleVoiceEmailHandlerUnitTests
{
    [TestClass]
    public class TestGvEmailHandler
    {
        private void SetupServices()
        {
            ServiceLocator.Current.Log = new MockLog();
            ServiceLocator.Current.PushSender = new MockPushSender();
        }

        /// <summary>
        /// Test our logic if clicking the link inside a forward email fails.
        /// </summary>
        [TestMethod]
        [DeploymentItem("EmailCollateral\\forwardrequest.txt")]
        public void TestHandleEmailForwardPermissionEmail()
        {
            SetupServices();
            ServiceLocator.Current.LinkClicker = new MockLinkClicker(false);

            IEmail email = EmailParser.ReadEmailFromFilePath("forwardrequest.txt");

            GvEmailHandler handler = new GvEmailHandler();
            handler.HandleEmail(email);

            // Rest of test in MockPushSender
        }

        public class MockPushSender : IPushSender
        {
            public bool Send(string userEmail, string sender, string body)
            {
                Assert.AreEqual("aVoice Push", sender);
                Assert.AreEqual("aVoice Push hit a snag. Try using the permission code Google sent us: 250929777", body);

                return false;
            }
        }
    }
}
