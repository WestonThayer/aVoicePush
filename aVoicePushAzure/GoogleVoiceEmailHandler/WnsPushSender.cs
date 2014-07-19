using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVoiceEmailHandler
{
    public class WnsPushSender : IPushSender
    {
        public bool Send(Uri connection, string rawContent, string clientId, string clientSecret)
        {
            // Need Azure mobile sdk?
            /*WnsAccessTokenProvider provider = new WnsAccessTokenProvider(clientId, clientSecret);

            // We need to guess if the computer receives the notification or not
            var opts = new NotificationSendOptions();
            opts.RequestForStatus = true;
            // If we can't deliver the notificaton in a minute, don't bother?
            //opts.SecondsTTL = 60;

            var raw = NotificationsExtensions.RawContent.RawContentFactory.CreateRaw();
            raw.Content = rawContent;

            var toast = ToastContentFactory.CreateToastText02();
            toast.TextHeading.Text = sender;
            toast.TextBodyWrap.Text = body;
            toast.Audio.Content = ToastAudioContent.SMS;
            toast.Audio.Loop = false;

            ServiceLocator.Current.Log.Information("About to send a Toast to " + connection);

            NotificationSendResult result = raw.Send(connection, provider, opts);
            if (IsConnectionStringInvalid(result))
            {
                DeleteRow
            }
             
             */

            return false;
        }

        /// <summary>
        /// Check the result of sending a WNS notification and tell us if the
        /// given connection string is invalid.
        /// </summary>
        /// <param name="result">The result of a WNS operation</param>
        /// <returns>True if the connection string is invalid and useless</returns>
        /*private bool IsConnectionStringInvalid(NotificationSendResult result)
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
        }*/
    }
}
