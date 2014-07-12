using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ILog
    {
        void Information(string message);
        void Information(string format, params string[] values);
        void InformationIf(bool condition, string message);
        void InformationIf(bool condition, string format, params string[] values);
        void Warning(string message);
        void Warning(string format, params string[] values);
        void WarningIf(bool condition, string message);
        void WarningIf(bool condition, string format, params string[] values);
        void Error(string message);
        void Error(string format, params string[] values);
        void ErrorIf(bool condition, string message);
        void ErrorIf(bool condition, string format, params string[] values);
    }
}
