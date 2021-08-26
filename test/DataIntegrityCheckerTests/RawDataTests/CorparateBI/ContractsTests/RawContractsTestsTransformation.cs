using System.Collections.Generic;
using DataIntegrityCheckerTests.DataUtils;

namespace DataIntegrityCheckerTests.RawDataTests.CorparateBI.ContractsTests
{
	public class RawContactsTestsTransformation : ITransformation
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
				case "StartDate":
				case "EndDate":
				case "FinalDate":
				case "CancellationDate":
					value = AwsRawCellPreparationMapper.AsDate(value);
					break;

				case "CIGProcessed":
				case "IsValid":
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
				case "Account":
				case "Document":
				case "ContractTerm":
				case "ContractEventID":
					value = RawCellMapper.AsGuid(value); break;

				case "syscreated":
				case "sysmodified":
				case "CIGCopyTime":
				case "FinalDate":
				case "StartDate":
				case "EndDate":
				case "CancellationDate":
					value = RawCellMapper.AsDate(value); break;
			}
			return $"{value}\n";
		}
	}
}