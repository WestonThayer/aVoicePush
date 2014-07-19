using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVoiceEmailHandler
{
    /// <summary>
    /// Object to be serialized and sent as the push notification content.
    /// </summary>
    public sealed class Payload
    {
        public string Sender { get; set; }
        public string ThreadId { get; set; }
        public string Body { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Number { get; set; }
    }
}
