using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Dapper;
using DataIntegrityCheckerTests.DataUtils;

namespace DataIntegrityCheckerTests.DataAccess
{
	public class CIDBRepository : IDisposable
	{
		private readonly SqlConnection connection;
		private readonly int timeoutInSeconds = 15 * 60;

		public CIDBRepository(ICiDbConfiguration config)
		{
			connection = new SqlConnection(config.ConnectionString);
		}

		public string CountInTable(string tableName)
		{
			return connection.Query<string>($"SELECT COUNT(0) FROM {tableName}",
				commandTimeout: timeoutInSeconds).First();
		}

		public string CountInTableToday(
			string tableName,
			string ingestionDateColumnName)
		{
			return connection.Query<string>(
					$"SELECT COUNT(0) FROM {tableName} "+
					$"WHERE CAST({ingestionDateColumnName} AS date) = CAST(GETDATE() AS date)")
				.First();
		}

		public List<object> GetTopFromTable(
			string tableName,
			string orderByColumnName,
			int top)
		{
			// Important: Needed to handle years from DB properly like 1999 and 2099
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

			return connection.Query<object>(
				$"SELECT TOP {top} * FROM {tableName} ORDER BY {orderByColumnName}",
					commandTimeout: timeoutInSeconds)
				.ToList();
		}

		public List<string> GetTableColumnNames(
			string tableName)
		{
			var record = connection.Query($"SELECT TOP 1 * FROM {tableName}",
					commandTimeout: timeoutInSeconds).First();

			var columnNames = (record as IDictionary<string, object>).Keys.ToList();

			columnNames.Sort();

			return columnNames;
		}

		public List<ColumnType> GetTableColumnTypes(
			string tableName)
		{
			var tableSchemaAndName = tableName.Split(".");

			var columns = connection.Query(
				"SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE " +
				"FROM INFORMATION_SCHEMA.COLUMNS " +
				$"Where TABLE_SCHEMA='{tableSchemaAndName[0]}' AND TABLE_NAME='{tableSchemaAndName[1]}'");

			return columns.Select(c =>
			{
				var column = (c as IDictionary<string, object>);
				return new ColumnType(
					(string)column["COLUMN_NAME"],
					(string)column["DATA_TYPE"],
					(string)column["IS_NULLABLE"]);
			}).ToList();
		}

		public void Dispose()
		{
			connection.Close();
			connection.Dispose();
		}
	}
}