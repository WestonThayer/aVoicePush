using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GoogleVoiceEmailHandler
{
    public class WnsMessenger
    {
        /// <summary>
        /// Send the Message to the given connection represented by the Item row.
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="item">Item row for the push connection</param>
        /// <param name="clientId">The ClientID string for the receiving app</param>
        /// <param name="clientSecret">The ClientSecret string for the receiving app</param>
        /// <returns>True on success, false on failure</returns>
        public static bool NotifyUser(Message message, IItem item, string clientId, string clientSecret)
        {
            if (message == null)
                throw new ArgumentException("message");
            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentException("clientId");
            if (string.IsNullOrWhiteSpace(clientSecret))
                throw new ArgumentException("clientSecret");

            // Serialize our message
            var serializer = new XmlSerializer(typeof(Payload));
            var payload = new Payload
            {
                Sender = message.Sender,
                ThreadId = message.ThreadId,
                Body = message.Body,
                TimeStamp = DateTime.Now,
                Number = message.Number
            };
            var stream = new StringWriter();
            serializer.Serialize(stream, payload);
            string p = stream.ToString();

            return ServiceLocator.Current.PushSender.Send(new Uri(item.PushConnectionString), p, clientId, clientSecret);
        }
    }
}
