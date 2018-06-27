using Silk.Data.SQL.Postgresql.Expressions;

namespace Silk.Data.SQL.Postgresql
{
	public static class Postgresql
	{
		public static PostgresqlRawSqlExpression Raw(string sql)
		{
			return new PostgresqlRawSqlExpression(sql);
		}
	}
}
