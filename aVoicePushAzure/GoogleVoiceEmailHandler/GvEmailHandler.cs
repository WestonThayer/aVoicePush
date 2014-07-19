using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVoiceEmailHandler
{
    public class GvEmailHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="ArgumentException">On failure to parse some aspect of the email</exception>
        public void HandleEmail(IEmail email)
        {
            if (IsGoogleVoiceEmail(email.Header.From))
            {
                ServiceLocator.Current.Log.Information("IsGoogleVoiceEmail == true");

                // Extract the message
                Message message = GvEmailParser.ParseMessage(email);

                // Send it to all of the recipient's push endpoints
                SendMessage(message);
            }
            else if (IsEmailForwardPermissionMail(email.Header.From, email.Header.Subject))
            {
                ServiceLocator.Current.Log.Information("IsEmailForwardPermissionMail == true");

                string link = GvEmailParser.ParsePermission(email);
                string userEmail = GvEmailParser.ParsePermissionForUserEmail(email);
                bool clicked = ServiceLocator.Current.LinkClicker.Click(link);

                if (clicked && userEmail != null)
                {
                    ServiceLocator.Current.Log.Information("Email forward request successful.");

                    // Notify the user of success
                    Message message = new Message()
                    {
                        Body = "Awesome! aVoice Push has been configured correctly",
                        Number = "avoice",
                        Sender = "aVoice Push",
                        ThreadId = null,
                        Type = null,
                        UserEmail = userEmail
                    };

                    SendMessage(message);
                }
                else if (userEmail != null)
                {
                    ServiceLocator.Current.Log.Error("Failed to accept forward request. link: {0}. userEmail: {1}. clicked: {2}", link, userEmail, clicked ? "true" : "false");

                    // Ugh, we failed
                    Message message = new Message()
                    {
                        Body = "Uhoh, something broke.",
                        Number = "avoice",
                        Sender = "aVoice Push",
                        ThreadId = null,
                        Type = null,
                        UserEmail = userEmail
                    };

                    SendMessage(message);
                }
            }
            else
            {
                ServiceLocator.Current.Log.Warning("We've received an email that we don't have logic to handle. From: " + email.Header.From);
            }
        }

        /// <summary>
        /// Distribute the given message to all of the recipient's push endpoints.
        /// </summary>
        /// <param name="message"></param>
        private void SendMessage(Message message)
        {
            foreach (IItem item in ServiceLocator.Current.Item.Query(message.UserEmail))
            {
                WnsMessenger.NotifyUser(message, item, "bad", "bad");
            }
        }

        /// <summary>
        /// Checks to see if the given sender was Google Voice, either a SMS or a VM.
        /// </summary>
        /// <param name="sender">The original message sender's email address</param>
        /// <returns>True if from GV, false if else</returns>
        private bool IsGoogleVoiceEmail(string sender)
        {
            if (sender != null &&
                (sender.Contains("@txt.voice.google.com") || sender.Contains("voice-noreply@google.com")))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Test if the email is from Gmail's forward permission service. We determine this based
        /// on the From and Subject headers.
        /// </summary>
        /// <param name="from">Who the email is from</param>
        /// <param name="subject">Subject of the email</param>
        /// <returns>True if we believe that it is a forward permission message</returns>
        private bool IsEmailForwardPermissionMail(string from, string subject)
        {
            if (!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(subject) &&
                (from.Contains("mail-noreply@google.com") || from.Contains("forwarding-noreply@google.com")) &&
                subject.Contains("Gmail Forwarding Confirmation"))
            {
                return true;
            }

            return false;
        }
    }
}
