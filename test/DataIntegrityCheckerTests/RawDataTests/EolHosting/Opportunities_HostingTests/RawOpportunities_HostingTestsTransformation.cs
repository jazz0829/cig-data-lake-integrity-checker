using System;
using System.Collections.Generic;
using System.Linq;
using DataIntegrityCheckerTests.DataUtils;

namespace DataIntegrityCheckerTests.RawDataTests.EolHosting.Opportunities_HostingTests
{
	public class RawOpportunitiesHostingTransformation : ITransformation
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
				case "AmountFC":
				case "AmountDC":
				case "RateFC":
					value = AwsRawCellPreparationMapper.AsFloat(value); break;

				case "Name":
				case "Notes":
				case "NextAction":
					if (value != null)
					{
						value = value.ToUpper();
						value = string.Join("\n", value.ToUpper().Replace("\r\n", "\n")
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
				case "Account":
				case "Accountant":
				case "Owner":
				case "OpportunityStage":
				case "Reseller":
				case "LeadSource":
				case "SalesType":
				case "ReasonCode":
				case "ExchangeRateType":
				case "Competitor":
				case "Campaign":
				case "Project":
				case "Extension":
					value = RawCellMapper.AsGuid(value); break;

				case "Environment":
					value = RawCellMapper.AsEnvironmentCode(value); break;

				case "Probability":
				case "AmountFC":
				case "AmountDC":
				case "RateFC":
					value = RawCellMapper.AsFloat(value); break;

				case "CIGCopyTime":
				case "syscreated":
				case "sysmodified":
				case "CloseDate":
				case "ActionDate":
					value = RawCellMapper.AsDate(value); break;

				case "Name":
				case "Notes":
				case "NextAction":
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