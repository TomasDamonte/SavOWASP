using Newtonsoft.Json;
using System.Collections.Generic;

namespace SavOWASP
{
    public class OWASPZAPReport
    {
        [JsonProperty("@version")]
        public string Version { get; set; }

        [JsonProperty("@generated")]
        public string Generated { get; set; }
        public List<Site> site { get; set; }
    }

    public class Instance
    {
        public string uri { get; set; }
        public string method { get; set; }
        public string param { get; set; }
        public string attack { get; set; }
        public string evidence { get; set; }
    }

    public class Alert
    {
        public string pluginid { get; set; }
        public string alertRef { get; set; }
        public string alert { get; set; }
        public string name { get; set; }
        public string riskcode { get; set; }
        public string confidence { get; set; }
        public string riskdesc { get; set; }
        public string desc { get; set; }
        public List<Instance> instances { get; set; }
        public string count { get; set; }
        public string solution { get; set; }
        public string otherinfo { get; set; }
        public string reference { get; set; }
        public string cweid { get; set; }
        public string wascid { get; set; }
        public string sourceid { get; set; }
    }

    public class Site
    {
        [JsonProperty("@name")]
        public string Name { get; set; }

        [JsonProperty("@host")]
        public string Host { get; set; }

        [JsonProperty("@port")]
        public string Port { get; set; }

        [JsonProperty("@ssl")]
        public string Ssl { get; set; }
        public List<Alert> alerts { get; set; }
    }

}
