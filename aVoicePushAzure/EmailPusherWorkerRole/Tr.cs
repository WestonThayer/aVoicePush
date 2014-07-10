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
    public static class Tr
    {
        #region Private fields

        private static TraceSource traceSource = new TraceSource("CustomTrace", SourceLevels.All);

        #endregion

        #region Public methods

        public static void Error(string message)
        {
            HelperTrace(TraceEventType.Error, message);
        }

        public static void Error(string format, params string[] values)
        {
            HelperTrace(TraceEventType.Error, format, values);
        }

        public static void ErrorIf(bool condition, string message)
        {
            HelperTraceIf(TraceEventType.Error, condition, message);
        }

        public static void ErrorIf(bool condition, string format, params string[] values)
        {
            HelperTraceIf(TraceEventType.Error, condition, format, values);
        }

        public static void Information(string message)
        {
            HelperTrace(TraceEventType.Information, message);
        }

        public static void Information(string format, params string[] values)
        {
            HelperTrace(TraceEventType.Information, format, values);
        }

        public static void InformationIf(bool condition, string message)
        {
            HelperTraceIf(TraceEventType.Information, condition, message);
        }

        public static void InformationIf(bool condition, string format, params string[] values)
        {
            HelperTraceIf(TraceEventType.Information, condition, format, values);
        }

        public static void Warning(string message)
        {
            HelperTrace(TraceEventType.Warning, message);
        }

        public static void Warning(string format, params string[] values)
        {
            HelperTrace(TraceEventType.Warning, format, values);
        }

        public static void WarningIf(bool condition, string message)
        {
            HelperTraceIf(TraceEventType.Warning, condition, message);
        }

        public static void WarningIf(bool condition, string format, params string[] values)
        {
            HelperTraceIf(TraceEventType.Warning, condition, format, values);
        }

        #endregion

        #region Private methods

        private static void HelperTrace(TraceEventType category, string format, params string[] values)
        {
            HelperTrace(category, string.Format(format, values));
        }

        private static void HelperTraceIf(TraceEventType category, bool condition, string format, params string[] values)
        {
            HelperTraceIf(category, condition, string.Format(format, values));
        }

        private static void HelperTraceIf(TraceEventType category, bool condition, string message)
        {
            if (condition)
                traceSource.TraceEvent(category, 0, message);
        }

        private static void HelperTrace(TraceEventType category, string message)
        {
            HelperTraceIf(category, true, message);
        }

        #endregion
    }
}
