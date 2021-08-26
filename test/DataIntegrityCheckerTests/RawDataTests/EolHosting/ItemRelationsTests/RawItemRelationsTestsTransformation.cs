using System.Collections.Generic;
using DataIntegrityCheckerTests.DataUtils;

namespace DataIntegrityCheckerTests.RawDataTests.EolHosting.ItemRelationsTests
{
	public class RawItemRelationsTestsTransformation : ITransformation
	{
		public string PrepareAwsValue(
			string columnName, Dictionary<string, string> row)
		{
			var value = row[columnName.ToLowerInvariant()];

			switch (columnName)
			{
				case "Quantity":
				case "MaxQuantity":
					value = AwsRawCellPreparationMapper.AsFloat(value); break;

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
				case "ItemRelated":
				case "Item":
					value = RawCellMapper.AsGuid(value); break;

				case "Quantity":
				case "MaxQuantity":
					value = RawCellMapper.AsFloat(value); break;

				case "CIGCopyTime":
					value = RawCellMapper.AsDate(value); break;
			}
			return $"{value}\n";
		}
	}
}