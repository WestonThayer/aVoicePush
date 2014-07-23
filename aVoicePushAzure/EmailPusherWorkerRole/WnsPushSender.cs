using Microsoft.WindowsAzure.ServiceRuntime;
using Newtonsoft.Json.Linq;
using Services;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace EmailPusher
{
    public class WnsPushSender : IPushSender
    {
        public WnsPushSender()
        {
        }

        public bool Send(string userEmail, string sender, string body)
        {
            string appUrl = RoleEnvironment.GetConfigurationSettingValue("MobileServices.ApplicationUrl");
            string appKey = RoleEnvironment.GetConfigurationSettingValue("MobileServices.ApplicationKey");

            JObject json = new JObject();
            json.Add("userEmail", userEmail);
            json.Add("sender", sender);
            json.Add("body", body);

            var request = (HttpWebRequest)WebRequest.Create(appUrl + "api/sendpush");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers.Add("X-ZUMO-MASTER", appKey);
            
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json.ToString());
                streamWriter.Flush();
                streamWriter.Close();
            }

            var response = (HttpWebResponse)request.GetResponse();

            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}
