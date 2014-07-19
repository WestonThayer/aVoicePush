using anmar.SharpMimeTools;
using EricDaugherty.CSES.SmtpServer;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailPusher
{
    public class SmtpEmail : IEmail
    {
        private SMTPMessage message;
        private SmtpEmailHeader header;
        private string body;

        #region Public Properties

        public IEmailHeader Header
        {
            get
            {
                return header;
            }
        }

        public string Data
        {
            get
            {
                return message.Data;
            }
        }

        public string RawBody
        {
            get
            {
                return body;
            }
        }

        #endregion

        public SmtpEmail(SMTPMessage message)
        {
            this.message = message;
            header = new SmtpEmailHeader(message);

            var msg = new SharpMessage(new MemoryStream(Encoding.ASCII.GetBytes(message.Data)), SharpDecodeOptions.None);
            body = msg.Body.Trim();
        }

        public class SmtpEmailHeader : IEmailHeader
        {
            private SMTPMessage message;

            #region Public Properties

            public string From
            {
                get
                {
                    return message.Headers["From"] as string;
                }
            }

            public string DeliveredTo
            {
                get
                {
                    return message.Headers["Delivered-To"] as string;
                }
            }

            public string References
            {
                get
                {
                    return message.Headers["References"] as string;
                }
            }

            public string MessageId
            {
                get
                {
                    return message.Headers["Message-ID"] as string;
                }
            }

            public string Subject
            {
                get
                {
                    return message.Headers["Subject"] as string;
                }
            }

            #endregion

            public SmtpEmailHeader(SMTPMessage message)
            {
                this.message = message;
            }
        }
    }
}
