using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVoiceEmailHandler
{
    public class WnsMessenger
    {
        private string clientId;
        private string clientSecret;
        private SqlConnection conn;

        /// <summary>
        /// Creates a new WnsMessenger that can operate on the given SQL db, which is assumed to:
        /// * Have a table called Item
        /// * In Item, have columns named email and wnsConnection
        /// </summary>
        /// <param name="connection">A connection to the SQL DB</param>
        /// <param name="clientId">The app's client id for sending notifications</param>
        /// <param name="clientSecret">The app's client secret for sending WNS notifications</param>
        public WnsMessenger(SqlConnection connection, string clientId, string clientSecret)
        {
            conn = connection;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }

        /// <summary>
        /// Send the given user a WNS notificaton.
        /// </summary>
        /// <param name="userEmail">The email of the user so that we can look it up</param>
        /// <param name="body">The body of the message to the user</param>
        /// <param name="sender">The sender of the message to the user</param>
        /// <param name="threadId">The unique ID associated with the message's thread</param>
        /// <param name="number">The number of the sender</param>
        /// <returns></returns>
        public bool NotifyUser(string userEmail, string body, string sender, string threadId, string number)
        {
            SqlCommand comm = new SqlCommand(
                @"SELECT * FROM [avoice].[Item] WHERE email = '" + userEmail + "'",
                conn);

            using (SqlDataReader reader = comm.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    if (sender != null && body != null)
                    {
                        List<string> wnsConnections = new List<string>();
                        List<string> rowIds = new List<string>();

                        // Send a push to each entry found
                        while (reader.Read())
                        {
                            wnsConnections.Add(reader["pushConnectionString"].ToString());
                            rowIds.Add(reader["id"].ToString());
                        }

                        // Close the DataReader so that we can delete if need be
                        reader.Close();

                        for (int i = 0; i < wnsConnections.Count; i++)
                        {
                            string wnsConn = wnsConnections[i];

                            NotificationSendResult result = SendWnsRawNotification(sender, body, threadId, number, wnsConn);

                            // Need to act on the result better for stale notifications

                            // Delete the row if the connection string is wrong
                            if (IsConnectionStringInvalid(result))
                                DeleteRow(conn, wnsConn, rowIds[i]);
                        }
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Send a type 2 Toast notification. The bold will be the sender's name. The rest will be what of
        /// the message body that fits in the toast.
        /// </summary>
        /// <param name="sender">The person who sent this text</param>
        /// <param name="body">The message that they sent</param>
        /// <param name="threadId">The thread that this is a part of</param>
        /// <param name="number">The number of the contact</param>
        /// <param name="connection">The WNS connection string to send it to</param>
        /// <returns>The result of sending the toast</returns>
        private NotificationSendResult SendWnsRawNotification(string sender, string body, string threadId, string number, string connection)
        {
            WnsAccessTokenProvider provider = new WnsAccessTokenProvider(clientId, clientSecret);

            // We need to guess if the computer receives the notification or not
            var opts = new NotificationSendOptions();
            opts.RequestForStatus = true;
            // If we can't deliver the notificaton in a minute, don't bother?
            //opts.SecondsTTL = 60;

            // Serialize our message
            var serializer = new XmlSerializer(typeof(Payload));
            var payload = new Payload
            {
                Sender = sender,
                ThreadId = threadId,
                Body = body,
                TimeStamp = DateTime.Now,
                Number = number
            };
            var stream = new StringWriter();
            serializer.Serialize(stream, payload);
            string p = stream.ToString();

            var raw = NotificationsExtensions.RawContent.RawContentFactory.CreateRaw();
            raw.Content = p;

            /*var toast = ToastContentFactory.CreateToastText02();
            toast.TextHeading.Text = sender;
            toast.TextBodyWrap.Text = body;
            toast.Audio.Content = ToastAudioContent.SMS;
            toast.Audio.Loop = false;*/

            Tr.Information("About to send a Toast to " + connection);

            return raw.Send(new Uri(connection), provider, opts);
        }

        /// <summary>
        /// Check the result of sending a WNS notification and tell us if the
        /// given connection string is invalid.
        /// </summary>
        /// <param name="result">The result of a WNS operation</param>
        /// <returns>True if the connection string is invalid and useless</returns>
        private bool IsConnectionStringInvalid(NotificationSendResult result)
        {
            bool isInvalid = false;

            // See http://msdn.microsoft.com/en-us/library/windows/apps/hh465435.aspx#sending_a_notification_request_and_receiving_a_response
            switch (result.StatusCode)
            {
                case HttpStatusCode.NotFound:
                case HttpStatusCode.Gone:
                    isInvalid = true;
                    break;
                default:
                    break;
            }

            Tr.Information("WNS response code: " + result.StatusCode.ToString());

            return isInvalid;
        }

        private void DeleteRow(SqlConnection conn, string wnsConn, string rowId)
        {
            SqlCommand del = new SqlCommand(@"DELETE FROM [avoice].[Item] " +
                "WHERE pushConnectionString = '" + wnsConn + "' AND " +
                "id = " + rowId,
                conn);
            del.ExecuteNonQuery();
        }
    }
}
