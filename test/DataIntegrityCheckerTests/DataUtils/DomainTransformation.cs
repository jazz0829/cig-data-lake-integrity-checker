using System.Collections.Generic;

namespace DataIntegrityCheckerTests.DataUtils
{
	public class DomainTransformation
	{
		private readonly IList<string> guidColumns = new List<string> { "uniqueidentifier" };
		private readonly IList<string> dateColumns = new List<string> { "datetime", "datetime2", "date" };
		private readonly IList<string> boolColumns = new List<string> { "bit", "varbinary" /*experiment*/, "boolean" };
		private readonly IList<string> doubleColumns = new List<string> { "float" };
		private readonly IList<string> toTrimTextColumns = new List<string> { "char", "nchar", "nvarchar" };

		public string PrepareAwsValue(
			string awsColumnName,
			ColumnType sqlColumn,
			Dictionary<string, string> row)
		{
			var value = row[awsColumnName.ToLowerInvariant()];
			switch (sqlColumn.TypeName)
			{
				case string c when dateColumns.Contains(c):
					value = AwsDomainCellPreparationMapper
						.AsDate(value);
					break;

				case string c when boolColumns.Contains(c):
					value = AwsDomainCellPreparationMapper
						.AsBool(value);
					break;

				case string c when doubleColumns.Contains(c):
					value = AwsDomainCellPreparationMapper
						.AsFloat(value);
					break;

				case string c when toTrimTextColumns.Contains(c):
					value = AwsDomainCellPreparationMapper
						.AsTrimmedString(value);
					break;
			}
			return $"{value}\n";
		}

		public string TransformSqlValueToAws(
			ColumnType sqlColumn,
			object row)
		{
			var value = (row as IDictionary<string, object>)[sqlColumn.ColumnName];
			switch (sqlColumn.TypeName)
			{
				case string c when guidColumns.Contains(c):
					value = DomainCellMapper
						.AsGuid(value);
					break;

				case string c when dateColumns.Contains(c):
					value = DomainCellMapper
						.AsDate(value);
					break;

				case string c when boolColumns.Contains(c):
					value = DomainCellMapper
						.AsBool(value);
					break;

				case string c when doubleColumns.Contains(c):
					value = DomainCellMapper
						.AsFloat(value);
					break;

				case string c when toTrimTextColumns.Contains(c):
					value = DomainCellMapper
						.AsTrimmedString(value);
					break;
			}

			if (sqlColumn.ColumnName == "Environment")
			{
				value = DomainCellMapper.AsEnvironmentCode(value);
			}

			return $"{value}\n";
		}
	}
}
