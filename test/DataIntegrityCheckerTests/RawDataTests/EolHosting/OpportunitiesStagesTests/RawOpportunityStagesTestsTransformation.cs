using System;
using System.Collections.Generic;
using System.Linq;
using DataIntegrityCheckerTests.DataUtils;

namespace DataIntegrityCheckerTests.RawDataTests.EolHosting.OpportunitiesStagesTests
{
	public class RawOpportunityStagesTestsTransformation : ITransformation
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
				case "CloseDate":
				case "ActionDate":
					value = AwsRawCellPreparationMapper.AsDate(value);
					break;

				case "CIGProcessed":
					value = AwsRawCellPreparationMapper.AsBool(value);
					break;

				case "Environment":
					value = AwsRawCellPreparationMapper.AsEnvironmentCode(value); break;

				case "Probability":
					value = AwsRawCellPreparationMapper.AsFloat(value); break;

				case "Name":
				case "Notes":
				case "NextAction":
					if (value != null)
					{
						value = value.ToUpper();
						value = string.Join("\n", value.Replace("\r\n", "\n")
							.Split('\n', StringSplitOptions.RemoveEmptyEntries)
							.Select(s => s.TrimStart()));
					}
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

				case "Environment":
					value = RawCellMapper.AsEnvironmentCode(value); break;

				case "Probability":
					value = RawCellMapper.AsFloat(value); break;

				case "CIGCopyTime":
				case "syscreated":
				case "sysmodified":
					value = RawCellMapper.AsDate(value); break;

				case "Notes":
					if (value != null)
					{
						value = value.ToString().ToUpper();
						value = string.Join("\n", value.ToString().Replace("\r\n", "\n")
							.Split('\n', StringSplitOptions.RemoveEmptyEntries)
							.Select(s => s.TrimStart()));
					}
					break;
			}
			return $"{value}\n";
		}
	}
}