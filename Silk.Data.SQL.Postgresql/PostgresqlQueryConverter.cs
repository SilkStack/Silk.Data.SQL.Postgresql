using System.Linq;
using System.Text;
using Silk.Data.SQL.Expressions;
using Silk.Data.SQL.Postgresql.Expressions;
using Silk.Data.SQL.Queries;

namespace Silk.Data.SQL.Postgresql
{
	public class PostgresqlQueryConverter : QueryConverterCommonBase
	{
		protected override string ProviderName => PostgresqlDataProvider.PROVIDER_NAME;

		protected override string AutoIncrementSql => "";

		public PostgresqlQueryConverter()
		{
			ExpressionWriter = new PostgresqlQueryWriter(Sql, this);
		}

		protected override void WriteFunctionToSql(QueryExpression queryExpression)
		{
			switch (queryExpression)
			{
				case RandomFunctionExpression randomFunctionExpression:
					Sql.Append($"FLOOR(RANDOM() * {int.MaxValue})::int");
					return;
				case LastInsertIdFunctionExpression lastInsertIdExpression:
					Sql.Append("LASTVAL()");
					return;
				case TableExistsVirtualFunctionExpression tableExistsExpression:
					ExpressionWriter.Visit(
						Postgresql.Raw($@"SELECT 1 
   FROM   pg_catalog.pg_class c
   JOIN   pg_catalog.pg_namespace n ON n.oid = c.relnamespace
   WHERE  n.nspname = current_schema()
   AND    c.relname = '{tableExistsExpression.Table.TableName.Replace("'", "\\'")}'
   AND    c.relkind = 'r'    -- only tables")
						);
					return;
			}
			base.WriteFunctionToSql(queryExpression);
		}

		protected override string GetDbDatatype(SqlDataType sqlDataType)
		{
			switch (sqlDataType.BaseType)
			{
				case SqlBaseType.Guid: return "uuid";
				case SqlBaseType.TinyInt: return "int2";
				case SqlBaseType.SmallInt: return "int2";
				case SqlBaseType.Int: return "int4";
				case SqlBaseType.BigInt: return "int8";
				case SqlBaseType.Float: return $"float({sqlDataType.Parameters[0]})";
				case SqlBaseType.Bit: return "boolean";
				case SqlBaseType.Decimal: return $"numeric({sqlDataType.Parameters[0]}, {sqlDataType.Parameters[1]})";
				case SqlBaseType.Date: return "date";
				case SqlBaseType.Time: return "time";
				case SqlBaseType.DateTime: return "timestamp";
				case SqlBaseType.Text: return sqlDataType.Parameters.Length > 0 ? $"varchar({sqlDataType.Parameters[0]})" : "text";
				case SqlBaseType.Binary: return "bytea";
			}
			throw new System.NotSupportedException($"SQL data type not supported: {sqlDataType.BaseType}.");
		}

		protected override string QuoteIdentifier(string schemaComponent)
		{
			if (schemaComponent == "*")
				return "*";
			return $"\"{schemaComponent}\"";
		}

		private class PostgresqlQueryWriter : QueryWriter
		{
			public new PostgresqlQueryConverter Converter { get; }

			public PostgresqlQueryWriter(StringBuilder sql, PostgresqlQueryConverter converter) : base(sql, converter)
			{
				Converter = converter;
			}

			protected override void VisitColumnDefinition(QueryExpression queryExpression)
			{
				if (queryExpression is ColumnDefinitionExpression columnDefinitionExpression)
				{
					Sql.Append(Converter.QuoteIdentifier(columnDefinitionExpression.ColumnName));
					Sql.Append(" ");
					var dbType = Converter.GetDbDatatype(columnDefinitionExpression.DataType);
					if (columnDefinitionExpression.IsAutoIncrement)
					{
						switch (dbType)
						{
							case "int2": dbType = "serial2"; break;
							case "int4": dbType = "serial4"; break;
							case "int8": dbType = "serial8"; break;
						}
					}
					Sql.Append(dbType);
					if (!columnDefinitionExpression.IsNullable)
					{
						Sql.Append(" NOT NULL");
					}
				}
			}

			protected override void VisitQuery(QueryExpression queryExpression)
			{
				switch (queryExpression)
				{
					case PostgresqlRawSqlExpression rawExpression:
						Sql.Append(rawExpression.SqlText);
						break;
					default:
						base.VisitQuery(queryExpression);
						break;
				}
			}
		}
	}
}
