using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVoiceEmailHandler
{
    /// <summary>
    /// Object representing the crucial bits of a Google Voice message.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Email of the user this message was sent to.
        /// </summary>
        public string UserEmail { get; set; }
        /// <summary>
        /// Content of this message.
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// Name or other human readable identifier for who sent the message.
        /// </summary>
        public string Sender { get; set; }
        /// <summary>
        /// Identifier for what message thread this belongs to.
        /// </summary>
        public string ThreadId { get; set; }
        /// <summary>
        /// Phone number of who sent the message.
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// Description of what kind of message was left.
        /// </summary>
        public string Type { get; set; }
    }
}
