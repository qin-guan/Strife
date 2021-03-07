namespace Strife.API.Contracts.Events.Guild
{
    public interface IGuildCreated
    {
        public string[] UserIds { get; }
        public string GuildId { get; }
    }
}