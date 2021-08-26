using System.Collections.Generic;
using DataIntegrityCheckerTests.DataUtils;

namespace DataIntegrityCheckerTests.RawDataTests.EolHosting.ContractLinesTests
{
	public class RawContractLinesTransformation : ITransformation
	{
		public string PrepareAwsValue(
			string columnName, Dictionary<string, string> row)
		{
			var value = row[columnName.ToLowerInvariant()];
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			switch (columnName)
			{
				case "syscreated":
				case "sysmodified":
				case "StartDate":
				case "FinalDate":
				case "EndDate":
				case "CancellationDate":
				case "CIGCopyTime":
					value = AwsRawCellPreparationMapper.AsDate(value);
					break;
				case "Price":
				case "Quantity":
				case "UnitFactor":
				case "Discount":
					value = AwsRawCellPreparationMapper.AsFloat(value);
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
			if (value == null)
			{
				return value as string;
			}
			switch (columnName)
			{
				case "ID":
				case "ContractID":
				case "ContractEventID":
				case "CancellationEventID":
				case "Item":
				case "ItemPrice":
				case "syscreator":
				case "sysmodifier":
					value = RawCellMapper.AsGuid(value);
					break;
				case "syscreated":
				case "sysmodified":
				case "StartDate":
				case "FinalDate":
				case "EndDate":
				case "CancellationDate":
				case "CIGCopyTime":
					value = RawCellMapper.AsDate(value);
					break;
				case "Price":
				case "Quantity":
				case "UnitFactor":
				case "Discount":
					value = RawCellMapper.AsFloat(value);
					break;
			}
			return $"{value}\n";
		}
	}
}