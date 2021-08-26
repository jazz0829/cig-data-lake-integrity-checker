using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Amazon.Athena;
using Amazon.Athena.Model;
using Amazon.Runtime;
using DataIntegrityCheckerTests.DataUtils;

namespace DataIntegrityCheckerTests.DataAccess
{
	public class AWSDataLakeRepository : IDisposable
	{
		private readonly QueryExecutionContext queryContext;
		private readonly ResultConfiguration resultConfig;
		private readonly AmazonAthenaClient client;

		public AWSDataLakeRepository(IAthenaConfiguration config)
		{
			queryContext = new QueryExecutionContext();
			queryContext.Database = config.QueryContextDataBase;
			resultConfig = new ResultConfiguration();
			resultConfig.OutputLocation = config.QueryOutputLocation;

			client = new AmazonAthenaClient(
				new BasicAWSCredentials(config.AccessKey, config.AccessSecret),
				Amazon.RegionEndpoint.EUWest1);
		}

		public string CountInTableToday(string tableName)
		{
			var today = DateTime.UtcNow.Date;
			var request = new StartQueryExecutionRequest
			{
				QueryString =
					$"SELECT COUNT(0) FROM {tableName} " +
					$"WHERE ingestionyear = '{today.Year}' AND ingestionmonth = '{today.Month}' AND ingestionday = '{today.Day}'",
				QueryExecutionContext = queryContext,
				ResultConfiguration = resultConfig
			};

			var response = client.StartQueryExecutionAsync(request).Result;

			var queryId = response.QueryExecutionId;
			waitForQueryToComplete(queryId);

			return readCountFromQueryResults(queryId);
		}

		public string CountInTable(string tableName)
		{
			var request = new StartQueryExecutionRequest
			{
				QueryString =
					$"SELECT COUNT(0) FROM {tableName};",
				QueryExecutionContext = queryContext,
				ResultConfiguration = resultConfig
			};

			var response = client.StartQueryExecutionAsync(request).Result;

			var queryId = response.QueryExecutionId;
			waitForQueryToComplete(queryId);

			return readCountFromQueryResults(queryId);
		}

		public List<Dictionary<string, string>> GetTopFromTable(
			string tableName,
			string orderByColumnName,
			int top)
		{
			var request = new StartQueryExecutionRequest
			{
				QueryString = $"SELECT * FROM {tableName}" +
							  $"ORDER BY {orderByColumnName} " +
							  $"limit {top};",
				QueryExecutionContext = queryContext,
				ResultConfiguration = resultConfig
			};
			var response = client.StartQueryExecutionAsync(request).Result;

			var queryId = response.QueryExecutionId;
			waitForQueryToComplete(queryId);

			var items = processResultRows(queryId);

			//cleanRequestResults(queryId);

			return (items.Skip(1)).ToList();
		}

		public List<Dictionary<string, string>> GetTopIngestedTodayFromTable(
			string tableName,
			string orderByColumnName,
			int top)
		{
			var today = DateTime.UtcNow.Date;
			var request = new StartQueryExecutionRequest
			{
				QueryString = $"SELECT * FROM {tableName}" +
							  $"WHERE ingestionyear = '{today.Year}' AND ingestionmonth = '{today.Month}' AND ingestionday = '{today.Day}'" +
							  $"ORDER BY {orderByColumnName} " +
							  $"limit {top};",
				QueryExecutionContext = queryContext,
				ResultConfiguration = resultConfig
			};
			var response = client.StartQueryExecutionAsync(request).Result;

			var queryId = response.QueryExecutionId;
			waitForQueryToComplete(queryId);

			var items = processResultRows(queryId);

			//cleanRequestResults(queryId);

			return (items.Skip(1)).ToList();
		}

		public string GetTableColumnNamesAsString(string tableName)
		{
			var request = new StartQueryExecutionRequest
			{
				QueryString = $"SELECT * FROM {tableName} limit 1;",
				QueryExecutionContext = queryContext,
				ResultConfiguration = resultConfig
			};
			var response = client.StartQueryExecutionAsync(request).Result;

			var queryId = response.QueryExecutionId;
			waitForQueryToComplete(queryId);

			var items = processResultRows(queryId);

			//cleanRequestResults(queryId);

			var columnNames = items.First().Keys.ToList();
			columnNames.Sort();

			return string.Join("\n", columnNames);
		}

