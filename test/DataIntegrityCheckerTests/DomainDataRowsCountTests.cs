using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using DataIntegrityCheckerTests.DataAccess;
using Xunit;
using Xunit.Sdk;

namespace DataIntegrityCheckerTests
{
	[UseReporter(typeof(DiffReporter))]
	public class DomainDataRowsCountTests : IDisposable
	{
		public static IEnumerable<object[]> TablesToTest = Services.TestsToExecute
			.Where(t => t.scope.Contains(Consts.RowCount))
			.Select(c => new[] { c.source, c.target });

		private AWSDataLakeRepository awsRepo = new AWSDataLakeRepository(Services.DomainAthenaConfiguration);
		private CIDBRepository sqlRepo = new CIDBRepository(Services.CiDbConfiguration);

		[Theory]
		[Trait("Scope", Consts.RowCount)] // Needed for tests filtering during console run
		[MemberData(nameof(TablesToTest))]
		public void number_of_rows_match(
			SourceConfig source,
			SourceConfig target)
		{
			var sqlTableName = source.name;
			var awsTableName = target.name;
			var domainSqlTableName = $"domain.{sqlTableName}";
			var domainAwsTableName = $"\"customerintelligence\".\"{awsTableName}\"";

			var countInSql = sqlRepo.CountInTable(domainSqlTableName);
			var countInAthena = awsRepo.CountInTable(domainAwsTableName);
			if (!countInAthena.Equals(countInSql, StringComparison.InvariantCulture))
			{
				throw new XunitException($"Expected ({source.type}): {countInSql}\n  Actual ({target.type}): {countInAthena}");
			}
		}

		public void Dispose()
		{
			sqlRepo.Dispose();
			awsRepo.Dispose();
		}
	}
}