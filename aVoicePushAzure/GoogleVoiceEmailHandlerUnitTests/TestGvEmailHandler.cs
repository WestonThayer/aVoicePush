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
            ServiceLocator.Current.Item = new MockItem();
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

        public class MockItem : IItem
        {
            public long Id { get; set; }

            public string Email { get; set; }

            public string PushConnectionString { get; set; }

            public int DeviceType { get; set; }

            public IEnumerable<IItem> Query(string userEmail)
            {
                return new List<IItem>() { new MockItem() { Id = 1, Email = userEmail, PushConnectionString = "http://test.com/", DeviceType = 0 } };
            }
        }

        public class MockPushSender : IPushSender
        {
            public bool Send(Uri connection, string rawContent, string clientId, string clientSecret)
            {
                Assert.IsTrue(rawContent.Contains("aVoice Push hit a snag. Try using the permission code Google sent us: 250929777"));

                return false;
            }
        }
    }
}
