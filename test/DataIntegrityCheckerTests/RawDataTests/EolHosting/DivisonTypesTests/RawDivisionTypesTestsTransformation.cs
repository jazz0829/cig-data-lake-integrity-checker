using System.Collections.Generic;
using DataIntegrityCheckerTests.DataUtils;

namespace DataIntegrityCheckerTests.RawDataTests.EolHosting.DivisonTypesTests
{
	public class RawDivisionTypesTestsTransformation : ITransformation
	{
		public string PrepareAwsValue(
			string columnName, Dictionary<string, string> row)
		{
			var value = row[columnName.ToLowerInvariant()];

			switch (columnName)
			{
				case "syscreated":
				case "sysmodified":
				case "CIGCopyTime":
					value = AwsRawCellPreparationMapper.AsDate(value);
					break;

				case "CIGProcessed":
					value = AwsRawCellPreparationMapper.AsBool(value);
					break;

				case "Environment":
					value = AwsRawCellPreparationMapper.AsEnvironmentCode(value);
					break;
			}
			return $"{value}\n";
		}

		public string Execute(string columnName, object row)
		{
			var value = (row as IDictionary<string, object>)[columnName];

			switch (columnName)
			{
				case "ID":
				case "syscreator":
				case "sysmodifier":
				case "Item":
					value = RawCellMapper.AsGuid(value);
					break;

				case "Environment":
					value = RawCellMapper.AsEnvironmentCode(value);
					break;

				case "syscreated":
				case "sysmodified":
				case "CIGCopyTime":
					value = RawCellMapper.AsDate(value);
					break;
			}
			return $"{value}\n";
		}
	}
}