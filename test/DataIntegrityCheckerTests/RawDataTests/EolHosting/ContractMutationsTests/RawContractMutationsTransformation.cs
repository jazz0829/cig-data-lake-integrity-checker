using System.Collections.Generic;
using DataIntegrityCheckerTests.DataUtils;

namespace DataIntegrityCheckerTests.RawDataTests.EolHosting.ContractMutationsTests
{
	public class RawContractMutationsTransformation : ITransformation
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
				case "EventDate":
				case "CIGCopyTime":
					value = AwsRawCellPreparationMapper.AsDate(value);
					break;
				case "Quantity":
				case "ContractLineValue":
					value = AwsRawCellPreparationMapper.AsFloat(value);
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
			if (value == null)
			{
				return value as string;
			}
			switch (columnName)
			{
				case "ID":
				case "Event":
				case "Contract":
				case "Account":
				case "Item":
				case "ContractLine":
				case "syscreator":
				case "sysmodifier":
					value = RawCellMapper.AsGuid(value);
					break;
				case "Environment":
					value = RawCellMapper.AsEnvironmentCode(value);
					break;
				case "syscreated":
				case "sysmodified":
				case "EventDate":
				case "CIGCopyTime":
					value = RawCellMapper.AsDate(value);
					break;
				case "Quantity":
				case "ContractLineValue":
					value = RawCellMapper.AsFloat(value);
					break;
			}
			return $"{value}\n";
		}
	}
}