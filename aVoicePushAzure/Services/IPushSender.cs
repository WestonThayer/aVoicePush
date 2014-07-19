using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// Interface to allow the actual sending a toast step to be mocked.
    /// </summary>
    public interface IPushSender
    {
        bool Send(Uri connection, string rawContent, string clientId, string clientSecret);
    }
}
