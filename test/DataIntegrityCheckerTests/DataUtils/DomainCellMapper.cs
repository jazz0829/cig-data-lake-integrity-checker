using System;
using System.Linq;

namespace DataIntegrityCheckerTests.DataUtils
{
	public static class DomainCellMapper
	{
		public static string AsGuid(object cellValue)
		{
			return cellValue?.ToString().ToUpperInvariant();
		}

		public static string AsTrimmedString(object cellValue)
		{
			if (cellValue == null)
			{
				return (string)null;
			}
			var value = cellValue.ToString();
			value = string.Join("\n", value.Replace("\r\n", "\n")
				.Split('\n', StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.TrimStart()));

			return value.TrimStart().TrimEnd();
		}

		public static string AsEnvironmentCode(object cellValue)
		{
			return cellValue?.ToString().TrimStart().TrimEnd().ToUpperInvariant();
		}

		public static string AsDateTime(object cellValue)
		{
			if (cellValue == null)
			{
				return (string)null;
			}
			var date = DateTime.Parse(cellValue.ToString());

			return date.ToString("yyyy-MM-dd HH:mm:ss.fff");
		}

		public static string AsDate(object cellValue)
		{
			if (cellValue == null)
			{
				return (string)null;
			}
			var date = DateTime.Parse(cellValue.ToString());

			return date.ToString("yyyy-MM-dd");
		}

		public static string AsBool(object cellValue)
		{
			if (cellValue == null)
			{
				return (string)null;
			}

			dynamic value = cellValue.ToString();
			if (value == "0" || value == "1")
			{
				value = int.Parse(value);
			}
			return Convert.ToBoolean(value).ToString().ToLowerInvariant();
		}

		public static string AsFloat(object cellValue)
		{
			if (cellValue == null)
			{
				return (string)null;
			}
			return double.Parse(cellValue.ToString()).ToString("0.0###");
		}
	}
}