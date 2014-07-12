using GoogleVoiceEmailHandler;
using Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailPusher
{
    /// <summary>
    /// Wapper class for writing output.
    /// </summary>
    public class Tr : ILog
    {
        #region Private fields

        private TraceSource traceSource;

        #endregion

        public Tr()
        {
            traceSource = new TraceSource("CustomTrace", SourceLevels.All);
        }

        #region Public methods

        public void Error(string message)
        {
            HelperTrace(TraceEventType.Error, message);
        }

        public void Error(string format, params string[] values)
        {
            HelperTrace(TraceEventType.Error, format, values);
        }

        public void ErrorIf(bool condition, string message)
        {
            HelperTraceIf(TraceEventType.Error, condition, message);
        }

        public void ErrorIf(bool condition, string format, params string[] values)
        {
            HelperTraceIf(TraceEventType.Error, condition, format, values);
        }

        public void Information(string message)
        {
            HelperTrace(TraceEventType.Information, message);
        }

        public void Information(string format, params string[] values)
        {
            HelperTrace(TraceEventType.Information, format, values);
        }

        public void InformationIf(bool condition, string message)
        {
            HelperTraceIf(TraceEventType.Information, condition, message);
        }

        public void InformationIf(bool condition, string format, params string[] values)
        {
            HelperTraceIf(TraceEventType.Information, condition, format, values);
        }

        public void Warning(string message)
        {
            HelperTrace(TraceEventType.Warning, message);
        }

        public void Warning(string format, params string[] values)
        {
            HelperTrace(TraceEventType.Warning, format, values);
        }

        public void WarningIf(bool condition, string message)
        {
            HelperTraceIf(TraceEventType.Warning, condition, message);
        }

        public void WarningIf(bool condition, string format, params string[] values)
        {
            HelperTraceIf(TraceEventType.Warning, condition, format, values);
        }

        #endregion

        #region Private methods

        private void HelperTrace(TraceEventType category, string format, params string[] values)
        {
            HelperTrace(category, string.Format(format, values));
        }

        private void HelperTraceIf(TraceEventType category, bool condition, string format, params string[] values)
        {
            HelperTraceIf(category, condition, string.Format(format, values));
        }

        private void HelperTraceIf(TraceEventType category, bool condition, string message)
        {
            if (condition)
                traceSource.TraceEvent(category, 0, message);
        }

        private void HelperTrace(TraceEventType category, string message)
        {
            HelperTraceIf(category, true, message);
        }

        #endregion
    }
}
