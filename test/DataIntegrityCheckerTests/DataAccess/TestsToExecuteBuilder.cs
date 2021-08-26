using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;

namespace DataIntegrityCheckerTests.DataAccess
{
	public class TestsToExecuteInitializationDetails
	{
		public List<string> ValidConfigFiles { get; set; }
		public List<string> NotFoundConfigFiles { get; set; }
		public List<string> InvalidFormatConfigFiles { get; set; } = new List<string>();
		public List<TestExecutionConfig> TestsToExecute { get; set; } = new List<TestExecutionConfig>();
		public List<TestExecutionConfig> TestsWithMissedScope { get; set; } = new List<TestExecutionConfig>();
		public List<TestExecutionConfig> TestsWithUnknownScope { get; set; } = new List<TestExecutionConfig>();
	}

	public interface ITestsToExecuteInitializationDetailsBuilder
	{
		ITestsToExecuteInitializationDetailsBuilder FromConsoleInputs(IEnumerable<string> inputs);
		ITestsToExecuteInitializationDetailsBuilder UsingYamlFilesIncludingFilesFromDirectories();
		ITestsToExecuteInitializationDetailsBuilder WithOnlyExistingFilesParsed();
		ITestsToExecuteInitializationDetailsBuilder WithIdentifiedTestsOfInvalidScope();
		TestsToExecuteInitializationDetails Build();
	}

	public class TestsToExecuteInitializationDetailsBuilder : ITestsToExecuteInitializationDetailsBuilder
	{
		private TestsToExecuteInitializationDetails details = new TestsToExecuteInitializationDetails();

		public ITestsToExecuteInitializationDetailsBuilder FromConsoleInputs(
			IEnumerable<string> inputs)
		{
			if (inputs == null || inputs.Count() == 0)
			{
				var currentDirectory = Consts.AssemblyDirectory;
				inputs = new List<string>{ currentDirectory };
			}

			var files = new List<string>();
			foreach (var input in inputs)
			{
				if (Directory.Exists(input))
				{
					files.AddRange(getAllConfigFilesFromDirectory(input));
				}
				else
				{
					files.Add(input);
				}
			}

			details.ValidConfigFiles = files;
			return this;
		}

		public ITestsToExecuteInitializationDetailsBuilder UsingYamlFilesIncludingFilesFromDirectories()
		{
			var configFiles = details.ValidConfigFiles;
			var notExistingFiles = configFiles?.Where(f => !File.Exists(f)) ?? new List<string>();
			var existingFiles = configFiles?.Where(File.Exists) ?? new List<string>();

			details.NotFoundConfigFiles = notExistingFiles.ToList();
			details.ValidConfigFiles = existingFiles.ToList();

			return this;
		}

		public ITestsToExecuteInitializationDetailsBuilder WithOnlyExistingFilesParsed()
		{
			var yamlDeserializer = new DeserializerBuilder().Build();
			details.ValidConfigFiles.ForEach(file =>
			{
				try
				{
					var fileContent = File.OpenText(file);
					var tests = yamlDeserializer.Deserialize<List<TestExecutionConfig>>(fileContent);
					details.TestsToExecute.AddRange(tests);
				}
				catch
				{
					details.InvalidFormatConfigFiles.Add(file);
				}
			});
			details.InvalidFormatConfigFiles.ForEach(invalidFile =>
					details.ValidConfigFiles.Remove(invalidFile));
			return this;
		}

		public ITestsToExecuteInitializationDetailsBuilder WithIdentifiedTestsOfInvalidScope()
		{
			details.TestsWithMissedScope = details.TestsToExecute
				.Where(t => t.scope == null || t.scope.Count == 0).ToList();

			details.TestsWithUnknownScope = details.TestsToExecute
				.Where(t => t.scope != null &&
							t.scope.Any(test => !Consts.KnownScopes.Contains(test))).ToList();

			details.TestsToExecute = details.TestsToExecute
				.Except(details.TestsWithMissedScope)
				.Except(details.TestsWithUnknownScope).ToList();
			return this;
		}

		public TestsToExecuteInitializationDetails Build()
		{
			return details;
		}

		private static IEnumerable<string> getAllConfigFilesFromDirectory(
			string directory)
		{
			return Directory.GetFiles(
				directory, "*.yaml", SearchOption.AllDirectories);
		}
	}
}
