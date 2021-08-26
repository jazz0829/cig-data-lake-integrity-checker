using System.Collections.Generic;
using DataIntegrityCheckerTests.DataAccess;
using Microsoft.Extensions.Configuration;

namespace DataIntegrityCheckerTests
{
	public static class Services
	{
		public static ICiDbConfiguration CiDbConfiguration { get; }
		public static IAthenaConfiguration RawAthenaConfiguration { get; }
		public static IAthenaConfiguration DomainAthenaConfiguration { get; }
		
		public static List<TestExecutionConfig> TestsToExecute { get; set; } = new List<TestExecutionConfig>();


		/// <summary>
		/// Static constructor here is used as Startup. It is enough for now.
		/// But this logic should go to some Startup.cs once the solution grows significantly
		/// </summary>
		static Services()
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				// To add multiple environment configurations
				// .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.Build();

			CiDbConfiguration = new CiDbConfiguration();
			RawAthenaConfiguration = new RawAthenaConfiguration();
			DomainAthenaConfiguration = new DomainAthenaConfiguration();
			config.GetSection("CiDbConfiguration").Bind(CiDbConfiguration);
			config.GetSection("AthenaConfiguration").Bind(RawAthenaConfiguration);
			config.GetSection("AthenaConfiguration").Bind(DomainAthenaConfiguration);

			InitRunConfiguration();
		}

		public static TestsToExecuteInitializationDetails InitRunConfiguration(
			IEnumerable<string> filesAndDirectories = null)
		{
			var initializationDetails = new TestsToExecuteInitializationDetailsBuilder()
				.FromConsoleInputs(filesAndDirectories)
				.UsingYamlFilesIncludingFilesFromDirectories()
				.WithOnlyExistingFilesParsed()
				.WithIdentifiedTestsOfInvalidScope()
				.Build();

			TestsToExecute = initializationDetails.TestsToExecute;

			return initializationDetails;
		}
	}
}