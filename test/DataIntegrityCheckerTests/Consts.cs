using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DataIntegrityCheckerTests
{
	public static class Consts
	{
		public static List<string> AthenaIngestionDatesColumns = new List<string>
		{
			"ingestionday", "ingestionmonth", "ingestionyear"
		};

		public const string RowCount = "row_count";
		public const string TableSchema = "table_schema";
		public const string ValuesPerColumn = "values_per_column";
		public static List<string> KnownScopes = new List<string>
		{
			RowCount, TableSchema, ValuesPerColumn
		};

		public static string AssemblyDirectory
		{
			get
			{
				var codeBase = Assembly.GetExecutingAssembly().CodeBase;
				var uri = new UriBuilder(codeBase);
				var path = Uri.UnescapeDataString(uri.Path);

				return Path.GetDirectoryName(path);
			}
		}
	}
}