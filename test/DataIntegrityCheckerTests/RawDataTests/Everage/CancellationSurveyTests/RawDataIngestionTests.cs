using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using DataIntegrityCheckerTests.DataAccess;
using DataIntegrityCheckerTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DataIntegrityCheckerTests.RawDataTests.Everage.CancellationSurveyTests
{
	[UseReporter(typeof(DiffReporter))]
	public class RawDataIngestionTests : IDisposable
	{
		private string sqlTableName = "raw.Evergage_CancellationSurvey";
		private string athenaTableName = "\"customerintelligence_raw\".\"source_evergage\"";

		private AWSDataLakeRepository awsRepo = new AWSDataLakeRepository(Services.RawAthenaConfiguration);
		private CIDBRepository sqlRepo = new CIDBRepository(Services.CiDbConfiguration);

		[Fact(Skip = "Skip for now")]
		public void source_and_athena_table_column_names_match()
		{
			var sqlColumnNames = sqlRepo.GetTableColumnNames(sqlTableName);
			var athenaColumnNames = awsRepo.GetTableColumnNamesAsString(athenaTableName);

			sqlColumnNames.AddRange(Consts.AthenaIngestionDatesColumns);
			sqlColumnNames.Add("object");
			sqlColumnNames.Sort();
			var expected = string.Join("\n", sqlColumnNames).ToLowerInvariant();

			Approvals.Verify(new TwoTextsApprovalFileWriter(
				expected, athenaColumnNames, nameOfExpected: $"SQL table - {sqlTableName}"));
		}

		[Fact(Skip = "Skip for now")]
		public void number_of_rows_in_source_and_athena_tables_match()
		{
			var countInSQL = sqlRepo.CountInTable(sqlTableName);
			var countInAthena = awsRepo.CountInTableToday(athenaTableName);

			///countInAthena.Should()
			//	.Be(countInSQL, because: $"{sqlTableName} -> {athenaTableName}");
		}

		[Fact(Skip = "Skip for now")]
		public void data_in_source_and_athena_tables_match()
		{
			var limit = 5000;

			var sqlData = sqlRepo.GetTopFromTable(sqlTableName, "AccountName", limit);
			var actual = awsRepo.GetTopIngestedTodayFromTable(athenaTableName, "accountname", limit);

			actual.Count.Should().BeGreaterThan(0);

			var columnNames = (sqlData.First() as IDictionary<string, object>).Keys;
			var transformation = new RawEvergageCancellationSurveyTransformation();
			foreach (var columnName in columnNames)
			{
				var sqlColumnValues = "";
				var athenaColumnValues = "";
				for (var i = 0; i < sqlData.Count; i++)
				{
					athenaColumnValues += $"{actual[i][columnName.ToLowerInvariant()]}\n";
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