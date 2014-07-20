using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVoiceEmailHandlerUnitTests.Mocks
{
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
}
