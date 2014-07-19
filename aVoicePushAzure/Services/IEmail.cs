using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEmail
    {
        IEmailHeader Header { get; }
        string Data { get; }
        string RawBody { get; }
    }
}
