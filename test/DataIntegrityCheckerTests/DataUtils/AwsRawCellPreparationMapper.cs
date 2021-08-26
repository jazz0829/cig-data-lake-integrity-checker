namespace DataIntegrityCheckerTests.DataUtils
{
	public class AwsRawCellPreparationMapper
	{
		public static string AsFloat(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			return double.Parse(value).ToString("0.0#");
		}

		public static string AsBool(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			return value == "0" || value.ToLowerInvariant() == "false" 
				? "False" : "True";
		}

		public static string AsEnvironmentCode(object cellValue)
		{
			return cellValue?.ToString().Trim().ToUpperInvariant();
		}

		public static string AsDate(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			var timeIndex = value.LastIndexOf(" ");
			if (timeIndex >= 0)
			{
				value = value.Remove(timeIndex);
			}
			return value;
		}
		public static string AsDateTimeWithNoMilliseconds(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			var msIndex = value.LastIndexOf(".");
			if (msIndex >= 0)
			{
				value = value.Remove(msIndex);
			}
			return value.Replace("T", " ").Replace("Z", "");
		}
	}
}
