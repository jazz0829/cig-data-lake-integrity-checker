using System.Collections.Generic;
using DataIntegrityCheckerTests.DataUtils;

namespace DataIntegrityCheckerTests.RawDataTests.EolHosting.AccountsClassificationsTests
{
	public class RawAccountsClassificationsTransformation : ITransformation
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
			}
			return $"{value}\n";
		}

		public string Execute(string columnName, object row)
		{
			var value = (row as IDictionary<string, object>)[columnName];

			switch (columnName)
			{
				case "ID":
				case "CreditManagementScenario":
				case "syscreator":
				case "sysmodifier":
					value = RawCellMapper.AsGuid(value); break;

				case "syscreated":
				case "sysmodified":
				case "CIGCopyTime":
					value = RawCellMapper.AsDate(value); break;
			}
			return $"{value}\n";
		}
	}
}