using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GoogleVoiceEmailHandler
{
    public class GvEmailParser
    {
        /// <summary>
        /// Find a Google Voice Message from the email given.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>A new Message</returns>
        /// <exception cref="ArgumentException">If some part of the email can't be parsed</exception>
        public static Message ParseMessage(IEmail email)
        {
            Message result = new Message();
            result.UserEmail = ParseUserEmail(email);
            result.Body = ParseBody(email);

            string sender, number, type;
            ParseSenderAndNumberAndType(email, out sender, out number, out type);
            result.Sender = sender;
            result.Number = number;
            result.Type = type;

            result.ThreadId = ParseThreadId(email);

            return result;
        }

        /// <summary>
        /// Look at the Delivered-To header for the Google Voice user's email address.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>The user email. Throws ArgumentException on failure</returns>
        private static string ParseUserEmail(IEmail email)
        {
            string userEmail = email.Header.DeliveredTo;

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                ServiceLocator.Current.Log.Information("Google user email determined as: " + userEmail);
                return userEmail;
            }
            else
            {
                throw new ArgumentException("Blank Delivered-To header");
            }
        }

        /// <summary>
        /// Take the body section of a GV message and clean GV's crap out of it, leaving just what
        /// somebody typed.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>A cleaned body, just what the user is interested in or null on failure</returns>
        private static string ParseBody(IEmail email)
        {
            string body = email.RawBody.Trim();

            if (string.IsNullOrEmpty(body))
            {
                ServiceLocator.Current.Log.Error("Email body claims to be " + body == null ? "null" : "empty");
                return null;
            }

            // To clean up the body, we need to check for:
            // * An SMS that is received for the first time will have an ugly footer
            // * A missed call will have HTML formatting, and so will a VM, so use a regex

            // There's more, but we'll say this is enough to match
            string smsFooter = "--\r\nSent using SMS-to-email";

            if (body.Contains(smsFooter))
            {
                // cut it out
                body = body.Remove(body.IndexOf(smsFooter));
            }

            Regex missedCall = new Regex(@"Missed call from:.+");
            Regex voicemail = new Regex(@"Voicemail from: .+ at .+Transcript: (.+)Play message:", RegexOptions.Singleline);

            if (missedCall.IsMatch(body))
            {
                var m = missedCall.Match(body);
                string raw = m.Groups[0].Value;
                raw = raw.Replace("<b>", "");
                body = raw.Replace("</b>", "");
            }
            else if (voicemail.IsMatch(body))
            {
                var m = voicemail.Match(body);
                body = m.Groups[1].Value;
                body = body.Replace("  \r\n", " "); // trim out the weird new lines
            }

            // Always trim off any trailing new lines surrounding the message
            return body.Trim();
        }

        /// <summary>
        /// Operate on the "From" header and get the actual GV sender's name in the case of SMS.
        /// Otherwise, we need to look at the body of the message for missed calls and voicemails.
        /// </summary>
        /// <param name="email">A GV mail</param>
        /// <returns>Valid sender, number, and type</returns>
        /// <exception cref="ArgumentException">If we cannot parse the sender</exception>
        private static void ParseSenderAndNumberAndType(IEmail email, out string sender, out string number, out string type)
        {
            // There are several possible forms of the From header:
            // 
            // SMS from known contact: "Cryclops Test (SMS)" <14108499375.14108499138.btjW8QZCSl@txt.voice.google.com>
            // SMS from unknown contact: "(410) 849-9138" <14108499375.14108499138.btjW8QZCSl@txt.voice.google.com>
            // SMS from unknown contact with a short number: 262966 <12345678888.262966.LFQ2vLRWWR@txt.voice.google.com>
            // Missed call/Voicemail: Google Voice <voice-noreply@google.com>
            //
            // In this last case, the vital information is in the body of the message, which can look like this:
            //
            // Missed call from unknown contact: Missed call from:  (410) 849-9138 at 12:06 PM
            // Missed call from known contact: Missed call from: Cryclops Tester (410) 849-9138 at 12:20 PM
            //
            // Voicemail from unknown contact:
            // Voicemail from known contact: 

            sender = email.Header.From;

            if (sender == @"Google Voice <voice-noreply@google.com>")
            {
                Regex subjectRegex = new Regex(@".+ from (.+) at");

                if (subjectRegex.IsMatch(email.Header.Subject))
                {
                    var m = subjectRegex.Match(email.Header.Subject);
                    sender = m.Groups[1].Value;

                    Regex typeRegex = new Regex(@"(.+) from: .+ at");

                    // It's probably a missed call or voicemail
                    if (typeRegex.IsMatch(email.RawBody))
                    {
                        m = typeRegex.Match(email.RawBody);
                        type = m.Groups[1].Value;

                        Regex numberRegex = new Regex(@".+ from: " + Regex.Escape(sender) + " (.+) at");

                        if (numberRegex.IsMatch(email.RawBody))
                        {
                            m = numberRegex.Match(email.RawBody);
                            number = ParseNumber(m.Groups[1].Value);
                        }
                        else
                        {
                            number = ParseNumber(sender);
                        }

                        return;
                    }
                }
            }
            else
            {
                type = "SMS";
                Regex r1 = new Regex(@"""(.+) \(SMS\)"" <.+@txt\.voice\.google\.com>");
                Regex r2 = new Regex(@"""(.+)"" <.+@txt\.voice\.google\.com>");
                Regex r3 = new Regex(@"(.+) <.+@txt\.voice\.google\.com>");

                if (r1.IsMatch(sender))
                {
                    var m = r1.Match(sender);
                    number = ParseNumber(sender);
                    sender = m.Groups[1].Value;

                    return;
                }
                else if (r2.IsMatch(sender))
                {
                    var m = r2.Match(sender);
                    number = ParseNumber(sender);
                    sender = m.Groups[1].Value;

                    return;
                }
                else if (r3.IsMatch(sender))
                {
                    var m = r3.Match(sender);
                    number = m.Groups[1].Value;
                    sender = m.Groups[1].Value;

                    return;
                }
            }

            throw new ArgumentException("Unknown sender: " + PurgePersonalString(sender));
        }

        /// <summary>
        /// Remove any personal data from a string so that we can safely log it.
        /// 
        /// Replace any alphanumeric with a 'a'. Replace any digit with a '0'.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string PurgePersonalString(string data)
        {
            if (data == null)
                return "null";

            Regex r = new Regex(@"[a-zA-Z]");
            data = r.Replace(data, "a");

            r = new Regex(@"[0-9]");
            return r.Replace(data, "0");
        }

        /// <summary>
        /// Look for the number of the sender.
        /// </summary>
        /// <param name="fromHeader">The contents of the From header</param>
        /// <returns>The number in +12223334444 format</returns>
        /// <exception cref="ArgumentException">If we cannot parse the given string</exception>
        private static string ParseNumber(string dirtyNumber)
        {
            // We'll take any of the following formats:
            // "Cryclops Testing (SMS)" <14108499375.14108499138.btjW8QZCSl@txt.voice.google.com>
            // Any 11 digit number
            // Any 10 digit number

            Regex r1 = new Regex(@"<\d{11}\.(\d{11})\..+@");

            if (r1.IsMatch(dirtyNumber))
            {
                var m = r1.Match(dirtyNumber);
                return "+" + m.Groups[1].Value;
            }
            else
            {
                int digitCount = dirtyNumber.Count(c => Char.IsDigit(c));

                if (digitCount == 10 || digitCount == 11)
                {
                    string number = new String(dirtyNumber.Where(c => Char.IsDigit(c)).ToArray());

                    if (digitCount == 11)
                        return "+" + number;
                    else if (digitCount == 10)
                        return "+1" + number;
                }
            }

            throw new ArgumentException("dirtyNumber is unmatched: " + PurgePersonalString(dirtyNumber));
        }

        /// <summary>
        /// Look for the Google thread ID for the sms in the given References or Message-ID header.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>The Google thread ID, or null if not found</returns>
        private static string ParseThreadId(IEmail email)
        {
            string threadHeader = email.Header.References != null ? email.Header.References : email.Header.MessageId;

            if (threadHeader != null)
            {
                // We expect the header to be as follows:
                // <+14108499375.3c3e7802ca1fe3436a066df575c79ab9f1ba039c@txt.voice.google.com>

                Regex r = new Regex(@"\.(.+)@txt\.voice\.google\.com");
                if (r.IsMatch(threadHeader))
                {
                    var m = r.Match(threadHeader);
                    return m.Groups[1].Value;
                }
            }

            ServiceLocator.Current.Log.Error(
                "Failed to parse ThreadID. References: {0}; Message-ID: {1}",
                PurgePersonalString(email.Header.References),
                PurgePersonalString(email.Header.MessageId));

            return null;
        }

        /// <summary>
        /// When adding a forwarder in Gmail, Google first sends a forward permission request email
        /// to the potential forwarder. We need to find the link in it to enable the forwarding pipe.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>The link to enable forwarding</returns>
        /// <exception cref="ArgumentException">If we can't find the link</exception>
        public static string ParsePermission(IEmail email)
        {
            // We should click the link to allow forwarding
            Regex r = new Regex(@"link below to confirm the request:\s+(https://.+)");

            if (r.IsMatch(email.RawBody))
            {
                var m = r.Match(email.Data);
                string link = m.Groups[1].Value.Trim();

                return link;
            }
            else
            {
                throw new ArgumentException("Can't find a link to click! (regex fail): " + PurgePersonalString(email.RawBody));
            }
        }

        /// <summary>
        /// Given an email forwarding permission mail, find the email of the user who
        /// requested it.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>The email, or null if not found</returns>
        public static string ParsePermissionForUserEmail(IEmail email)
        {
            Regex r = new Regex(@"(.+@.+) has requested to automatically forward");

            if (r.IsMatch(email.RawBody))
            {
                var m = r.Match(email.RawBody);
                return m.Groups[1].Value;
            }
            else
            {
                ServiceLocator.Current.Log.Error("Regex match fail in ParsePermissionForUserEmail. Body: " + PurgePersonalString(email.RawBody));
                return null;
            }
        }
    }
}
