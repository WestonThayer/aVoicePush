using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVoiceEmailHandlerUnitTests.Mocks
{
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
}
