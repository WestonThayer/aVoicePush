using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.ServiceRuntime;
using Services;
using System.Collections.Generic;
using System.Net.Http;

namespace EmailPusher
{
    public class WnsPushSender : IPushSender
    {
        private MobileServiceClient mobileService;

        public WnsPushSender()
        {
            mobileService = new MobileServiceClient(
                RoleEnvironment.GetConfigurationSettingValue("MobileServices.ApplicationUrl"),
                RoleEnvironment.GetConfigurationSettingValue("MobileServices.ApplicationKey")
                );
        }

        public bool Send(string userEmail, string sender, string body)
        {
            var args = new Dictionary<string, string>()
            {
                { "userEmail", userEmail },
                { "sender", sender },
                { "body", body }
            };

            // Invoke async, no need to wait
            mobileService.InvokeApiAsync("sendpush", HttpMethod.Post, args);

            return true;
        }
    }
}
