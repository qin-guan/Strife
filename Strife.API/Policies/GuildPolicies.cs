using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Strife.API.Policies
{
    public class GuildPolicies
    {
        public const string CreateChannels = "Guild/CreateChannels";
        public const string ReadChannels = "Guild/ReadChannels";
        public const string UpdateChannels = "Guild/UpdateChannels";
        public const string DeleteChannels = "Guild/DeleteChannels";

        public static List<string> GetAllPolicies()
        {
            var guildPolicies = new GuildPolicies();
            return guildPolicies
                .GetType()
                .GetFields(BindingFlags.Static)
                .Where(field => field.FieldType == typeof(string))
                .Select(field => (string) field.GetValue(null))
                .ToList();
        }
    }
}