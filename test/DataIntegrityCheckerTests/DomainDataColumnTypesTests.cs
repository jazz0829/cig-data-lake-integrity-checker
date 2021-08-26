using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using DataIntegrityCheckerTests.DataAccess;
using DataIntegrityCheckerTests.DataUtils;
using DataIntegrityCheckerTests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace DataIntegrityCheckerTests
{
	[UseReporter(typeof(DiffReporter))]
	public class DomainDataColumnTypesTests : IDisposable
	{
		public static IEnumerable<object[]> TablesToTest = Services.TestsToExecute
			.Where(t => t.scope.Contains(Consts.TableSchema))
			.Select(c => new[] { c.source, c.target });

		private AWSDataLakeRepository awsRepo = new AWSDataLakeRepository(Services.DomainAthenaConfiguration);
		private CIDBRepository sqlRepo = new CIDBRepository(Services.CiDbConfiguration);

		[Theory]
		[Trait("Scope", Consts.TableSchema)] // Needed for tests filtering during console run
		[MemberData(nameof(TablesToTest))]
		public void column_types_match(
			SourceConfig source,
			SourceConfig target)
		{
			var sqlTableName = source.name;
			var awsTableName = target.name;
			var domainSqlTableName = $"domain.{sqlTableName}";
			var domainAwsTableName = $"\"customerintelligence\".\"{awsTableName}\"";

			var sqlColumnTypes = sqlRepo.GetTableColumnTypes(domainSqlTableName);
			var awsColumnTypes = awsRepo.GetTableColumnTypes(domainAwsTableName);

			var actualColumnTypes = prepareActualColumnTypes(awsColumnTypes, target.options);
			var expectedAwsColumns = prepareExpectedColumnTypes(sqlColumnTypes, source.options);

			compareColumnsAndWriteMismatchesToOutput(source, target, actualColumnTypes, expectedAwsColumns);
			compareColumnsWithApprovals(source, target, actualColumnTypes, expectedAwsColumns);
		}
		private IEnumerable<ColumnType> prepareExpectedColumnTypes(
			IEnumerable<ColumnType> sourceColumnTypes,
			SourceOptions options)
		{
			return sourceColumnTypes
				.Select(mappedColumn => ColumnMappingExceptionHandler.HandleOverrideColumnName(mappedColumn, options))
				.Select(ColumnTypeMapper.MapSqlToAthenaType)
				.Select(mappedColumn => ColumnMappingExceptionHandler.HandleOverrideColumnType(mappedColumn, options));
		}

		private IEnumerable<ColumnType> prepareActualColumnTypes(
			IEnumerable<ColumnType> targetColumnTypes,
			SourceOptions options)
		{
			return targetColumnTypes.Where(column => !ColumnMappingExceptionHandler.HasToBeSkipped(column.ColumnName, options));
		}

		private void compareColumnsWithApprovals(
			SourceConfig source,
			SourceConfig target,
			IEnumerable<ColumnType> actualColumns,
			IEnumerable<ColumnType> expectedAwsColumns)
		{
			var actual = string.Join("\n", actualColumns.OrderBy(c => c.ColumnName));
			var expected = string.Join("\n", expectedAwsColumns.OrderBy(c => c.ColumnName));

			Approvals.Verify(new TwoTextsApprovalFileWriter(
				expected, actual,
				nameOfActual: $"{target.type} table - {target.name}",
				nameOfExpected: $"{source.type} table (lowered case) - {source.name}"));
		}

		private readonly ITestOutputHelper output;
		public DomainDataColumnTypesTests(ITestOutputHelper output)
		{
			this.output = output;
		}
		private void compareColumnsAndWriteMismatchesToOutput(
			SourceConfig source,
			SourceConfig target,
			IEnumerable<ColumnType> targetColumns,
			IEnumerable<ColumnType> sourceColumns)
		{
			var message = new StringBuilder();
			foreach (var sourceColumn in sourceColumns)
			{
				var expected = targetColumns.FirstOrDefault(f => f.ColumnName == sourceColumn.ColumnName);
				if (expected != null)
				{
					if (!sourceColumn.TypeName.Equals(expected.TypeName, StringComparison.InvariantCulture))
					{
						message.Append(
							$"\n{expected.ColumnName} ({expected.TypeName}) column type is not matched with source type: {sourceColumn.TypeName}.");
					}
				}
				else
				{
					message.Append($"\n{sourceColumn.ColumnName} ({sourceColumn.TypeName}) is not found in target columns ({target.type}).");
				}
			}

			foreach (var targetColumn in targetColumns)
			{
				if (!sourceColumns.Any(c => c.ColumnName == targetColumn.ColumnName))
				{
					message.Append(
						$"\n{targetColumn.ColumnName} ({targetColumn.TypeName}) is not found in source columns ({source.type}).");
				}
			}

			if (message.Length > 0)
			{
				output.WriteLine(message.ToString());
				//throw new XunitException(message.ToString());
			}
		}

		public void Dispose()
		{
			sqlRepo.Dispose();
			awsRepo.Dispose();
		}
	}
}
