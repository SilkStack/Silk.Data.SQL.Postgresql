using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.SQL.Providers;
using Silk.Data.SQL.ProviderTests;

namespace Silk.Data.SQL.Postgresql.Tests
{
	[TestClass]
	public class PostgresqlTests : SqlProviderTests
	{
		public override IDataProvider CreateDataProvider(string connectionString)
		{
			return new PostgresqlDataProvider(connectionString);
		}

		public override void Dispose()
		{
			DataProvider.Dispose();
		}
	}
}
