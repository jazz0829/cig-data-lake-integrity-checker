using System;

namespace DataIntegrityCheckerTests.DataUtils
{
	public static class ColumnTypeMapper
	{
		public static ColumnType MapSqlToAthenaType(ColumnType sqlColumn)
		{
			var typeName = string.Empty;
			var sqlTypeName = sqlColumn.TypeName.Trim();
			switch (sqlTypeName)
			{
				case "uniqueidentifier":
				case "nvarchar":
				case "nchar":
				case "char":
					typeName = "varchar"; break;

				case "bit":
				case "varbinary":
					typeName = "boolean"; break;

				case "int":
				case "tinyint":
				case "smallint":
					typeName = "integer"; break;

				case "bigint":
					typeName = "bigint"; break;

				case "float":
					typeName = "double"; break;

				case "date":
					typeName = "date"; break;

				case "datetime":
				case "datetime2":
					typeName = "timestamp"; break;
			}

			if (string.IsNullOrEmpty(typeName))
			{
				throw new Exception($"Not yet handled type: {sqlColumn.TypeName}\nPlease extend the mapping");
			}

			var columnName = sqlColumn.ColumnName.ToLowerInvariant();

			return new ColumnType(columnName, typeName, sqlColumn.IsNullable);
		}
	}
}