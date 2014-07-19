using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using GoogleVoiceEmailHandler;
using System.Collections.Generic;

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

            try
            {
                WnsMessenger.NotifyUser(null, item, "", "test");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "message");
            }

            try
            {
                WnsMessenger.NotifyUser(message, item, "", "test");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "clientId");
            }

            try
            {
                WnsMessenger.NotifyUser(message, item, "test", "");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "clientSecret");
            }
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
