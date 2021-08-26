using System.Collections.Generic;
using DataIntegrityCheckerTests.DataUtils;

namespace DataIntegrityCheckerTests.RawDataTests.EolHosting.BlockingStatusTests
{
	public class RawBlockingStatusTransformation : ITransformation
	{
		public string PrepareAwsValue(
			string columnName, Dictionary<string, string> row)
		{
			var value = row[columnName.ToLowerInvariant()];

			switch (columnName)
			{
				case "CIGCopyTime":
					value = AwsRawCellPreparationMapper.AsDate(value);
					break;
			}
			return $"{value}\n";
		}
		public string Execute(string columnName, object row)
		{
			var value = (row as IDictionary<string, object>)[columnName];

			switch (columnName)
			{
				case "CIGCopyTime":
					value = RawCellMapper.AsDate(value); break;
			}
			return $"{value}\n";
		}
	}
}
