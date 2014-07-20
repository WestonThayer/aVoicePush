using EmailPusher;
using EricDaugherty.CSES.SmtpServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVoiceEmailHandlerUnitTests.Helpers
{
    public class EmailParser
    {
        public static IEmail ReadEmailFromFilePath(string path)
        {
            Assert.IsTrue(File.Exists(path));
            string emailText = File.ReadAllText(path);

            SMTPMessage msg = new SMTPMessage();
            msg.AddData(emailText);

            SmtpEmail email = new SmtpEmail(msg);

            return email;
        }
    }
}
