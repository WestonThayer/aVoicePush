using EricDaugherty.CSES.Common;
using EricDaugherty.CSES.SmtpServer;
using Microsoft.WindowsAzure.ServiceRuntime;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EmailPusher
{
    /// <summary>
    /// This class is responsible for starting an SMTP server to listen for
    /// Google Voice emails forwarded from users.
    /// </summary>
    public class SmtpHandler
    {
        /// <summary>
        /// Used for watching SMTP port 25 (may not actually be 25 due to how Azure works).
        /// </summary>
        private TcpListener listener;
        /// <summary>
        /// Used for processing SMTP mail (this is the SMTP server).
        /// </summary>
        private SMTPProcessor processor;

        /// <summary>
        /// Create a new SmtpHandler.
        /// </summary>
        public SmtpHandler()
        {
            ServiceLocator.Current.Log.Information("SmtpHandler ctor, Port is: {0}. Domain is {1}.",
                RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["SmtpIn"].IPEndpoint.ToString(),
                RoleEnvironment.GetConfigurationSettingValue("DomainName"));

            listener = new TcpListener(RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["SmtpIn"].IPEndpoint);
            listener.ExclusiveAddressUse = false;
            processor = new SMTPProcessor(
                RoleEnvironment.GetConfigurationSettingValue("DomainName"),
                new RecipientFilter(),
                new MessageSpool()
                );

            ServiceLocator.Current.Log.Information("SmtpHandler ctor, exiting");
        }

        public void Run()
        {
            ServiceLocator.Current.Log.Information("SmtpHandler Run() entry, starting to listen");
            listener.Start();

            while (true)
            {
                Socket soc = listener.AcceptSocket();
                ServiceLocator.Current.Log.Information("Socket accepted, about to process connection");

                Task.Run(() =>
                {
                    processor.ProcessConnection(soc);
                    ServiceLocator.Current.Log.Information("Socket processing finished");
                });

                ServiceLocator.Current.Log.Information("Thread spawned, accepting next socket");
            }
        }
    }

    public class MessageSpool : IMessageSpool
    {
        public MessageSpool()
        {
        }

        public bool SpoolMessage(SMTPMessage message)
        {
            ServiceLocator.Current.Log.Information("Email encountered");

            try
            {
                using (SqlConnection conn = new SqlConnection(RoleEnvironment.GetConfigurationSettingValue("SqlConnectionString")))
                {
                    conn.Open();

                    WnsMessenger messenger = new WnsMessenger(
                        conn,
                        RoleEnvironment.GetConfigurationSettingValue("ClientId"),
                        RoleEnvironment.GetConfigurationSettingValue("ClientSecret")
                        );

                    GvEmailHandler handler = new GvEmailHandler();
                    handler.ProcessEmail(message, messenger);
                }
            }
            catch (Exception e)
            {
                // We can survive an Exception, since it only breaks this message's processing. Continue to strive for others.
                ServiceLocator.Current.Log.Warning("Exception in MessageSpool! " + e.Message);
            }

            return true;
        }
    }

    public class RecipientFilter : IRecipientFilter
    {
        public bool AcceptRecipient(SMTPContext context, EmailAddress recipient)
        {
            bool match = recipient.Username.ToLower() == "text";

            ServiceLocator.Current.Log.WarningIf(!match, "Received mail not to text@avoice.cloudapp.net. Instead, " + recipient.ToString());

            return match;
        }
    }
}
