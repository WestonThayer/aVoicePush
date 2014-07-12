using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVoiceEmailHandler
{
    public interface IEmail
    {
        IHeader Header { get; set; }
        string Data { get; set; }
        string RawBody { get; set; }

        public interface IHeader
        {
            string From { get; set; }
            string DeliveredTo { get; set; }
            string References { get; set; }
            string MessageId { get; set; }
            string Subject { get; set; }
        }
    }
}
