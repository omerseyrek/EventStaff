using MassTransit;

namespace EventStaf.Infra.Configuration
{
	public class ApplicationConfiguration
	{
		public required ConnectionStrings ConnectionStrings { get; set; }

		public required MassTransitConfig MassTransit { get; set; }

		public required Logging Logging { get; set; }

		public required Jwt Jwt { get; set; }
	}


	public class MassTransitConfig
	{
		public required string Host { get; set; }
		public required int Port { get; set; }
		public required string VirtualHost { get; set; }
		public required string Username { get; set; }
		public required string Password { get; set; }
	}


	public class ConnectionStrings
	{
		public required string DefaultConnection { get; set; }
		public required string DevelopmentConnection { get; set; }
		public required string RedisGrpc { get; set; }
		public required string RedisHttp { get; set; }
		public required string Redis { get; set; }

	}

	public class Logging
	{
		public required LogLevel LogLevel { get; set; }
	}

	public class LogLevel
	{
		public required string Default { get; set; }
	}


	public class Jwt
	{
		public required string Key { get; set; }
		public required string Issuer { get; set; }
		public required string Audience { get; set; }
		public required int ExpireInMinues { get; set; }
	}
}
