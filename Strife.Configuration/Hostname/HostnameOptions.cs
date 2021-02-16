using System;
using System.Net.NetworkInformation;

namespace Strife.Configuration.Hostname
{
    public enum Hostname
    {
        Api,
        Auth,
        Web
    }
    public class HostnameOptions
    {
        public const string Hostnames = "Hostnames";
        public string Api { get; set; }
        public string Auth { get; set; }
        public string Web { get; set; }
        
        public string GetHostnameWithPath(Hostname hostname, string path)
        {
            return hostname switch
            {
                Hostname.Api => string.IsNullOrWhiteSpace(Api) ? throw new Exception("No configured API hostname") : $"{Api}{path}",
                Hostname.Auth => string.IsNullOrWhiteSpace(Auth) ? throw new Exception("No configured Auth hostname") : $"{Auth}{path}",
                Hostname.Web => string.IsNullOrWhiteSpace(Web) ? throw new Exception("No configured Web hostname") : $"{Web}{path}",
                _ => throw new IndexOutOfRangeException("Invalid hostname")
            };
        }

        public Uri GetUriHostnameWithPath(Hostname hostname, string path)
        {
            return new(GetHostnameWithPath(hostname, path));
        }
    }
}