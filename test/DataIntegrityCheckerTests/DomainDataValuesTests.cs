using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using DataIntegrityCheckerTests.DataAccess;
using DataIntegrityCheckerTests.DataUtils;
using DataIntegrityCheckerTests.Infrastructure;
using Xunit;
using Xunit.Sdk;

namespace DataIntegrityCheckerTests
{
	[UseReporter(typeof(DiffReporter))]
	public class DomainDataValuesTests : IDisposable
	{
		public static IEnumerable<object[]> TablesToTest = Services.TestsToExecute
			.Where(t => t.scope.Contains(Consts.ValuesPerColumn))
			.Select(c => new[] { c.source, c.target });

		private AWSDataLakeRepository awsRepo = new AWSDataLakeRepository(Services.DomainAthenaConfiguration);
		private CIDBRepository sqlRepo = new CIDBRepository(Services.CiDbConfiguration);

		[Theory]
		[Trait("Scope", Consts.ValuesPerColumn)] // Needed for tests filtering during console run
		[MemberData(nameof(TablesToTest))]
		public void target_domain_data_match_source_domain_data(
			SourceConfig source,
			SourceConfig target)
		{
			var sqlTableName = source.name;
			var awsTableName = target.name;
			var limitSql = source?.options?.limit ?? 5000;
			var limitAws = target?.options?.limit ?? 5000;
			var sqlOrderBy = source?.options?.order_by;
			var awsOrderBy = target?.options?.order_by;

			var domainSqlTableName = $"domain.{sqlTableName}";
			var domainAwsTableName = $"\"customerintelligence\".\"{awsTableName}\"";

			var sqlData = sqlRepo.GetTopFromTable(domainSqlTableName, sqlOrderBy, limitSql);
			var awsData = awsRepo.GetTopFromTable(domainAwsTableName, awsOrderBy, limitAws);

			var transformation = new DomainTransformation();

			var columns = sqlRepo.GetTableColumnTypes(domainSqlTableName)
				.Select(c => ColumnMappingExceptionHandler.HandleOverrideColumnType(c, source.options));

			var awsColumnsCount = awsData.FirstOrDefault()?.Count ?? 0;
			checkColumnsCount(awsColumnsCount, columns.Count(), source, target);

			foreach (var sqlColumn in columns)
			{
				var sqlColumnValues = "";
				var athenaColumnValues = "";
				var sqlColumnName = sqlColumn.ColumnName;
				var awsColumnName = ColumnMappingExceptionHandler
					.HandleOverrideColumnName(sqlColumn, source.options)
					.ColumnName;

				if (ColumnMappingExceptionHandler.HasToBeSkipped(sqlColumnName, source.options) ||
					ColumnMappingExceptionHandler.HasToBeSkipped(awsColumnName, target.options))
				{
					continue;
				}

				for (var i = 0; i < sqlData.Count; i++)
				{
					athenaColumnValues += transformation.PrepareAwsValue(awsColumnName, sqlColumn, awsData[i]);
					sqlColumnValues += transformation.TransformSqlValueToAws(sqlColumn, sqlData[i]);
				}
				Approvals.Verify(new TwoTextsApprovalFileWriter(
					sqlColumnValues, athenaColumnValues,
					nameOfActual: $"{target.type} - {awsColumnName} {awsTableName}",
					nameOfExpected: $"{source.type} - {sqlColumnName} {sqlTableName}"));
			}
		}

		private void checkColumnsCount(
			int awsColumnsCount,
			int sqlColumnsCount,
			SourceConfig source,
			SourceConfig target)
		{
			if (awsColumnsCount == 0)
			{
				return;
			}
			var awsColumnsExcludingSkippedCount = awsColumnsCount - target.options?.skip_columns?.Count ?? 0;
			var sqlColumnsExcludingSkippedCount = sqlColumnsCount - source.options?.skip_columns?.Count ?? 0;

			if (awsColumnsExcludingSkippedCount != sqlColumnsExcludingSkippedCount)
			{
				throw new XunitException($"Number of columns of : {source.type} {source.name}" +
									   $"\nshould be the same as: {target.type} {target.name}");
			}
		}

		public void Dispose()
		{
			sqlRepo.Dispose();
			awsRepo.Dispose();
		}
	}
}