using System.Collections.Generic;
using DataIntegrityCheckerTests.DataUtils;

namespace DataIntegrityCheckerTests.RawDataTests.EolHosting.DivisionsTests
{
	public class RawDivisionsTransformation : ITransformation
	{
		public string PrepareAwsValue(
			string columnName, Dictionary<string, string> row)
		{
			var value = row[columnName.ToLowerInvariant()];
			switch (columnName)
			{
				case "syscreated":
				case "sysmodified":
				case "StartDate":
				case "EndDate":
				case "CIGCopyTime":
					value = AwsRawCellPreparationMapper.AsDate(value);
					break;
				case "Deleted":
					value = AwsRawCellPreparationMapper
						.AsDateTimeWithNoMilliseconds(value);
					break;
				case "Main":
				case "CustomerPortal":
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
				case "Account":
				case "Customer":
				case "ActivitySector":
				case "BusinessType":
				case "CompanySize":
				case "ActivitySubSector":
				case "Deleter":
				case "syscreator":
				case "sysmodifier":
					value = RawCellMapper.AsGuid(value);
					break;
				case "syscreated":
				case "sysmodified":
				case "StartDate":
				case "EndDate":
				case "CIGCopyTime":
					value = RawCellMapper.AsDate(value);
					break;
				case "Deleted":
					value = RawCellMapper
						.AsDateTimeWithNoMilliseconds(value);
					break;
			}
			return $"{value}\n";
		}
	}
}