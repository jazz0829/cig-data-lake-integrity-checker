using System;
using System.Linq;

namespace DataIntegrityCheckerTests.DataUtils
{
	public class AwsDomainCellPreparationMapper
	{
		public static string AsBool(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			return value == "0" || value.ToLowerInvariant() == "false"
				? "false" : "true";
		}

		public static string AsTrimmedString(string value)
		{
			if (value != null)
			{
				value = string.Join("\n", value.Replace("\r\n", "\n")
					.Split('\n', StringSplitOptions.RemoveEmptyEntries)
					.Select(s => s.TrimStart()));
			}
			return value?.TrimStart().TrimEnd();
		}

		public static string AsDateTime(string value)
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
			value = value.Replace("Z", "") + ".000";
			return value;
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
			timeIndex = value.LastIndexOf("T");
			if (timeIndex >= 0)
			{
				value = value.Remove(timeIndex);
			}
			return value;
		}

		public static string AsFloat(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			return double.Parse(value).ToString("0.0###");
		}
	}
}
