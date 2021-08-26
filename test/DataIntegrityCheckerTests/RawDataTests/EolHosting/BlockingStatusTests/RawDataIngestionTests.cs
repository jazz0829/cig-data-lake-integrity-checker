using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using DataIntegrityCheckerTests.DataAccess;
using DataIntegrityCheckerTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DataIntegrityCheckerTests.RawDataTests.EolHosting.BlockingStatusTests
{
	[UseReporter(typeof(DiffReporter))]
	public class RawDataIngestionTests : IDisposable
	{
		private string sqlTableName = "raw.HOST_CIG_BlockingStatus";
		private string athenaTableName = "\"customerintelligence_raw\".\"object_host_cig_blockingstatus\"";

		private AWSDataLakeRepository awsRepo = new AWSDataLakeRepository(Services.RawAthenaConfiguration);
		private CIDBRepository sqlRepo = new CIDBRepository(Services.CiDbConfiguration);

		[Fact(Skip = "Skip for now")]
		public void source_and_athena_table_column_names_match()
		{
			var sqlColumnNames = sqlRepo.GetTableColumnNames(sqlTableName);
			var athenaColumnNames = awsRepo.GetTableColumnNamesAsString(athenaTableName);

			sqlColumnNames.AddRange(Consts.AthenaIngestionDatesColumns);
			sqlColumnNames.Sort();
			var expected = string.Join("\n", sqlColumnNames).ToLowerInvariant();

			Approvals.Verify(new TwoTextsApprovalFileWriter(
				expected, athenaColumnNames, nameOfExpected: $"SQL table - {sqlTableName}"));
		}

		[Fact(Skip = "Skip for now")]
		public void number_of_rows_in_source_and_athena_tables_match()
		{
			var countInSQL = sqlRepo.CountInTableToday(sqlTableName, "CIGCopyTime");
			var countInAthena = awsRepo.CountInTableToday(athenaTableName);

			var numberOfEolEnvironments = 7;
			var actual = (int.Parse(countInAthena) / numberOfEolEnvironments).ToString();

			actual.Should()
				.Be(countInSQL, because: $"{sqlTableName} -> {athenaTableName}");
		}

		[Fact(Skip = "Skip for now")]
		// These table is unusual, because there are 16 records in SQL
		// And several thousands in AWS. So some tricks are needed to 
		// reduce AWS columns to 16
		public void data_in_source_and_athena_tables_match()
		{
			var limit = 5000;

			var sqlData = sqlRepo.GetTopFromTable(sqlTableName, "CAST(ID AS varchar),UPPER(Environment)", limit);
			var actual = awsRepo.GetTopIngestedTodayFromTable(athenaTableName, "UPPER(environment), id", limit);

			actual.Count.Should().BeGreaterThan(0);

			var columnNames = (sqlData.First() as IDictionary<string, object>).Keys;
			var transformation = new RawBlockingStatusTransformation();
			foreach (var columnName in columnNames)
			{
				// 16 SQL table rows are same for every Environment
				if (columnName == "Environment")
				{
					continue;
				}
				var sqlColumnValues = "";
				var athenaColumnValues = "";
				for (var i = 0; i < sqlData.Count; i++)
				{
					athenaColumnValues += transformation.PrepareAwsValue(columnName, actual[i]);
					sqlColumnValues += transformation.Execute(columnName, sqlData[i]);
				}

				var nameOfExpected = $"{columnName} {sqlTableName}";
				Approvals.Verify(new TwoTextsApprovalFileWriter(
					sqlColumnValues, athenaColumnValues, nameOfExpected: nameOfExpected));
			}
		}

		public void Dispose()
		{
			sqlRepo.Dispose();
			awsRepo.Dispose();
		}
	}
}