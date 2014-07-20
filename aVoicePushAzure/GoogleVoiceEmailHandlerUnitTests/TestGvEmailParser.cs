using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using System.IO;
using EricDaugherty.CSES.SmtpServer;
using EmailPusher;
using GoogleVoiceEmailHandler;

namespace GoogleVoiceEmailHandlerUnitTests
{
    [TestClass]
    public class TestGvEmailParser
    {
        [TestMethod]
        [DeploymentItem("EmailCollateral\\sms_known_new.txt")]
        public void TestParseMessageSmsKnownNew()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = ReadEmailFromFilePath("sms_known_new.txt");
            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(result.Body, @"ooooobebe how are you?");
            Assert.AreEqual(result.Number, @"+14445558888");
            Assert.AreEqual(result.Sender, @"Weston Thayer");
            Assert.AreEqual(result.ThreadId, @"cdca40ae8131958de6403716efb6403f9111904d");
            Assert.AreEqual(result.Type, @"SMS");
            Assert.AreEqual(result.UserEmail, @"obfuscated@gmail.com");
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\sms_known_continued.txt")]
        public void TestParseMessageSmsKnownContinued()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = ReadEmailFromFilePath("sms_known_continued.txt");
            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(result.Body, @"estoy bueno! happy it's friday and sunny. what should we do tonight?");
            Assert.AreEqual(result.Number, @"+14445558888");
            Assert.AreEqual(result.Sender, @"Weston Thayer");
            Assert.AreEqual(result.ThreadId, @"cdca40ae8131958de6403716efb6403f9111904d");
            Assert.AreEqual(result.Type, @"SMS");
            Assert.AreEqual(result.UserEmail, @"obfuscated@gmail.com");
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\sms_unknown_new.txt")]
        public void TestParseMessageSmsUnknownNew()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = ReadEmailFromFilePath("sms_unknown_new.txt");
            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(result.Body, @"Hi! It's Lisa. Should I bring computer/notepad to the meeting?");
            Assert.AreEqual(result.Number, @"+12223334444");
            Assert.AreEqual(result.Sender, @"(222) 333-4444");
            Assert.AreEqual(result.ThreadId, @"91d8b929b4014ade635a74b964a1c81070e1b711");
            Assert.AreEqual(result.Type, @"SMS");
            Assert.AreEqual(result.UserEmail, @"obfuscated@gmail.com");
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\sms_unknown_continued.txt")]
        public void TestParseMessageSmsUnknownContinued()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = ReadEmailFromFilePath("sms_unknown_continued.txt");
            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(result.Body, @"I'm here a bit early so I stopped by the office");
            Assert.AreEqual(result.Number, @"+12223334444");
            Assert.AreEqual(result.Sender, @"(222) 333-4444");
            Assert.AreEqual(result.ThreadId, @"91d8b929b4014ade635a74b964a1c81070e1b711");
            Assert.AreEqual(result.Type, @"SMS");
            Assert.AreEqual(result.UserEmail, @"obfuscated@gmail.com");
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\voicemail_unknown.txt")]
        public void TestParseMessageVoicemailUnknown()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = ReadEmailFromFilePath("voicemail_unknown.txt");
            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(result.Body, @"Hi Natalie, this is Laura Klein from a test one. In the we've been scheduled with us on the all this. 9. Birth of wedding hair. Yes, I need to get us a call back from you to be able to keep that 20 minutes it took me a call back at your earliest convenience. My number is 23334444. Thank you.");
            Assert.AreEqual(result.Number, @"+12223334444");
            Assert.AreEqual(result.Sender, @"(222) 333-4444");
            Assert.AreEqual(result.ThreadId, null);
            Assert.AreEqual(result.Type, @"Voicemail");
            Assert.AreEqual(result.UserEmail, @"obfuscated@gmail.com");
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\voicemail_known.txt")]
        public void TestParseMessageVoicemailKnown()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = ReadEmailFromFilePath("voicemail_known.txt");
            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(result.Body, @"Thank you again. I just want to tell you that we got the books today and thank you so much. And the third really really cool to bring. Some of them only. Com. Anyways, hope you guys are having a good weekend. Just gimme a call tomorrow. Okay bye.");
            Assert.AreEqual(result.Number, @"+12223334444");
            Assert.AreEqual(result.Sender, @"FirstN LastN");
            Assert.AreEqual(result.ThreadId, null);
            Assert.AreEqual(result.Type, @"Voicemail");
            Assert.AreEqual(result.UserEmail, @"obfuscated@gmail.com");
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\missedcall_unknown.txt")]
        public void TestParseMessageMissedCallUnknown()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = ReadEmailFromFilePath("missedcall_unknown.txt");
            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(result.Body, @"Missed call from:  (443) 672-2025 at 2:13 PM");
            Assert.AreEqual(result.Number, @"+14436722025");
            Assert.AreEqual(result.Sender, @"(443) 672-2025");
            Assert.AreEqual(result.ThreadId, null);
            Assert.AreEqual(result.Type, @"Missed call");
            Assert.AreEqual(result.UserEmail, @"obfuscated@gmail.com");
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\sms_unknown_new_smallnum.txt")]
        public void TestParseMessageSmsUnknownNewSmallNum()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = ReadEmailFromFilePath("sms_unknown_new_smallnum.txt");
            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(result.Body, @"Amazon.com: Your package with Long Rocker Wig - Mixed Blonde has.");
            Assert.AreEqual(result.Number, @"262966");
            Assert.AreEqual(result.Sender, @"262966");
            Assert.AreEqual(result.ThreadId, @"19b059c4d5af854b6f57fce6ae20222d68133415");
            Assert.AreEqual(result.Type, @"SMS");
            Assert.AreEqual(result.UserEmail, @"obfuscated@gmail.com");
        }

