using System.Collections.Generic;
using DataIntegrityCheckerTests.DataUtils;

namespace DataIntegrityCheckerTests.RawDataTests.EolHosting.LeadSourcesTests
{
	public class RawLeadSourcesTestsTransformation : ITransformation
	{
		public string PrepareAwsValue(
			string columnName, Dictionary<string, string> row)
		{
			var value = row[columnName.ToLowerInvariant()];

			switch (columnName)
			{
				case "CIGCopyTime":
				case "syscreated":
				case "sysmodified":
					value = AwsRawCellPreparationMapper.AsDate(value);
					break;

				case "CIGProcessed":
					value = AwsRawCellPreparationMapper.AsBool(value);
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
					value = RawCellMapper.AsGuid(value); break;

				case "CIGCopyTime":
				case "syscreated":
				case "sysmodified":
					value = RawCellMapper.AsDate(value); break;
			}
			return $"{value}\n";
		}
	}
}