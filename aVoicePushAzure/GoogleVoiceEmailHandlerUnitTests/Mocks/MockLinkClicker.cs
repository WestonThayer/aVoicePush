using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVoiceEmailHandlerUnitTests.Mocks
{
    public class MockLinkClicker : ILinkClicker
    {
        private bool didClickWork;

        public MockLinkClicker(bool didClickWork)
        {
            this.didClickWork = didClickWork;
        }

        public bool Click(string link)
        {
            return didClickWork;
        }
    }
}
