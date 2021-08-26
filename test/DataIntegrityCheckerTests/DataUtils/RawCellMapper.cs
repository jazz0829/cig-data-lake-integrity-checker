using System;

namespace DataIntegrityCheckerTests.DataUtils
{
	public static class RawCellMapper
	{
		public static string AsGuid(object cellValue)
		{
			return cellValue?.ToString().ToUpperInvariant();
		}
		public static string AsEnvironmentCode(object cellValue)
		{
			return cellValue?.ToString().Trim().ToUpperInvariant();
		}

		public static string AsDateTimeWithNoMilliseconds(object cellValue)
		{
			if (cellValue == null)
			{
				return (string)null;
			}
			var date = DateTime.Parse(cellValue.ToString());

			return date
				// We skip transforming .ToUniversalTime()
				.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static string AsDateTime(object cellValue)
		{
			if (cellValue == null)
			{
				return (string)null;
			}
			var date = DateTime.Parse(cellValue.ToString());

			return date
				// We skip transforming .ToUniversalTime()
				.ToString("u");
		}

		public static string AsDate(object cellValue)
		{
			if (cellValue == null)
			{
				return (string)null;
			}
			var date = DateTime.Parse(cellValue.ToString());

			return date
				// We skip transforming .ToUniversalTime()
				.ToString("yyyy-MM-dd");
		}

		public static object AsFloat(object cellValue)
		{
			if (cellValue == null)
			{
				return (string)null;
			}
			return double.Parse(cellValue.ToString()).ToString("0.0#");
		}
	}
}