		public List<ColumnType> GetTableColumnTypes(
			string tableName)
		{
			var request = new StartQueryExecutionRequest
			{
				QueryString = $"SELECT * FROM {tableName} limit 1;",
				QueryExecutionContext = queryContext,
				ResultConfiguration = resultConfig
			};
			var response = client.StartQueryExecutionAsync(request).Result;

			var queryId = response.QueryExecutionId;
			waitForQueryToComplete(queryId);

			var columnsInfo = getColumnsInfo(queryId);

			//cleanRequestResults(queryId);

			return columnsInfo
				.Select(c => new ColumnType(c.Name, c.Type, c.Nullable.Value))
				.ToList();
		}

		private string readCountFromQueryResults(string queryExecutionId)
		{
			var queryRequest = new GetQueryResultsRequest
			{
				QueryExecutionId = queryExecutionId
			};

			var queryResult = client.GetQueryResultsAsync(queryRequest).GetAwaiter().GetResult();

			var count = "0";

			var queryResults = queryResult.ResultSet.Rows.Last();

			count = queryResults.Data.Last().VarCharValue;

			return count;
		}

		private void cleanRequestResults(string queryExecutionId)
		{
			var deleteNamedQueryRequest = new DeleteNamedQueryRequest
			{
				NamedQueryId = queryExecutionId
			};
			client.DeleteNamedQueryAsync(deleteNamedQueryRequest).GetAwaiter().GetResult();
		}

		private void waitForQueryToComplete(string queryExecutionId)
		{
			var getQueryExecutionRequest = new GetQueryExecutionRequest
			{
				QueryExecutionId = queryExecutionId
			};

			GetQueryExecutionResponse getQueryExecutionResult = null;

			var isQueryStillRunning = true;
			while (isQueryStillRunning)
			{
				getQueryExecutionResult =
					client.GetQueryExecutionAsync(getQueryExecutionRequest).Result;

				var queryState = getQueryExecutionResult.QueryExecution.Status.State;
				if (queryState == QueryExecutionState.FAILED)
				{
					throw new Exception($"Query Failed to run with Error Message: {getQueryExecutionResult.QueryExecution.Status.StateChangeReason}");
				}
				else if (queryState == QueryExecutionState.CANCELLED)
				{
					throw new Exception("Query was cancelled.");
				}
				else if (queryState == QueryExecutionState.SUCCEEDED)
				{
					isQueryStillRunning = false;
				}
				else
				{
					// Sleep before retrying.
					Thread.Sleep(1000);
				}
			}
		}

		private IList<Dictionary<string, string>> processResultRows(string queryExecutionId)
		{
			var queryRequest = new GetQueryResultsRequest
			{
				QueryExecutionId = queryExecutionId
			};

			var queryResult = client.GetQueryResultsAsync(queryRequest).GetAwaiter().GetResult();

			var columnInfoList = queryResult.ResultSet.ResultSetMetadata.ColumnInfo;

			var results = new List<Dictionary<string, string>>();
			while (true)
			{
				var queryResults = queryResult.ResultSet.Rows;
				foreach (var row in queryResults)
				{
					// Process the row. The first row of the first page holds the column names.
					results.Add(processRow(row, columnInfoList));
				}

				// If nextToken is null, there are no more pages to read. Break out of the loop.
				if (queryResult.NextToken == null)
				{
					break;
				}

				queryRequest.NextToken = queryResult.NextToken;
				queryResult = client.GetQueryResultsAsync(queryRequest).Result;
			}

			return results;
		}

		private IList<ColumnInfo> getColumnsInfo(string queryExecutionId)
		{
			var queryRequest = new GetQueryResultsRequest
			{
				QueryExecutionId = queryExecutionId
			};

			var queryResult = client.GetQueryResultsAsync(queryRequest).GetAwaiter().GetResult();

			return queryResult.ResultSet.ResultSetMetadata.ColumnInfo;
		}

		private static Dictionary<string, string> processRow(
			Row row, IList<ColumnInfo> columnInfoList)
		{
			var rowResult = new Dictionary<string, string>();

			for (var i = 0; i < columnInfoList.Count; i++)
			{
				var record = row.Data[i].VarCharValue;
				rowResult[columnInfoList[i].Name] = record;
			}
			return rowResult;
		}

		public void Dispose()
		{
			client.Dispose();
		}
	}
}