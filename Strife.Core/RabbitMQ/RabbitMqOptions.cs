namespace Strife.Core.RabbitMQ
{
    public class RabbitMqOptions
    {
        public const string RabbitMq = "RabbitMq";
        
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}