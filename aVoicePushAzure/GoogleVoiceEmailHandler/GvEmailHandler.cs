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

                Message message = GvEmailParser.ParseMessage(email);
            }
            else if (IsEmailForwardPermissionMail(email.Header.From, email.Header.Subject))
            {
                ServiceLocator.Current.Log.Information("IsEmailForwardPermissionMail == true");

                string link = GvEmailParser.ParsePermission(email);

                sdf
            }
            else
            {
                ServiceLocator.Current.Log.Error("We've received an email that we don't have logic to handle.");
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
