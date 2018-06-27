using System.Data.Common;
using System.Threading.Tasks;
using Npgsql;
using Silk.Data.SQL.Providers;
using Silk.Data.SQL.Queries;

namespace Silk.Data.SQL.Postgresql
{
	public class PostgresqlDataProvider : DataProviderCommonBase
	{
		public const string PROVIDER_NAME = "postgresql";
		private readonly string _connectionString;

		public override string ProviderName => PROVIDER_NAME;

		public PostgresqlDataProvider(string connectionString)
		{
			_connectionString = connectionString;
		}

		public PostgresqlDataProvider(NpgsqlConnectionStringBuilder connectionStringBuilder) :
			this(connectionStringBuilder.ConnectionString) { }

		public PostgresqlDataProvider(string hostname, string database, string username, string password) :
			this(new NpgsqlConnectionStringBuilder
			{
				Host = hostname,
				Database = database,
				Username = username,
				Password = password
			}) { }

		public override void Dispose()
		{
		}

		protected override DbConnection Connect()
		{
			var connection = new NpgsqlConnection(_connectionString);
			connection.Open();
			return connection;
		}

		protected override async Task<DbConnection> ConnectAsync()
		{
			var connection = new NpgsqlConnection(_connectionString);
			await connection.OpenAsync();
			return connection;
		}

		protected override IQueryConverter CreateQueryConverter()
		{
			return new PostgresqlQueryConverter();
		}

#if DEBUG
		public override DbCommand CreateCommand(DbConnection connection, SqlQuery sqlQuery)
		{
			//  overridden in DEBUG builds to provide a useful breakpoint placement to look at raw SQL
			return base.CreateCommand(connection, sqlQuery);
		}
#endif
	}
}
