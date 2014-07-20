using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using System.Collections.Generic;
using GoogleVoiceEmailHandler;
using GoogleVoiceEmailHandlerUnitTests.Helpers;

namespace GoogleVoiceEmailHandlerUnitTests
{
    [TestClass]
    public class TestWnsMessenger
    {
        private Message GetFakeMessage()
        {
            Message message = new Message()
            {
                Body = "This is a test body",
                Number = "+14104440000",
                Sender = "Test Sender",
                ThreadId = "asdf",
                Type = "SMS",
                UserEmail = "test@email.com"
            };

            return message;
        }

        [TestMethod]
        public void TestNotifyUserNormal()
        {
            ServiceLocator.Current.PushSender = new MockPushSender();

            Message message = GetFakeMessage();

            MockItem item = new MockItem()
            {
                PushConnectionString = "http://testuri.com"
            };

            WnsMessenger.NotifyUser(message, item, "test", "test");
        }

        [TestMethod]
        public void TestNotifyUserSerialization()
        {
            ServiceLocator.Current.PushSender = new MockPushSender();

            // Pass all null properties
            Message message = new Message();

            MockItem item = new MockItem()
            {
                PushConnectionString = "http://testuri.com"
            };

            WnsMessenger.NotifyUser(message, item, "test", "test");
        }

        [TestMethod]
        public void TestNotifyUserBadArgs()
        {
            ServiceLocator.Current.PushSender = new MockPushSender();

            Message message = GetFakeMessage();

            MockItem item = new MockItem()
            {
                PushConnectionString = "http://testuri.com"
            };

            AssertExtensions.Throws(typeof(ArgumentException), "message", () => WnsMessenger.NotifyUser(null, item, "test", "test"));
            AssertExtensions.Throws(typeof(ArgumentException), "clientId", () => WnsMessenger.NotifyUser(message, item, "", "test"));
            AssertExtensions.Throws(typeof(ArgumentException), "clientSecret", () => WnsMessenger.NotifyUser(message, item, "test", ""));
        }

        public class MockItem : IItem
        {
            public long Id { get; set; }

            public string Email { get; set; }

            public string PushConnectionString { get; set; }

            public int DeviceType { get; set; }

            public IEnumerable<IItem> Query(string userEmail)
            {
                throw new NotImplementedException();
            }
        }

        public class MockPushSender : IPushSender
        {
            public bool Send(Uri connection, string rawContent, string clientId, string clientSecret)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(rawContent));

                return false;
            }
        }
    }
}
