using System.Collections.Generic;
using DataIntegrityCheckerTests.DataUtils;

namespace DataIntegrityCheckerTests.RawDataTests.Everage.CancellationSurveyTests
{
	public class RawEvergageCancellationSurveyTransformation : ITransformation
	{
		public string Execute(string columnName, object row)
		{
			var value = (row as IDictionary<string, object>)[columnName];

			switch (columnName)
			{
				case "SegmentJoined":
					value = RawCellMapper.AsDateTime(value); break;
			}
			return $"{value}\n";
		}
	}
}