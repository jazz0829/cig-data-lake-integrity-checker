namespace DataIntegrityCheckerTests.DataUtils
{
	public static class ColumnMappingExceptionHandler
	{
		public static bool HasToBeSkipped(
			string columnName,
			SourceOptions options)
		{
			return options != null &&
				   options.skip_columns != null &&
				   options.skip_columns.Contains(columnName);
		}

		public static ColumnType HandleOverrideColumnType(
			ColumnType columnType,
			SourceOptions options)
		{
			if (options == null ||
				options.override_column_type == null ||
				!options.override_column_type.ContainsKey(columnType.ColumnName))
			{
				return columnType;
			}
			var correctedType = options.override_column_type[columnType.ColumnName];

			return new ColumnType(columnType.ColumnName, correctedType, columnType.IsNullable);
		}

		public static ColumnType HandleOverrideColumnName(
			ColumnType columnType,
			SourceOptions options)
		{
			if (options == null ||
			    options.override_column_name == null ||
			    !options.override_column_name.ContainsKey(columnType.ColumnName))
			{
				return columnType;
			}
			var correctedName = options.override_column_name[columnType.ColumnName];

			return new ColumnType(correctedName, columnType.TypeName);
		}
	}
}