        [TestMethod]
        public void TestParseMessageError1()
        {
            ServiceLocator.Current.Log = new MockLog();

            MockEmailHeader header = new MockEmailHeader(
                @"""Weston Thayer (SMS)"" <12345678888.14445558888.qR32nhnzms@txt.voice.google.com>",
                @"obfuscated@gmail.com",
                null,
                null,
                @"SMS from Weston Thayer [(444) 555-8888]"
                );

            MockEmail email = new MockEmail(header, null, "test");

            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(result.Body, "test");
            Assert.AreEqual(result.Number, "+14445558888");
            Assert.AreEqual(result.Sender, "Weston Thayer");
            Assert.AreEqual(result.ThreadId, null);
            Assert.AreEqual(result.Type, "SMS");
            Assert.AreEqual(result.UserEmail, "obfuscated@gmail.com");
        }

        private IEmail ReadEmailFromFilePath(string path)
        {
            Assert.IsTrue(File.Exists(path));
            string emailText = File.ReadAllText(path);

            SMTPMessage msg = new SMTPMessage();
            msg.AddData(emailText);

            SmtpEmail email = new SmtpEmail(msg);

            return email;
        }

        public class MockEmail : IEmail
        {
            private MockEmailHeader header;
            private string data, rawBody;

            public IEmailHeader Header { get { return header; } }

            public string Data { get { return data; } }

            public string RawBody { get { return rawBody; } }

            public MockEmail(MockEmailHeader header, string data, string rawBody)
            {
                this.header = header;
                this.data = data;
                this.rawBody = rawBody;
            }
        }

        public class MockEmailHeader : IEmailHeader
        {
            private string from, deliveredTo, references, messageId, subject;

            public string From { get { return from; } }

            public string DeliveredTo { get { return deliveredTo; } }

            public string References { get { return references; } }

            public string MessageId { get { return messageId; } }

            public string Subject { get { return subject; } }

            public MockEmailHeader(string from, string deliveredTo, string references, string messageId, string subject)
            {
                this.from = from;
                this.deliveredTo = deliveredTo;
                this.references = references;
                this.messageId = messageId;
                this.subject = subject;
            }
        }

        public class MockLog : ILog
        {
            public void Information(string message) { }

            public void Information(string format, params string[] values) { }

            public void InformationIf(bool condition, string message) { }

            public void InformationIf(bool condition, string format, params string[] values) { }

            public void Warning(string message) { }

            public void Warning(string format, params string[] values) { }

            public void WarningIf(bool condition, string message) { }

            public void WarningIf(bool condition, string format, params string[] values) { }

            public void Error(string message) { }

            public void Error(string format, params string[] values) { }

            public void ErrorIf(bool condition, string message) { }

            public void ErrorIf(bool condition, string format, params string[] values) { }
        }

    }
}
