using GoogleVoiceEmailHandler;
using GoogleVoiceEmailHandlerUnitTests.Helpers;
using GoogleVoiceEmailHandlerUnitTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using System;

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

            IEmail email = EmailParser.ReadEmailFromFilePath("sms_known_new.txt");
            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(@"ooooobebe how are you?", result.Body);
            Assert.AreEqual(@"+14445558888", result.Number);
            Assert.AreEqual(@"Weston Thayer", result.Sender);
            Assert.AreEqual(@"cdca40ae8131958de6403716efb6403f9111904d", result.ThreadId);
            Assert.AreEqual(@"SMS", result.Type);
            Assert.AreEqual(@"obfuscated@gmail.com", result.UserEmail);
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\sms_known_continued.txt")]
        public void TestParseMessageSmsKnownContinued()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = EmailParser.ReadEmailFromFilePath("sms_known_continued.txt");
            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(@"estoy bueno! happy it's friday and sunny. what should we do tonight?", result.Body);
            Assert.AreEqual(@"+14445558888", result.Number);
            Assert.AreEqual(@"Weston Thayer", result.Sender);
            Assert.AreEqual(@"cdca40ae8131958de6403716efb6403f9111904d", result.ThreadId);
            Assert.AreEqual(@"SMS", result.Type);
            Assert.AreEqual(@"obfuscated@gmail.com", result.UserEmail);
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\sms_unknown_new.txt")]
        public void TestParseMessageSmsUnknownNew()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = EmailParser.ReadEmailFromFilePath("sms_unknown_new.txt");
            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(@"Hi! It's Lisa. Should I bring computer/notepad to the meeting?", result.Body);
            Assert.AreEqual(@"+12223334444", result.Number);
            Assert.AreEqual(@"(222) 333-4444", result.Sender);
            Assert.AreEqual(@"91d8b929b4014ade635a74b964a1c81070e1b711", result.ThreadId);
            Assert.AreEqual(@"SMS", result.Type);
            Assert.AreEqual(@"obfuscated@gmail.com", result.UserEmail);
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\sms_unknown_continued.txt")]
        public void TestParseMessageSmsUnknownContinued()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = EmailParser.ReadEmailFromFilePath("sms_unknown_continued.txt");
            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(@"I'm here a bit early so I stopped by the office", result.Body);
            Assert.AreEqual(@"+12223334444", result.Number);
            Assert.AreEqual(@"(222) 333-4444", result.Sender);
            Assert.AreEqual(@"91d8b929b4014ade635a74b964a1c81070e1b711", result.ThreadId);
            Assert.AreEqual(@"SMS", result.Type);
            Assert.AreEqual(@"obfuscated@gmail.com", result.UserEmail);
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\voicemail_unknown.txt")]
        public void TestParseMessageVoicemailUnknown()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = EmailParser.ReadEmailFromFilePath("voicemail_unknown.txt");
            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(@"Hi Natalie, this is Laura Klein from a test one. In the we've been scheduled with us on the all this. 9. Birth of wedding hair. Yes, I need to get us a call back from you to be able to keep that 20 minutes it took me a call back at your earliest convenience. My number is 23334444. Thank you.", result.Body);
            Assert.AreEqual(@"+12223334444", result.Number);
            Assert.AreEqual(@"(222) 333-4444", result.Sender);
            Assert.AreEqual(null, result.ThreadId);
            Assert.AreEqual(@"Voicemail", result.Type);
            Assert.AreEqual(@"obfuscated@gmail.com", result.UserEmail);
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\voicemail_known.txt")]
        public void TestParseMessageVoicemailKnown()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = EmailParser.ReadEmailFromFilePath("voicemail_known.txt");
            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(@"Thank you again. I just want to tell you that we got the books today and thank you so much. And the third really really cool to bring. Some of them only. Com. Anyways, hope you guys are having a good weekend. Just gimme a call tomorrow. Okay bye.", result.Body);
            Assert.AreEqual(@"+12223334444", result.Number);
            Assert.AreEqual(@"FirstN LastN", result.Sender);
            Assert.AreEqual(null, result.ThreadId);
            Assert.AreEqual(@"Voicemail", result.Type);
            Assert.AreEqual(@"obfuscated@gmail.com", result.UserEmail);
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\missedcall_unknown.txt")]
        public void TestParseMessageMissedCallUnknown()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = EmailParser.ReadEmailFromFilePath("missedcall_unknown.txt");
            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(@"Missed call from:  (443) 672-2025 at 2:13 PM", result.Body);
            Assert.AreEqual(@"+14436722025", result.Number);
            Assert.AreEqual(@"(443) 672-2025", result.Sender);
            Assert.AreEqual(null, result.ThreadId);
            Assert.AreEqual(@"Missed call", result.Type);
            Assert.AreEqual(@"obfuscated@gmail.com", result.UserEmail);
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\sms_unknown_new_smallnum.txt")]
        public void TestParseMessageSmsUnknownNewSmallNum()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = EmailParser.ReadEmailFromFilePath("sms_unknown_new_smallnum.txt");
            Message result = GvEmailParser.ParseMessage(email);

            Assert.AreEqual(@"Amazon.com: Your package with Long Rocker Wig - Mixed Blonde has.", result.Body);
            Assert.AreEqual(@"262966", result.Number);
            Assert.AreEqual(@"262966", result.Sender);
            Assert.AreEqual(@"19b059c4d5af854b6f57fce6ae20222d68133415", result.ThreadId);
            Assert.AreEqual(@"SMS", result.Type);
            Assert.AreEqual(@"obfuscated@gmail.com", result.UserEmail);
        }
        
        /// <summary>
        /// The References and Message-ID headers are the only optional ones. Ensure that
        /// we don't throw and instead just end up with a null ThreadId.
        /// </summary>
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

            Assert.AreEqual("test", result.Body);
            Assert.AreEqual("+14445558888", result.Number);
            Assert.AreEqual("Weston Thayer", result.Sender);
            Assert.AreEqual(null, result.ThreadId);
            Assert.AreEqual("SMS", result.Type);
            Assert.AreEqual("obfuscated@gmail.com", result.UserEmail);
        }

        /// <summary>
        /// Every other header is necessary and should throw an Argument exception if
        /// it's missing or in an unexpected form.
        /// </summary>
        [TestMethod]
        public void TestParseMessageError2()
        {
            ServiceLocator.Current.Log = new MockLog();

            MockEmailHeader header = new MockEmailHeader(
                @"""Weston Thayer (SMS)"" <12345678888.14445558888.qR32nhnzms@bad.voice.google.com>", // unknown email
                @"obfuscated@gmail.com",
                null,
                null,
                @"SMS from Weston Thayer [(444) 555-8888]"
                );

            MockEmail email = new MockEmail(header, null, "test");

            AssertExtensions.Throws(typeof(ArgumentException), () => GvEmailParser.ParseMessage(email));

            header = new MockEmailHeader(
                @"""Weston Thayer (SMS)"" <12345678888.14445558888.qR32nhnzms@txt.voice.google.com>",
                @"", // can't be empty
                null,
                null,
                @"SMS from Weston Thayer [(444) 555-8888]"
                );

            email = new MockEmail(header, null, "test");

            AssertExtensions.Throws(typeof(ArgumentException), () => GvEmailParser.ParseMessage(email));

            header = new MockEmailHeader(
                @"Google Voice <voice-noreply@google.com>",
                @"obfuscated@gmail.com",
                null,
                null,
                @"" // can't be empty, but only used with above From header
                );

            email = new MockEmail(header, null, "test");

            AssertExtensions.Throws(typeof(ArgumentException), () => GvEmailParser.ParseMessage(email));
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\forwardrequest.txt")]
        public void TestParsePermission()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = EmailParser.ReadEmailFromFilePath("forwardrequest.txt");
            string link = GvEmailParser.ParsePermission(email);

            Assert.AreEqual(@"https://isolated.mail.google.com/mail/vf-fd4666a8da-test%40outlook.com-3hqNMJvuylEsLqdSs5nmufP48Gk", link);
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\forwardrequest.txt")]
        public void TestParsePermissionForUserEmail()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = EmailParser.ReadEmailFromFilePath("forwardrequest.txt");
            string userEmail = GvEmailParser.ParsePermissionForUserEmail(email);

            Assert.AreEqual(@"cryclopstest@gmail.com", userEmail);
        }

        [TestMethod]
        [DeploymentItem("EmailCollateral\\forwardrequest.txt")]
        public void TestParsePermissionForCode()
        {
            ServiceLocator.Current.Log = new MockLog();

            IEmail email = EmailParser.ReadEmailFromFilePath("forwardrequest.txt");
            string code = GvEmailParser.ParsePermissionForCode(email);

            Assert.AreEqual(@"250929777", code);
        }
    }
}
