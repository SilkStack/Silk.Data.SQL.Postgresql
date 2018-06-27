using Silk.Data.SQL.Expressions;

namespace Silk.Data.SQL.Postgresql.Expressions
{
	public class PostgresqlRawSqlExpression : QueryExpression
	{
		public string SqlText { get; }

		public override ExpressionNodeType NodeType => ExpressionNodeType.Query;

		public PostgresqlRawSqlExpression(string sqlText)
		{
			SqlText = sqlText;
		}
	}
}
