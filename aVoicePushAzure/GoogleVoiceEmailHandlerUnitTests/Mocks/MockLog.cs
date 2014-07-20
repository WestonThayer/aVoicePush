using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVoiceEmailHandlerUnitTests.Mocks
{
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
