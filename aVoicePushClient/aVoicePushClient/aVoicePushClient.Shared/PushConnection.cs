using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace aVoicePushClient
{
    public class PushConnection
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "userEmail")]
        public string UserEmail { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
    }
}
